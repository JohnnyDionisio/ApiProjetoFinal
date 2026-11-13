using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Repository
{
    public class InstrumentosRepository
    {
        private readonly AppDbContext _context;

        public InstrumentosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<InstrumentoDTO> Itens, int Total)> BuscarAsync(InstrumentoFiltroDTO f)
        {
            var query = _context.Instrumentos.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(f.Busca))
            {
                var termo = f.Busca.Trim();
                query = query.Where(i => i.Nome.Contains(termo)
                                      || i.Descricao.Contains(termo)
                                      || i.Identificação.Contains(termo));
            }

            if (f.CategoriaId.HasValue) query = query.Where(i => i.IdCategoria == f.CategoriaId.Value);
            if (f.MarcaId.HasValue) query = query.Where(i => i.IdMarca == f.MarcaId.Value);
            if (f.PrecoMin.HasValue) query = query.Where(i => i.Preco >= f.PrecoMin.Value);
            if (f.PrecoMax.HasValue) query = query.Where(i => i.Preco <= f.PrecoMax.Value);

            if (f.Disponivel.HasValue)
            {
                query = f.Disponivel.Value
                    ? query.Where(i => i.Estoque > 0)
                    : query.Where(i => i.Estoque == 0);
            }

            var total = await query.CountAsync();

            var campo = (f.OrdenarPor ?? "nome").ToLowerInvariant().Replace('ç', 'c');

            IOrderedQueryable<Instrumento> ordenada = (campo, f.Decrescente) switch
            {
                ("preco", false) => query.OrderBy(i => i.Preco),
                ("preco", true) => query.OrderByDescending(i => i.Preco),
                (_, true) => query.OrderByDescending(i => i.Nome),
                _ => query.OrderBy(i => i.Nome)
            };

            // desempate por Id: sem isso, itens com o mesmo nome/preço podem repetir ou sumir entre páginas
            ordenada = ordenada.ThenBy(i => i.Id);

            var itens = await ordenada
                .Skip((f.Pagina - 1) * f.Tamanho)
                .Take(f.Tamanho)
                .Select(i => new InstrumentoDTO
                {
                    Id = i.Id,
                    Identificação = i.Identificação,
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Descricao = i.Descricao,
                    Estoque = i.Estoque,
                    Categoria = i.Categoria.Nome,
                    Marca = i.Marca.Nome
                })
                .ToListAsync();

            return (itens, total);
        }

        // Usado só internamente (validações, update, delete) - entidade completa, rastreada
        public async Task<Instrumento> GetByIdAsync(Guid id)
        {
            return await _context.Instrumentos
                .Include(i => i.Categoria)
                .Include(i => i.Marca)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Vários de uma vez, sem rastreamento: usado ao montar uma compra (nome, preço, estoque atual)
        public async Task<List<Instrumento>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            var lista = ids.ToList();
            return await _context.Instrumentos
                .AsNoTracking()
                .Where(i => lista.Contains(i.Id))
                .ToListAsync();
        }

        // Usado pelo Controller no GET por id - já devolve o DTO
        public async Task<InstrumentoDTO?> GetByIdDTOAsync(Guid id)
        {
            return await _context.Instrumentos
                .Include(i => i.Categoria)
                .Include(i => i.Marca)
                .Where(i => i.Id == id)
                .Select(i => new InstrumentoDTO
                {
                    Id = i.Id,
                    Identificação = i.Identificação,
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Descricao = i.Descricao,
                    Estoque = i.Estoque,
                    Categoria = i.Categoria.Nome,
                    Marca = i.Marca.Nome
                })
                .FirstOrDefaultAsync();
        }

        public Instrumento? GetInstrumentoByIdentity(string identificacao)
        {
            return _context.Instrumentos.FirstOrDefault(i => i.Identificação == identificacao);
        }

        // qualquer compra, inclusive cancelada, conta: é o histórico do instrumento
        public bool EstaEmCompra(Guid id)
        {
            return _context.CompraItens.Any(ci => ci.InstrumentoId == id);
        }

        public void AdicionarInstrumento(Instrumento instrumento, Guid usuarioId)
        {
            _context.Instrumentos.Add(instrumento);

            // estoque informado no cadastro: registra como saldo inicial (mesmo SaveChanges, então tudo ou nada)
            if (instrumento.Estoque > 0)
            {
                _context.MovimentosEstoque.Add(new MovimentoEstoque
                {
                    Id = Guid.NewGuid(),
                    InstrumentoId = instrumento.Id,
                    Tipo = TipoMovimentoEstoque.SaldoInicial,
                    Variacao = instrumento.Estoque,
                    EstoqueApos = instrumento.Estoque,
                    Motivo = "Estoque inicial do cadastro",
                    UsuarioId = usuarioId,
                    DataHora = DateTime.UtcNow
                });
            }

            _context.SaveChanges();
        }
        public async Task<bool> ExisteAsync(Guid id)
        {
            return await _context.Instrumentos.AnyAsync(i => i.Id == id);
        }

        // Aplica a variação e registra o movimento, tudo ou nada.
        // Devolve null se a variação deixaria o estoque negativo.
        public async Task<MovimentoEstoque?> AjustarEstoqueAsync(
            Guid instrumentoId, TipoMovimentoEstoque tipo, int variacao, string motivo, Guid usuarioId)
        {
            await using var transacao = await _context.Database.BeginTransactionAsync();

            // o próprio banco decide: só aplica se o resultado não for negativo
            var linhas = await _context.Instrumentos
                .Where(i => i.Id == instrumentoId && i.Estoque + variacao >= 0)
                .ExecuteUpdateAsync(s => s.SetProperty(i => i.Estoque, i => i.Estoque + variacao));

            if (linhas == 0)
            {
                await transacao.RollbackAsync();
                return null;
            }

            var estoqueApos = await _context.Instrumentos
                .AsNoTracking()
                .Where(i => i.Id == instrumentoId)
                .Select(i => i.Estoque)
                .FirstAsync();

            var movimento = new MovimentoEstoque
            {
                Id = Guid.NewGuid(),
                InstrumentoId = instrumentoId,
                Tipo = tipo,
                Variacao = variacao,
                EstoqueApos = estoqueApos,
                Motivo = motivo,
                UsuarioId = usuarioId,
                DataHora = DateTime.UtcNow
            };

            _context.MovimentosEstoque.Add(movimento);
            await _context.SaveChangesAsync();

            await transacao.CommitAsync();
            return movimento;
        }

        public async Task<(List<MovimentoEstoqueDTO> Itens, int Total)> BuscarMovimentosAsync(Guid instrumentoId, MovimentoFiltroDTO f)
        {
            var query = _context.MovimentosEstoque
                .AsNoTracking()
                .Where(m => m.InstrumentoId == instrumentoId);

            if (f.Tipo.HasValue)
            {
                query = query.Where(m => m.Tipo == f.Tipo.Value);
            }

            var total = await query.CountAsync();

            var itens = await query
                .OrderByDescending(m => m.DataHora)
                .ThenByDescending(m => m.Id)
                .Skip((f.Pagina - 1) * f.Tamanho)
                .Take(f.Tamanho)
                .Select(m => new MovimentoEstoqueDTO
                {
                    Id = m.Id,
                    InstrumentoId = m.InstrumentoId,
                    Tipo = m.Tipo,
                    Variacao = m.Variacao,
                    EstoqueApos = m.EstoqueApos,
                    Motivo = m.Motivo,
                    CompraId = m.CompraId,
                    UsuarioId = m.UsuarioId,
                    DataHora = m.DataHora
                })
                .ToListAsync();

            return (itens, total);
        }

        // O instrumento chega rastreado (veio do GetByIdAsync) com os campos já alterados.
        // Só o SaveChanges basta: assim o EF não reescreve colunas que não mudaram,
        // e o estoque nunca é sobrescrito por um valor velho.
        public void AtualizarInstrumento(Guid id, Instrumento instrumento)
        {
            _context.SaveChanges();
        }

        public void DeletarInstrumento(Guid id)
        {
            var instrumento = _context.Instrumentos.FirstOrDefault(i => i.Id == id);
            if (instrumento != null)
            {
                _context.Instrumentos.Remove(instrumento);
                _context.SaveChanges();
            }
        }
    }
}