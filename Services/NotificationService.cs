using Microsoft.EntityFrameworkCore;
using ScanPlantAPI.Data;
using ScanPlantAPI.Models;
using ScanPlantAPI.Models.DTOs;
using ScanPlantAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScanPlantAPI.Services
{
    /// <summary>
    /// Serviço para gerenciamento e envio de notificações
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationSender _sender;

        public NotificationService(ApplicationDbContext context, INotificationSender sender)
        {
            _context = context;
            _sender = sender;
        }

        public async Task<IEnumerable<NotificationResponseDTO>> GetNotificationsAsync(
            string userId,
            bool? unread = null,
            string? type = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Plant)
                .Where(n => n.UserId == userId)
                .AsQueryable();

            if (unread.HasValue)
            {
                if (unread.Value)
                    query = query.Where(n => n.ReadAt == null);
                else
                    query = query.Where(n => n.ReadAt != null);
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                var lowered = type.ToLower();
                query = query.Where(n => n.Type.ToLower() == lowered);
            }

            if (startDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(n => n.CreatedAt <= endDate.Value);
            }

            var list = await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return list.Select(MapToDTO);
        }

        public async Task<NotificationResponseDTO?> GetByIdAsync(int id, string userId)
        {
            var n = await _context.Notifications
                .Include(x => x.User)
                .Include(x => x.Plant)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            return n == null ? null : MapToDTO(n);
        }

        public async Task<NotificationResponseDTO> CreateAsync(CreateNotificationDTO model, string currentUserId, bool isAdmin)
        {
            string targetUserId = currentUserId;
            if (isAdmin && !string.IsNullOrWhiteSpace(model.TargetUserId))
            {
                // Admin pode enviar para outro usuário
                targetUserId = model.TargetUserId!;
                // Verifica se o usuário destino existe
                var exists = await _context.Users.AnyAsync(u => u.Id == targetUserId);
                if (!exists)
                {
                    throw new ArgumentException("Usuário de destino não encontrado.");
                }
            }

            if (model.PlantId.HasValue)
            {
                // Se planta informada, verificar se existe e pertence ao targetUser
                var plantExists = await _context.Plants.AnyAsync(p => p.Id == model.PlantId.Value && p.UserId == targetUserId);
                if (!plantExists)
                {
                    throw new ArgumentException("Planta não encontrada ou não pertence ao usuário de destino.");
                }
            }

            var notification = new Notification
            {
                Title = model.Title,
                Message = model.Message,
                Type = model.Type,
                LinkUrl = model.LinkUrl,
                PlantId = model.PlantId,
                UserId = targetUserId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Enviar via canal configurado (in-app: sem efeito externo)
            await _sender.SendAsync(notification);
            notification.Status = "Sent";
            notification.SentAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var full = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Plant)
                .FirstAsync(n => n.Id == notification.Id);

            return MapToDTO(full);
        }

        public async Task<NotificationResponseDTO?> UpdateAsync(int id, UpdateNotificationDTO model, string userId, bool isAdmin)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notification == null)
                return null;

            if (!isAdmin && notification.UserId != userId)
                return null; // não permitido

            if (model.PlantId.HasValue)
            {
                var plantExists = await _context.Plants.AnyAsync(p => p.Id == model.PlantId.Value && p.UserId == notification.UserId);
                if (!plantExists)
                {
                    throw new ArgumentException("Planta não encontrada ou não pertence ao usuário.");
                }
            }

            notification.Title = model.Title;
            notification.Message = model.Message;
            notification.Type = model.Type;
            notification.LinkUrl = model.LinkUrl;
            notification.PlantId = model.PlantId;

            await _context.SaveChangesAsync();

            var full = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Plant)
                .FirstAsync(n => n.Id == notification.Id);

            return MapToDTO(full);
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var notification = await _context.Notifications.FirstOrDefaultAsync(n => n.Id == id);
            if (notification == null)
                return false;

            if (!isAdmin && notification.UserId != userId)
                return false;

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<NotificationResponseDTO?> MarkAsReadAsync(int id, string userId, bool read = true)
        {
            var notification = await _context.Notifications
                .Include(n => n.User)
                .Include(n => n.Plant)
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null)
                return null;

            notification.ReadAt = read ? DateTime.UtcNow : (DateTime?)null;
            notification.Status = read ? "Read" : (notification.SentAt.HasValue ? "Sent" : "Pending");
            await _context.SaveChangesAsync();
            return MapToDTO(notification);
        }

        public async Task<int> MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId && n.ReadAt == null)
                .ToListAsync();

            var now = DateTime.UtcNow;
            foreach (var n in notifications)
            {
                n.ReadAt = now;
                n.Status = "Read";
            }

            await _context.SaveChangesAsync();
            return notifications.Count;
        }

        private static NotificationResponseDTO MapToDTO(Notification n)
        {
            return new NotificationResponseDTO
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Type = n.Type,
                Status = n.Status,
                CreatedAt = n.CreatedAt,
                SentAt = n.SentAt,
                ReadAt = n.ReadAt,
                UserId = n.UserId,
                UserName = n.User?.UserName,
                PlantId = n.PlantId,
                PlantScientificName = n.Plant?.ScientificName,
                PlantCommonName = n.Plant?.CommonName,
                LinkUrl = n.LinkUrl
            };
        }
    }
}
