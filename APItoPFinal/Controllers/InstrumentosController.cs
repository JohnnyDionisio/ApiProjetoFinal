using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APItoPFinal.Data;
using APItoPFinal.Models;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstrumentosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InstrumentosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Instrumento>>> GetInstrumentos()
        {
            return await _context.Instrumentos.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Instrumento>> GetInstrumentosById(Guid id)
        {
            var instrumento = await _context.Instrumentos.FindAsync(id);

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
            _context.Instrumentos.Add(instrumento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInstrumentosById", new { id = instrumento.Id }, instrumento);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutInstrumento(Guid id, Instrumento instrumento)
        {
            if (id != instrumento.Id)
            {
                return BadRequest();
            }

            _context.Entry(instrumento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var instrumentoTemp = _context.Instrumentos.Any(e => e.Id == id);
                if (!instrumentoTemp)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInstrumento(Guid id)
        {
            var instrumento = await _context.Instrumentos.FindAsync(id);
            if (instrumento == null)
            {
                return NotFound();
            }

            _context.Instrumentos.Remove(instrumento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
