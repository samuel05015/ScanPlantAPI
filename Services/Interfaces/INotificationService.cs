using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScanPlantAPI.Models.DTOs;

namespace ScanPlantAPI.Services.Interfaces
{
    /// <summary>
    /// Interface do serviço de notificações
    /// </summary>
    public interface INotificationService
    {
        Task<IEnumerable<NotificationResponseDTO>> GetNotificationsAsync(
            string userId,
            bool? unread = null,
            string? type = null,
            DateTime? startDate = null,
            DateTime? endDate = null);

        Task<NotificationResponseDTO?> GetByIdAsync(int id, string userId);

        Task<NotificationResponseDTO> CreateAsync(CreateNotificationDTO model, string currentUserId, bool isAdmin);

        Task<NotificationResponseDTO?> UpdateAsync(int id, UpdateNotificationDTO model, string userId, bool isAdmin);

        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);

        Task<NotificationResponseDTO?> MarkAsReadAsync(int id, string userId, bool read = true);

        Task<int> MarkAllAsReadAsync(string userId);
    }
}
