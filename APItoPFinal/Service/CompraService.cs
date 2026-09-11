using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Service
{
    public class CompraService(AppDbContext context)
    {
        public async Task<IEnumerable<Compra>> GetCompras()
        {
            return await context.Compras.ToListAsync();
        }
        public async Task<Compra> GetComprasById(Guid id)
        {
            return await context.Compras.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Compra> PostCompras(Compra compra)
        {
            context.Compras.Add(compra);
            await context.SaveChangesAsync();

            return compra;
        }
        public async Task<Compra> PutCompras(Guid id, Compra compra)
        {
            context.Entry(compra).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var compraTemp = context.Compras.Any(c => c.Id == id);
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
            var compra = await context.Compras.FindAsync(id);
            if (compra == null)
            {
                return null;
            }

            context.Compras.Remove(compra);
            await context.SaveChangesAsync();

            return compra;
        }
    }
}
