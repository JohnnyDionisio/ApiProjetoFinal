using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Repository
{
    public class CompraRepository
    {
        private readonly AppDbContext _context;

        public CompraRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Compra>> PuxarCompras()
        {
            return await _context.Compras.ToListAsync();
        }
        public async Task<Compra> PostCompras(Compra compra)
        {
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return compra;
        }
        public async Task<Compra> PutCompras(Guid id, Compra compra)
        {
            _context.Entry(compra).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var compraTemp = _context.Compras.Any(c => c.Id == id);
                if (!compraTemp)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return compra;
        }
        public async Task<Compra> DeleteCompras(Guid id)
        {
            var compra = await _context.Compras.FindAsync(id);
            if (compra == null)
            {
                return null;
            }

            _context.Compras.Remove(compra);
            await _context.SaveChangesAsync();

            return compra;
        }
    }
}