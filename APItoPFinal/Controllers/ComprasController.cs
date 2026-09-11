using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComprasController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Compra>>> GetCompras()
        {
            return await _context.Compras.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Compra>> GetComprasById(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);

            if (compra == null)
            {
                return NotFound();
            }

            return compra;
        }
        [HttpPost]
        public async Task<ActionResult<Compra>> PostCompras(Compra compra)
        {
            compra.Id = Guid.NewGuid();
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetComprasById", new { id = compra.Id }, compra);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> PutCompras(Guid id, Compra compra)
        {
            if (id != compra.Id)
            {
                return BadRequest();
            }

            _context.Entry(compra).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var compraTemp = _context.Compras.Any(e => e.Id == id);
                if (!compraTemp)
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
        public async Task<ActionResult> DeleteCompras(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return NotFound();
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    
}
}
