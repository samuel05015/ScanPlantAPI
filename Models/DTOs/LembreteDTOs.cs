using System;
using System.ComponentModel.DataAnnotations;

namespace ScanPlantAPI.Models.DTOs
{
    /// <summary>
    /// DTO para criação de lembrete
    /// </summary>
    public class CreateLembreteDTO
    {
        /// <summary>
        /// Título do lembrete
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do lembrete
        /// </summary>
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora do lembrete
        /// </summary>
        [Required(ErrorMessage = "A data do lembrete é obrigatória")]
        public DateTime DataLembrete { get; set; }

        /// <summary>
        /// Prioridade do lembrete (1-Baixa, 2-Média, 3-Alta)
        /// </summary>
        [Range(1, 3, ErrorMessage = "A prioridade deve ser entre 1 (Baixa) e 3 (Alta)")]
        public int Prioridade { get; set; } = 2;

        /// <summary>
        /// Categoria do lembrete
        /// </summary>
        [MaxLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres")]
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// ID da planta relacionada (opcional)
        /// </summary>
        public int? PlantId { get; set; }
    }

    /// <summary>
    /// DTO para atualização de lembrete
    /// </summary>
    public class UpdateLembreteDTO
    {
        /// <summary>
        /// Título do lembrete
        /// </summary>
        [Required(ErrorMessage = "O título é obrigatório")]
        [MaxLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres")]
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do lembrete
        /// </summary>
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora do lembrete
        /// </summary>
        [Required(ErrorMessage = "A data do lembrete é obrigatória")]
        public DateTime DataLembrete { get; set; }

        /// <summary>
        /// Indica se o lembrete foi concluído
        /// </summary>
        public bool Concluido { get; set; }

        /// <summary>
        /// Prioridade do lembrete (1-Baixa, 2-Média, 3-Alta)
        /// </summary>
        [Range(1, 3, ErrorMessage = "A prioridade deve ser entre 1 (Baixa) e 3 (Alta)")]
        public int Prioridade { get; set; }

        /// <summary>
        /// Categoria do lembrete
        /// </summary>
        [MaxLength(50, ErrorMessage = "A categoria deve ter no máximo 50 caracteres")]
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// ID da planta relacionada (opcional)
        /// </summary>
        public int? PlantId { get; set; }
    }

    /// <summary>
    /// DTO para resposta de lembrete
    /// </summary>
    public class LembreteResponseDTO
    {
        /// <summary>
        /// ID do lembrete
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Título do lembrete
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descrição detalhada do lembrete
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora do lembrete
        /// </summary>
        public DateTime DataLembrete { get; set; }

        /// <summary>
        /// Indica se o lembrete foi concluído
        /// </summary>
        public bool Concluido { get; set; }

        /// <summary>
        /// Prioridade do lembrete (1-Baixa, 2-Média, 3-Alta)
        /// </summary>
        public int Prioridade { get; set; }

        /// <summary>
        /// Nome da prioridade
        /// </summary>
        public string PrioridadeNome => Prioridade switch
        {
            1 => "Baixa",
            2 => "Média",
            3 => "Alta",
            _ => "Desconhecida"
        };

        /// <summary>
        /// Categoria do lembrete
        /// </summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora em que o lembrete foi criado
        /// </summary>
        public DateTime CriadoEm { get; set; }

        /// <summary>
        /// Data e hora da última atualização
        /// </summary>
        public DateTime? AtualizadoEm { get; set; }

        /// <summary>
        /// ID do usuário
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Nome do usuário
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// ID da planta relacionada (opcional)
        /// </summary>
        public int? PlantId { get; set; }

        /// <summary>
        /// Nome científico da planta relacionada (opcional)
        /// </summary>
        public string? PlantScientificName { get; set; }

        /// <summary>
        /// Nome comum da planta relacionada (opcional)
        /// </summary>
        public string? PlantCommonName { get; set; }

        /// <summary>
        /// Indica se o lembrete está atrasado
        /// </summary>
        public bool Atrasado => !Concluido && DataLembrete < DateTime.UtcNow;

        /// <summary>
        /// Dias restantes até o lembrete (negativo se atrasado)
        /// </summary>
        public int DiasRestantes => (int)(DataLembrete.Date - DateTime.UtcNow.Date).TotalDays;
    }

    /// <summary>
    /// DTO para marcar lembrete como concluído
    /// </summary>
    public class ConcluirLembreteDTO
    {
        /// <summary>
        /// Indica se o lembrete foi concluído
        /// </summary>
        public bool Concluido { get; set; }
    }
}
