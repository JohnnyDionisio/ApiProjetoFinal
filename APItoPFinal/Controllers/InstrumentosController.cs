using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APItoPFinal.Data;
using APItoPFinal.Models;
using APItoPFinal.Service;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentosController : ControllerBase
    {
        private readonly InstrumentoService _service;

        public InstrumentosController(InstrumentoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instrumento>>> GetInstrumentos()
        {
            return _service.GetInstrumentos();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Instrumento>> GetInstrumentosById(Guid id)
        {
            var instrumento =  _service.GetInstrumentosById(id);

            if (instrumento == null)
            {
                return NotFound();
            }

            return instrumento;
        }
        [HttpPost]
        public async Task<ActionResult<Instrumento>> PostInstrumento(Instrumento instrumento)
        {
            instrumento.Id = Guid.NewGuid();
            _service.AdicionarInstrumento(instrumento);
            return CreatedAtAction("GetInstrumentosById", new { id = instrumento.Id }, instrumento);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutInstrumento(Guid id, Instrumento instrumento)
        {
            if (id != instrumento.Id)
            {
                return BadRequest();
            }

            try
            {
                // 1. O await executa a atualização e o salvamento interno no Service
                await _service.AtualizarInstrumento(instrumento);
            }
            catch (KeyNotFoundException)
            {
                // Caso o instrumento não exista no banco
                return NotFound();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInstrumento(Guid id)
        {
            var instrumento = _service.GetInstrumentosById(id);
            if (instrumento == null)
            {
                return NotFound();
            }

            _service.DeletarInstrumento(id);
            return NoContent();
        }
    }
}
