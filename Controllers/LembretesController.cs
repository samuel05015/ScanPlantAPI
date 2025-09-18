using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScanPlantAPI.Models.DTOs;
using ScanPlantAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScanPlantAPI.Controllers
{
    /// <summary>
    /// Controlador para operações com lembretes
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LembretesController : ControllerBase
    {
        private readonly ILembreteService _lembreteService;

        /// <summary>
        /// Construtor do controlador de lembretes
        /// </summary>
        /// <param name="lembreteService">Serviço de lembretes</param>
        public LembretesController(ILembreteService lembreteService)
        {
            _lembreteService = lembreteService;
        }

        /// <summary>
        /// Obtém todos os lembretes do usuário atual
        /// </summary>
        /// <returns>Lista de lembretes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetAllLembretes()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesByUserIdAsync(userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém um lembrete pelo ID
        /// </summary>
        /// <param name="id">ID do lembrete</param>
        /// <returns>Dados do lembrete</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LembreteResponseDTO), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLembreteById(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembrete = await _lembreteService.GetLembreteByIdAsync(id, userId);
            if (lembrete == null)
            {
                return NotFound();
            }

            return Ok(lembrete);
        }

        /// <summary>
        /// Obtém lembretes por categoria
        /// </summary>
        /// <param name="categoria">Nome da categoria</param>
        /// <returns>Lista de lembretes da categoria</returns>
        [HttpGet("categoria/{categoria}")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesByCategoria(string categoria)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesByCategoriaAsync(categoria, userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes por prioridade
        /// </summary>
        /// <param name="prioridade">Prioridade (1-Baixa, 2-Média, 3-Alta)</param>
        /// <returns>Lista de lembretes da prioridade especificada</returns>
        [HttpGet("prioridade/{prioridade}")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesByPrioridade(int prioridade)
        {
            if (prioridade < 1 || prioridade > 3)
            {
                return BadRequest("A prioridade deve ser entre 1 (Baixa) e 3 (Alta)");
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesByPrioridadeAsync(prioridade, userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes pendentes (não concluídos)
        /// </summary>
        /// <returns>Lista de lembretes pendentes</returns>
        [HttpGet("pendentes")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesPendentes()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesPendentesAsync(userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes concluídos
        /// </summary>
        /// <returns>Lista de lembretes concluídos</returns>
        [HttpGet("concluidos")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesConcluidos()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesConcluidosAsync(userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes atrasados
        /// </summary>
        /// <returns>Lista de lembretes atrasados</returns>
        [HttpGet("atrasados")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesAtrasados()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesAtrasadosAsync(userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes de hoje
        /// </summary>
        /// <returns>Lista de lembretes de hoje</returns>
        [HttpGet("hoje")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesHoje()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesHojeAsync(userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes da próxima semana
        /// </summary>
        /// <returns>Lista de lembretes da próxima semana</returns>
        [HttpGet("proxima-semana")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesProximaSemana()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesProximaSemanaAsync(userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém lembretes relacionados a uma planta específica
        /// </summary>
        /// <param name="plantId">ID da planta</param>
        /// <returns>Lista de lembretes da planta</returns>
        [HttpGet("planta/{plantId}")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesByPlantId(int plantId)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.GetLembretesByPlantIdAsync(plantId, userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Busca lembretes por título, descrição ou categoria
        /// </summary>
        /// <param name="searchTerm">Termo de busca</param>
        /// <returns>Lista de lembretes encontrados</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(IEnumerable<LembreteResponseDTO>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> SearchLembretes([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Termo de busca é obrigatório");
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembretes = await _lembreteService.SearchLembretesAsync(searchTerm, userId);
            return Ok(lembretes);
        }

        /// <summary>
        /// Obtém estatísticas dos lembretes do usuário
        /// </summary>
        /// <returns>Estatísticas dos lembretes</returns>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetLembretesStatistics()
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var statistics = await _lembreteService.GetLembretesStatisticsAsync(userId);
            return Ok(statistics);
        }

        /// <summary>
        /// Cria um novo lembrete
        /// </summary>
        /// <param name="model">Dados do lembrete</param>
        /// <returns>Dados do lembrete criado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(LembreteResponseDTO), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateLembrete([FromBody] CreateLembreteDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var lembrete = await _lembreteService.CreateLembreteAsync(model, userId);
                return CreatedAtAction(nameof(GetLembreteById), new { id = lembrete.Id }, lembrete);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Atualiza um lembrete existente
        /// </summary>
        /// <param name="id">ID do lembrete</param>
        /// <param name="model">Novos dados do lembrete</param>
        /// <returns>Dados do lembrete atualizado</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(LembreteResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateLembrete(int id, [FromBody] UpdateLembreteDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var lembrete = await _lembreteService.UpdateLembreteAsync(id, model, userId);
                if (lembrete == null)
                {
                    return NotFound();
                }

                return Ok(lembrete);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Marca um lembrete como concluído ou não concluído
        /// </summary>
        /// <param name="id">ID do lembrete</param>
        /// <param name="model">Status de conclusão</param>
        /// <returns>Dados do lembrete atualizado</returns>
        [HttpPatch("{id}/concluir")]
        [ProducesResponseType(typeof(LembreteResponseDTO), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConcluirLembrete(int id, [FromBody] ConcluirLembreteDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var lembrete = await _lembreteService.ConcluirLembreteAsync(id, model, userId);
            if (lembrete == null)
            {
                return NotFound();
            }

            return Ok(lembrete);
        }

        /// <summary>
        /// Exclui um lembrete
        /// </summary>
        /// <param name="id">ID do lembrete</param>
        /// <returns>Mensagem de sucesso</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteLembrete(int id)
        {
            var userId = GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _lembreteService.DeleteLembreteAsync(id, userId);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        /// <summary>
        /// Obtém o ID do usuário atual
        /// </summary>
        /// <returns>ID do usuário</returns>
        private string? GetCurrentUserId()
        {
            return User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
        }
    }
}
