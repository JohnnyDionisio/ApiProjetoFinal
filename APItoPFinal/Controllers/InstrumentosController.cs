using System.Security.Claims;
using APItoPFinal.DTOs;
using APItoPFinal.Exceptions;
using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InstrumentosController : ControllerBase
    {
        private readonly InstrumentoService _service;

        public InstrumentosController(InstrumentoService service)
        {
            _service = service;
        }

        private Guid GetUsuarioLogadoId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<ActionResult<PaginadoDTO<InstrumentoDTO>>> GetInstrumentos([FromQuery] InstrumentoFiltroDTO filtro)
        {
            return await _service.GetInstrumentos(filtro);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstrumentoDTO>> GetInstrumentosById(Guid id)
        {
            var instrumento = await _service.GetInstrumentoDTOById(id);
            if (instrumento == null)
            {
                throw new NotFoundException("Instrumento não encontrado.");
            }
            return instrumento;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Instrumento>> PostInstrumento(Instrumento instrumento)
        {
            instrumento.Id = Guid.NewGuid();
            await _service.AdicionarInstrumento(instrumento, GetUsuarioLogadoId());
            return CreatedAtAction("GetInstrumentosById", new { id = instrumento.Id }, instrumento);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstrumento(Guid id, Instrumento instrumento)
        {
            if (id != instrumento.Id)
            {
                throw new BadRequestException("O ID da URL não confere com o objeto.");
            }

            await _service.AtualizarInstrumento(instrumento);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInstrumento(Guid id)
        {
            await _service.DeletarInstrumento(id);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{id}/estoque")]
        public async Task<ActionResult<MovimentoEstoqueDTO>> AjustarEstoque(Guid id, AjusteEstoqueDTO dto)
        {
            var movimento = await _service.AjustarEstoque(id, dto, GetUsuarioLogadoId());
            return Ok(movimento);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}/estoque/movimentos")]
        public async Task<ActionResult<PaginadoDTO<MovimentoEstoqueDTO>>> GetMovimentos(Guid id, [FromQuery] MovimentoFiltroDTO filtro)
        {
            return await _service.GetMovimentos(id, filtro);
        }
    }
}