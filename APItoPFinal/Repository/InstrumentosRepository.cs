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
            if (f.Disponivel.HasValue) query = query.Where(i => i.Disponibilidade == f.Disponivel.Value);

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
                    Disponibilidade = i.Disponibilidade,
                    Categoria = i.Categoria.Nome,
                    Marca = i.Marca.Nome
                })
                .ToListAsync();

            return (itens, total);
        }

        // Usado só internamente (validações, update, delete) - entidade completa
        public async Task<Instrumento> GetByIdAsync(Guid id)
        {
            return await _context.Instrumentos
                .Include(i => i.Categoria)
                .Include(i => i.Marca)
                .FirstOrDefaultAsync(i => i.Id == id);
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
                    Disponibilidade = i.Disponibilidade,
                    Categoria = i.Categoria.Nome,
                    Marca = i.Marca.Nome
                })
                .FirstOrDefaultAsync();
        }

        public Instrumento? GetInstrumentoByIdentity(string identificacao)
        {
            return _context.Instrumentos.FirstOrDefault(i => i.Identificação == identificacao);
        }

        public bool EstaEmCompra(Guid id)
        {
            return _context.Compras.Any(c => c.Instrumentos.Any(i => i.Id == id));
        }

        public void AdicionarInstrumento(Instrumento instrumento)
        {
            _context.Instrumentos.Add(instrumento);
            _context.SaveChanges();
        }

        public void AtualizarInstrumento(Guid id, Instrumento instrumento)
        {
            _context.Instrumentos.Update(instrumento);
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