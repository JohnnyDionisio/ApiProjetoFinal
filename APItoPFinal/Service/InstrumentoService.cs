using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Service
{
    public class InstrumentoService(AppDbContext context) 
    {
        public async Task<IEnumerable<Instrumento>> GetInstrumentos()
        {
            return await context.Instrumentos.Include(i => i.Compras).ToListAsync();
        }
        public async Task<Instrumento> GetInstrumentosById(Guid id)
        {
            return await context.Instrumentos.FirstOrDefaultAsync(i => i.Id == id);
        }
        public async Task<Instrumento> PostInstrumentos(Instrumento instrumento)
        {
            context.Instrumentos.Add(instrumento);
            await context.SaveChangesAsync();

            return instrumento;
        }
        public async Task<Instrumento> PutInstrumentos(Guid id, Instrumento instrumento)
        {
            context.Entry(instrumento).State = EntityState.Modified;
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                var instrumentoTemp = context.Instrumentos.Any(i => i.Id == id);
                if (!instrumentoTemp)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return instrumento;
        }
        public async Task<Instrumento> DeleteInstrumentos(Guid id)
        {
            var instrumento = await context.Instrumentos.FindAsync(id);
            if (instrumento == null)
            {
                return null;
            }

            context.Instrumentos.Remove(instrumento);
            await context.SaveChangesAsync();

            return instrumento;
        }
    }
}
