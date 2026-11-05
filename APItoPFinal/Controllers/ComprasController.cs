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
    public class ComprasController : ControllerBase
    {
        private readonly CompraService _service;

        public ComprasController(CompraService service)
        {
            _service = service;
        }

        private Guid GetUsuarioLogadoId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        private bool IsAdmin() => User.IsInRole("Admin");

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            return Ok(await _service.GetCompras(GetUsuarioLogadoId(), IsAdmin()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CompraDTO>> GetComprasById(Guid id)
        {
            var compra = await _service.GetComprasDTOById(id, GetUsuarioLogadoId(), IsAdmin());
            if (compra == null)
            {
                throw new NotFoundException("Compra não encontrada.");
            }
            return Ok(compra);
        }

        [HttpPost]
        public async Task<ActionResult> PostCompras(CompraInputDTO input)
        {
            var usuarioId = GetUsuarioLogadoId();
            var id = await _service.AdicionarCompra(input, usuarioId);
            var compraCriada = await _service.GetComprasDTOById(id, usuarioId, IsAdmin());
            return Created($"/api/compras/{id}", compraCriada);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> PutCompras(Guid id, CompraInputDTO input)
        {
            await _service.AtualizarCompra(id, input);
            return NoContent();
        }

        [HttpPost("{id}/cancelar")]
        public async Task<IActionResult> CancelarCompra(Guid id)
        {
            await _service.CancelarCompra(id, GetUsuarioLogadoId(), IsAdmin());
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCompras(Guid id)
        {
            await _service.DeletarCompra(id);
            return NoContent();
        }
    }
}