using APItoPFinal.Data;
using APItoPFinal.DTOs;
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
        public async Task<Compra?> PuxarCompraEntityById(Guid id)
        {
            return await _context.Compras.Include(c => c.Instrumentos).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<CompraDTO> PuxarComprasById(Guid id)
        {
            return await _context.Compras.Include(c => c.Instrumentos).Where(c => c.Id == id).Select(c => new CompraDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                CPF = c.CPF,
                DataCompra = c.DataCompra,
                UsuarioId = c.UsuarioId,
                Instrumentos = c.Instrumentos.Select(i => new InstrumentoResumoDTO
                {
                    Id = i.Id,
                    Identificação = i.Identificação,
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Descricao = i.Descricao,
                    Disponibilidade = i.Disponibilidade
                }).ToList()
            }).FirstOrDefaultAsync();
        }
        public async Task<Compra> PostCompras(Compra compra)
        {
            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            return compra;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
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