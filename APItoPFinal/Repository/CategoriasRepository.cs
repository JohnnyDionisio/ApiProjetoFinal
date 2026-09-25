using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Repository
{
    public class CategoriasRepository
    {
        private readonly AppDbContext _context;

        public CategoriasRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Categoria> GetCategorias()
        {
            return _context.Categorias.ToList();
        }

        // Uso interno (validações) - continua igual
        public Categoria? GetCategoriaById(Guid id)
        {
            return _context.Categorias.FirstOrDefault(c => c.Id == id);
        }

        // NOVO - usado pelo Controller no GET por id, já com os instrumentos
        public async Task<CategoriaDTO?> GetCategoriaDTOByIdAsync(Guid id)
        {
            return await _context.Categorias
                .Include(c => c.Instrumentos)
                .Where(c => c.Id == id)
                .Select(c => new CategoriaDTO
                {
                    Id = c.Id,
                    Nome = c.Nome,
                    Instrumentos = c.Instrumentos.Select(i => new InstrumentoResumoDTO
                    {
                        Id = i.Id,
                        Identificação = i.Identificação,
                        Nome = i.Nome,
                        Preco = i.Preco,
                        Descricao = i.Descricao,
                        Disponibilidade = i.Disponibilidade
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public void AdicionarCategoria(Categoria categoria)
        {
            _context.Categorias.Add(categoria);
            _context.SaveChanges();
        }
        public void AtualizarCategoria(Guid id, Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            _context.SaveChanges();
        }
        public void DeletarCategoria(Guid id)
        {
            var categoria = _context.Categorias.FirstOrDefault(c => c.Id == id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
                _context.SaveChanges();
            }
        }
    }
}