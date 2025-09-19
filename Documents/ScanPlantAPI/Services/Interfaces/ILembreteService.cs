using ScanPlantAPI.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanPlantAPI.Services.Interfaces
{
    /// <summary>
    /// Interface para o serviço de lembretes
    /// </summary>
    public interface ILembreteService
    {
        /// <summary>
        /// Obtém todos os lembretes de um usuário
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesByUserIdAsync(string userId);

        /// <summary>
        /// Obtém um lembrete pelo ID
        /// </summary>
        Task<LembreteResponseDTO?> GetLembreteByIdAsync(int id, string userId);

        /// <summary>
        /// Obtém lembretes por categoria
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesByCategoriaAsync(string categoria, string userId);

        /// <summary>
        /// Obtém lembretes por prioridade
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesByPrioridadeAsync(int prioridade, string userId);

        /// <summary>
        /// Obtém lembretes pendentes (não concluídos)
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesPendentesAsync(string userId);

        /// <summary>
        /// Obtém lembretes concluídos
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesConcluidosAsync(string userId);

        /// <summary>
        /// Obtém lembretes atrasados
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesAtrasadosAsync(string userId);

        /// <summary>
        /// Obtém lembretes de hoje
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesHojeAsync(string userId);

        /// <summary>
        /// Obtém lembretes da próxima semana
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesProximaSemanaAsync(string userId);

        /// <summary>
        /// Obtém lembretes relacionados a uma planta específica
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> GetLembretesByPlantIdAsync(int plantId, string userId);

        /// <summary>
        /// Cria um novo lembrete
        /// </summary>
        Task<LembreteResponseDTO> CreateLembreteAsync(CreateLembreteDTO model, string userId);

        /// <summary>
        /// Atualiza um lembrete existente
        /// </summary>
        Task<LembreteResponseDTO?> UpdateLembreteAsync(int id, UpdateLembreteDTO model, string userId);

        /// <summary>
        /// Marca um lembrete como concluído ou não concluído
        /// </summary>
        Task<LembreteResponseDTO?> ConcluirLembreteAsync(int id, ConcluirLembreteDTO model, string userId);

        /// <summary>
        /// Exclui um lembrete
        /// </summary>
        Task<bool> DeleteLembreteAsync(int id, string userId);

        /// <summary>
        /// Busca lembretes por título ou descrição
        /// </summary>
        Task<IEnumerable<LembreteResponseDTO>> SearchLembretesAsync(string searchTerm, string userId);

        /// <summary>
        /// Obtém estatísticas dos lembretes do usuário
        /// </summary>
        Task<object> GetLembretesStatisticsAsync(string userId);
    }
}
