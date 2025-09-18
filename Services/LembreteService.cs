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
    /// Serviço para gerenciamento de lembretes
    /// </summary>
    public class LembreteService : ILembreteService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Construtor do serviço de lembretes
        /// </summary>
        /// <param name="context">Contexto do banco de dados</param>
        public LembreteService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtém todos os lembretes de um usuário
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesByUserIdAsync(string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém um lembrete pelo ID
        /// </summary>
        public async Task<LembreteResponseDTO?> GetLembreteByIdAsync(int id, string userId)
        {
            var lembrete = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            return lembrete != null ? MapToResponseDTO(lembrete) : null;
        }

        /// <summary>
        /// Obtém lembretes por categoria
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesByCategoriaAsync(string categoria, string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && l.Categoria.ToLower().Contains(categoria.ToLower()))
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes por prioridade
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesByPrioridadeAsync(int prioridade, string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && l.Prioridade == prioridade)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes pendentes (não concluídos)
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesPendentesAsync(string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && !l.Concluido)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes concluídos
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesConcluidosAsync(string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && l.Concluido)
                .OrderByDescending(l => l.AtualizadoEm ?? l.CriadoEm)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes atrasados
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesAtrasadosAsync(string userId)
        {
            var agora = DateTime.UtcNow;
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && !l.Concluido && l.DataLembrete < agora)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes de hoje
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesHojeAsync(string userId)
        {
            var hoje = DateTime.UtcNow.Date;
            var amanha = hoje.AddDays(1);

            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && l.DataLembrete >= hoje && l.DataLembrete < amanha)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes da próxima semana
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesProximaSemanaAsync(string userId)
        {
            var hoje = DateTime.UtcNow.Date;
            var proximaSemana = hoje.AddDays(7);

            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && l.DataLembrete >= hoje && l.DataLembrete <= proximaSemana)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém lembretes relacionados a uma planta específica
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> GetLembretesByPlantIdAsync(int plantId, string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && l.PlantId == plantId)
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Cria um novo lembrete
        /// </summary>
        public async Task<LembreteResponseDTO> CreateLembreteAsync(CreateLembreteDTO model, string userId)
        {
            // Verificar se a planta existe e pertence ao usuário (se PlantId foi fornecido)
            if (model.PlantId.HasValue)
            {
                var plantExists = await _context.Plants
                    .AnyAsync(p => p.Id == model.PlantId.Value && p.UserId == userId);

                if (!plantExists)
                {
                    throw new ArgumentException("Planta não encontrada ou não pertence ao usuário.");
                }
            }

            var lembrete = new Lembrete
            {
                Titulo = model.Titulo,
                Descricao = model.Descricao,
                DataLembrete = model.DataLembrete,
                Prioridade = model.Prioridade,
                Categoria = model.Categoria,
                PlantId = model.PlantId,
                UserId = userId,
                CriadoEm = DateTime.UtcNow
            };

            _context.Lembretes.Add(lembrete);
            await _context.SaveChangesAsync();

            // Recarregar com includes
            var lembreteCompleto = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .FirstAsync(l => l.Id == lembrete.Id);

            return MapToResponseDTO(lembreteCompleto);
        }

        /// <summary>
        /// Atualiza um lembrete existente
        /// </summary>
        public async Task<LembreteResponseDTO?> UpdateLembreteAsync(int id, UpdateLembreteDTO model, string userId)
        {
            var lembrete = await _context.Lembretes
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (lembrete == null)
            {
                return null;
            }

            // Verificar se a planta existe e pertence ao usuário (se PlantId foi fornecido)
            if (model.PlantId.HasValue)
            {
                var plantExists = await _context.Plants
                    .AnyAsync(p => p.Id == model.PlantId.Value && p.UserId == userId);

                if (!plantExists)
                {
                    throw new ArgumentException("Planta não encontrada ou não pertence ao usuário.");
                }
            }

            lembrete.Titulo = model.Titulo;
            lembrete.Descricao = model.Descricao;
            lembrete.DataLembrete = model.DataLembrete;
            lembrete.Concluido = model.Concluido;
            lembrete.Prioridade = model.Prioridade;
            lembrete.Categoria = model.Categoria;
            lembrete.PlantId = model.PlantId;
            lembrete.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Recarregar com includes
            var lembreteCompleto = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .FirstAsync(l => l.Id == lembrete.Id);

            return MapToResponseDTO(lembreteCompleto);
        }

        /// <summary>
        /// Marca um lembrete como concluído ou não concluído
        /// </summary>
        public async Task<LembreteResponseDTO?> ConcluirLembreteAsync(int id, ConcluirLembreteDTO model, string userId)
        {
            var lembrete = await _context.Lembretes
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (lembrete == null)
            {
                return null;
            }

            lembrete.Concluido = model.Concluido;
            lembrete.AtualizadoEm = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Recarregar com includes
            var lembreteCompleto = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .FirstAsync(l => l.Id == lembrete.Id);

            return MapToResponseDTO(lembreteCompleto);
        }

        /// <summary>
        /// Exclui um lembrete
        /// </summary>
        public async Task<bool> DeleteLembreteAsync(int id, string userId)
        {
            var lembrete = await _context.Lembretes
                .FirstOrDefaultAsync(l => l.Id == id && l.UserId == userId);

            if (lembrete == null)
            {
                return false;
            }

            _context.Lembretes.Remove(lembrete);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Busca lembretes por título ou descrição
        /// </summary>
        public async Task<IEnumerable<LembreteResponseDTO>> SearchLembretesAsync(string searchTerm, string userId)
        {
            var lembretes = await _context.Lembretes
                .Include(l => l.User)
                .Include(l => l.Plant)
                .Where(l => l.UserId == userId && 
                           (l.Titulo.ToLower().Contains(searchTerm.ToLower()) ||
                            l.Descricao.ToLower().Contains(searchTerm.ToLower()) ||
                            l.Categoria.ToLower().Contains(searchTerm.ToLower())))
                .OrderBy(l => l.DataLembrete)
                .ToListAsync();

            return lembretes.Select(MapToResponseDTO);
        }

        /// <summary>
        /// Obtém estatísticas dos lembretes do usuário
        /// </summary>
        public async Task<object> GetLembretesStatisticsAsync(string userId)
        {
            var agora = DateTime.UtcNow;
            var hoje = agora.Date;

            var total = await _context.Lembretes.CountAsync(l => l.UserId == userId);
            var concluidos = await _context.Lembretes.CountAsync(l => l.UserId == userId && l.Concluido);
            var pendentes = await _context.Lembretes.CountAsync(l => l.UserId == userId && !l.Concluido);
            var atrasados = await _context.Lembretes.CountAsync(l => l.UserId == userId && !l.Concluido && l.DataLembrete < agora);
            var hoje_count = await _context.Lembretes.CountAsync(l => l.UserId == userId && l.DataLembrete.Date == hoje);
            var alta_prioridade = await _context.Lembretes.CountAsync(l => l.UserId == userId && !l.Concluido && l.Prioridade == 3);

            return new
            {
                Total = total,
                Concluidos = concluidos,
                Pendentes = pendentes,
                Atrasados = atrasados,
                Hoje = hoje_count,
                AltaPrioridade = alta_prioridade,
                PercentualConclusao = total > 0 ? Math.Round((double)concluidos / total * 100, 2) : 0
            };
        }

        /// <summary>
        /// Mapeia um lembrete para DTO de resposta
        /// </summary>
        private static LembreteResponseDTO MapToResponseDTO(Lembrete lembrete)
        {
            return new LembreteResponseDTO
            {
                Id = lembrete.Id,
                Titulo = lembrete.Titulo,
                Descricao = lembrete.Descricao,
                DataLembrete = lembrete.DataLembrete,
                Concluido = lembrete.Concluido,
                Prioridade = lembrete.Prioridade,
                Categoria = lembrete.Categoria,
                CriadoEm = lembrete.CriadoEm,
                AtualizadoEm = lembrete.AtualizadoEm,
                UserId = lembrete.UserId,
                UserName = lembrete.User?.UserName ?? string.Empty,
                PlantId = lembrete.PlantId,
                PlantScientificName = lembrete.Plant?.ScientificName,
                PlantCommonName = lembrete.Plant?.CommonName
            };
        }
    }
}
