using APItoPFinal.Models;
using APItoPFinal.Repository;

namespace APItoPFinal.Service
{
    public class CategoriaService
    {
        private readonly CategoriasRepository _repository;

        public CategoriaService(CategoriasRepository repository)
        {
            _repository = repository;
        }
        public List<Categoria> GetCategorias()
        {
            return _repository.GetCategorias();
        }
        public Categoria? GetCategoriaById(Guid id)
        {
            return _repository.GetCategoriaById(id);
        }
        public void AdicionarCategoria(Categoria categoria)
        {
            var categoriaExiste = _repository.GetCategoriaById(categoria.Id);
            if (categoriaExiste != null)
            {
                throw new Exception("Categoria já existe.");
            }
            categoria.Id = Guid.NewGuid();
            _repository.AdicionarCategoria(categoria);
        }
        public void AtualizarCategoria(Categoria categoria)
        {
            var categoriaExiste = _repository.GetCategoriaById(categoria.Id);
            if (categoriaExiste == null)
            {
                throw new Exception("Categoria não encontrada.");
            }
            categoriaExiste.Nome = categoria.Nome;
            _repository.AtualizarCategoria(categoria.Id, categoriaExiste);
        }
        public void DeletarCategoria(Guid id)
        {
            var categoriaExiste = _repository.GetCategoriaById(id);
            if (categoriaExiste == null)
            {
                throw new Exception("Categoria não encontrada.");
            }
            _repository.DeletarCategoria(id);
        }
    }
}
