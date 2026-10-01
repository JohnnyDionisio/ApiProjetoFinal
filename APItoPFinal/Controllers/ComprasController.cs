using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComprasController : ControllerBase
    {
        private readonly CompraService _service;

        public ComprasController(CompraService service)
        {
            _service = service;
        }

        private Guid GetUsuarioLogadoId()
        {
            var idTexto = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(idTexto);
        }
        
        private bool IsAdmin() => User.IsInRole("Admin");

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            var compras = await _service.GetCompras(GetUsuarioLogadoId(), IsAdmin());
            return Ok(compras);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetComprasById(Guid id)
        {
            try
            {
                var compra = await _service.GetComprasDTOById(id, GetUsuarioLogadoId(), IsAdmin());
                if(compra == null)
                {
                    return NotFound("Compra não encontrada.");
                }
                return Ok(compra);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid();
            }
        }

        [HttpPost]
        public async Task<ActionResult> PostCompras(CompraInputDTO input)
        {
            try
            {
            var id = await _service.AdicionarCompra(input, GetUsuarioLogadoId());
            var compraCriada = await _service.GetComprasDTOById(id, GetUsuarioLogadoId(), IsAdmin());
            return CreatedAtAction(nameof(GetComprasById), new { id = compraCriada.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCompras(Guid id, CompraInputDTO input)
        {
            try
            {
                await _service.AtualizarCompra(id, input);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCompras(Guid id)
        {
            try
            {
                await _service.DeletarCompra(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}