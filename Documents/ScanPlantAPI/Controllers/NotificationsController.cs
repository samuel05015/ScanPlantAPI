using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPlantAPI.Models.DTOs;
using ScanPlantAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ScanPlantAPI.Controllers
{
    /// <summary>
    /// Controlador para operações com notificações
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lista notificações do usuário com filtros opcionais
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetNotifications(
            [FromQuery] bool? unread,
            [FromQuery] string? type,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var list = await _notificationService.GetNotificationsAsync(userId, unread, type, startDate, endDate);
            return Ok(list);
        }

        /// <summary>
        /// Lista apenas notificações não lidas do usuário
        /// </summary>
        [HttpGet("unread")]
        [ProducesResponseType(typeof(IEnumerable<NotificationResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetUnreadNotifications()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var list = await _notificationService.GetNotificationsAsync(userId, unread: true);
            return Ok(list);
        }

        /// <summary>
        /// Obtém uma notificação por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificationResponseDTO), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var n = await _notificationService.GetByIdAsync(id, userId);
            if (n == null) return NotFound();
            return Ok(n);
        }

        /// <summary>
        /// Cria e envia uma nova notificação (admins podem enviar para outros usuários)
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(NotificationResponseDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = IsAdmin();

            try
            {
                var created = await _notificationService.CreateAsync(model, userId, isAdmin);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza uma notificação (admin ou dono)
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NotificationResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateNotificationDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = IsAdmin();

            try
            {
                var updated = await _notificationService.UpdateAsync(id, model, userId, isAdmin);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Marca uma notificação como lida/não lida
        /// </summary>
        [HttpPatch("{id}/read")]
        [ProducesResponseType(typeof(NotificationResponseDTO), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> MarkAsRead(int id, [FromBody] MarkAsReadDTO model)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var updated = await _notificationService.MarkAsReadAsync(id, userId, model.Read);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        /// <summary>
        /// Marca todas as notificações do usuário como lidas
        /// </summary>
        [HttpPatch("read-all")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var count = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { Updated = count });
        }

        /// <summary>
        /// Exclui uma notificação (admin ou dono)
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var isAdmin = IsAdmin();
            var ok = await _notificationService.DeleteAsync(id, userId, isAdmin);
            if (!ok) return NotFound();
            return NoContent();
        }

        private string? GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }

        private bool IsAdmin()
        {
            // Considera roles padrão do Identity
            if (User.IsInRole("Admin")) return true;

            // Ou claim de role genérica
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value);
            return roles.Any(r => string.Equals(r, "Admin", StringComparison.OrdinalIgnoreCase));
        }
    }
}
