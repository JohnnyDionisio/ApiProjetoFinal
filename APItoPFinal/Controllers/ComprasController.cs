using APItoPFinal.DTOs;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            return Ok(await _service.GetCompras());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetComprasById(Guid id)
        {
            var compra = await _service.GetComprasDTOById(id);

            if (compra == null)
            {
                return NotFound("Compra não encontrada.");
            }

            return Ok(compra);
        }

        [HttpPost]
        public async Task<ActionResult> PostCompras(CompraInputDTO input)
        {
            try
            {
                var id = await _service.AdicionarCompra(input);
                var compraCriada = await _service.GetComprasDTOById(id);
                return CreatedAtAction(nameof(GetComprasById), new { id }, compraCriada);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

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