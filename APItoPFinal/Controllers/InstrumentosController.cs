using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Service;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InstrumentoDTO>>> GetInstrumentos()
        {
            return await _service.GetInstrumentos();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InstrumentoDTO>> GetInstrumentosById(Guid id)
        {
            var instrumento = await _service.GetInstrumentoDTOById(id);

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
            await _service.AdicionarInstrumento(instrumento);
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
                await _service.AtualizarInstrumento(instrumento);
            }
            catch (KeyNotFoundException)
            {
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
            var instrumento = await _service.GetInstrumentosById(id);
            if (instrumento == null)
            {
                return NotFound();
            }

            await _service.DeletarInstrumento(id);
            return NoContent();
        }
    }
}