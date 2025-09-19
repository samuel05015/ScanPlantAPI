using System;
using System.ComponentModel.DataAnnotations;

namespace ScanPlantAPI.Models.DTOs
{
    public class CreateNotificationDTO
    {
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(60)]
        public string Type { get; set; } = "info"; // ex.: info, alerta, vencimento_rega

        [MaxLength(300)]
        public string? LinkUrl { get; set; }

        public int? PlantId { get; set; }

        // Apenas admins poderão enviar para outro usuário (UserId destino)
        public string? TargetUserId { get; set; }
    }

    public class UpdateNotificationDTO
    {
        [Required]
        [MaxLength(120)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(1000)]
        public string Message { get; set; } = string.Empty;

        [MaxLength(60)]
        public string Type { get; set; } = "info";

        [MaxLength(300)]
        public string? LinkUrl { get; set; }

        public int? PlantId { get; set; }
    }

    public class NotificationResponseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Type { get; set; } = "info";
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ReadAt { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public int? PlantId { get; set; }
        public string? PlantScientificName { get; set; }
        public string? PlantCommonName { get; set; }
        public string? LinkUrl { get; set; }
    }

    public class MarkAsReadDTO
    {
        public bool Read { get; set; } = true;
    }
}
