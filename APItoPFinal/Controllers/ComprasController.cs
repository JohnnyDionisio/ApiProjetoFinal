using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Mvc;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly CompraService _service;

        public ComprasController(CompraService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Compra>> GetCompras()
        {
            return Ok(_service.GetCompras());
        }

        [HttpGet("{id}")]
        public ActionResult<Compra> GetComprasById(Guid id)
        {
            var compra = _service.GetComprasById(id);

            if (compra == null)
            {
                return NotFound("Compra não encontrada.");
            }

            return Ok(compra);
        }

        [HttpPost]
        public ActionResult<Compra> PostCompras(Compra compra)
        {
            try
            {
                compra.Id = Guid.NewGuid();
                _service.AdicionarCompra(compra);

                return CreatedAtAction(nameof(GetComprasById), new { id = compra.Id }, compra);
            }
            catch (Exception ex)
            {
                // Pega a mensagem lançada no CompraService ("Instrumento não encontrado" ou "já foi comprado")
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult PutCompras(Guid id, Compra compra)
        {
            if (id != compra.Id)
            {
                return BadRequest("O ID informado não confere com o objeto enviado.");
            }

            try
            {
                _service.AtualizarCompra(compra);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteCompras(Guid id)
        {
            try
            {
                _service.DeletarCompra(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}