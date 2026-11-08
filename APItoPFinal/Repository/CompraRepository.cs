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
            return await _context.Compras.Include(c => c.Itens).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<CompraDTO?> PuxarComprasById(Guid id)
        {
            return await _context.Compras
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CompraDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    CPF = c.CPF,
                    DataCompra = c.DataCompra,
                    UsuarioId = c.UsuarioId,
                    ValorTotal = c.ValorTotal,
                    Status = c.Status,
                    DataCancelamento = c.DataCancelamento,
                    Itens = c.Itens.Select(ci => new CompraItemDTO
                    {
                        InstrumentoId = ci.InstrumentoId,
                        Identificação = ci.Instrumento!.Identificação,
                        Nome = ci.Instrumento.Nome,
                        Quantidade = ci.Quantidade,
                        PrecoUnitario = ci.PrecoUnitario
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        // Cria a compra e dá baixa no estoque, tudo ou nada.
        // Devolve null se deu certo, ou o Id do instrumento que não tinha estoque suficiente.
        public async Task<Guid?> CriarComBaixaDeEstoqueAsync(Compra compra)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync();

            // ordem fixa por Id: duas compras simultâneas nunca travam uma à outra esperando em ordens opostas
            foreach (var item in compra.Itens.OrderBy(i => i.InstrumentoId))
            {
                var instrumentoId = item.InstrumentoId;
                var quantidade = item.Quantidade;

                // o próprio banco decide: só desconta se ainda houver estoque suficiente
                var linhas = await _context.Instrumentos
                    .Where(i => i.Id == instrumentoId && i.Estoque >= quantidade)
                    .ExecuteUpdateAsync(s => s.SetProperty(i => i.Estoque, i => i.Estoque - quantidade));

                if (linhas == 0)
                {
                    await transacao.RollbackAsync();
                    return instrumentoId;
                }
            }

            _context.Compras.Add(compra);
            await _context.SaveChangesAsync();

            await transacao.CommitAsync();
            return null;
        }

        // Cancela e devolve o estoque, tudo ou nada.
        // Devolve false se a compra já não estava ativa (por exemplo, dois cancelamentos ao mesmo tempo).
        public async Task<bool> CancelarComEstornoAsync(Guid compraId, IEnumerable<(Guid InstrumentoId, int Quantidade)> itens)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync();

            var agora = DateTime.UtcNow;

            // só muda se ainda estiver ativa: quem chegar depois recebe 0 linhas e não estorna de novo
            var linhas = await _context.Compras
                .Where(c => c.Id == compraId && c.Status == StatusCompra.Ativa)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(c => c.Status, StatusCompra.Cancelada)
                    .SetProperty(c => c.DataCancelamento, (DateTime?)agora));

            if (linhas == 0)
            {
                await transacao.RollbackAsync();
                return false;
            }

            foreach (var (instrumentoId, quantidade) in itens.OrderBy(i => i.InstrumentoId))
            {
                await _context.Instrumentos
                    .Where(i => i.Id == instrumentoId)
                    .ExecuteUpdateAsync(s => s.SetProperty(i => i.Estoque, i => i.Estoque + quantidade));
            }

            await transacao.CommitAsync();
            return true;
        }

        public async Task<Compra?> DeleteCompras(Guid id)
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