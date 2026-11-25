using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Repository
{
    public class CarrinhoRepository
    {
        private readonly AppDbContext _context;

        public CarrinhoRepository(AppDbContext context)
        {
            _context = context;
        }

        // itens do carrinho com os dados atuais do instrumento (preço e estoque), sem rastreamento
        public async Task<List<CarrinhoItem>> GetItensAsync(Guid usuarioId)
        {
            return await _context.CarrinhoItens
                .AsNoTracking()
                .Include(c => c.Instrumento)
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.DataAdicao)
                .ThenBy(c => c.InstrumentoId)
                .ToListAsync();
        }

        // rastreado: o Service altera a quantidade e chama SalvarAsync
        public async Task<CarrinhoItem?> GetItemAsync(Guid usuarioId, Guid instrumentoId)
        {
            return await _context.CarrinhoItens
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.InstrumentoId == instrumentoId);
        }

        public async Task AdicionarAsync(CarrinhoItem item)
        {
            _context.CarrinhoItens.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task SalvarAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<bool> RemoverItemAsync(Guid usuarioId, Guid instrumentoId)
        {
            var linhas = await _context.CarrinhoItens
                .Where(c => c.UsuarioId == usuarioId && c.InstrumentoId == instrumentoId)
                .ExecuteDeleteAsync();

            return linhas > 0;
        }

        public async Task EsvaziarAsync(Guid usuarioId)
        {
            await _context.CarrinhoItens
                .Where(c => c.UsuarioId == usuarioId)
                .ExecuteDeleteAsync();
        }
    }
}