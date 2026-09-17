using APItoPFinal.Data;
using APItoPFinal.Models;

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
        public Categoria? GetCategoriaById(Guid id)
        {
            return _context.Categorias.FirstOrDefault(c => c.Id == id);
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
