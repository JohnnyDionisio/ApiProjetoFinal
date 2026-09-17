using APItoPFinal.Models;
using APItoPFinal.Repository;

namespace APItoPFinal.Service
{
    public class MarcaService
    {
        private readonly MarcaRepository _repository;

        public MarcaService(MarcaRepository repository)
        {
            _repository = repository;
        }

        public List<Marca> GetMarcas()
        {
            return _repository.GetMarcas();
        }
        public Marca? GetMarcaById(Guid id)
        {
            return _repository.GetMarcaById(id);
        }
        public void AdicionarMarca(Marca marca)
        {
            var marcaExiste = _repository.GetMarcaById(marca.Id);
            if (marcaExiste != null)
            {
                throw new Exception("Marca já existe.");
            }

            marca.Id = Guid.NewGuid();
            _repository.AdicionarMarca(marca);
        }
        public void AtualizarMarca(Marca marca)
        {
            var marcaExiste = _repository.GetMarcaById(marca.Id);
            if(marcaExiste == null)
            {
                throw new Exception("Marca não encontrada.");
            }
            marcaExiste.Nome = marca.Nome;
            _repository.AtualizarMarca(marca.Id, marcaExiste);
        }
        public void DeletarMarca(Guid id)
        {
            var marcaExiste = _repository.GetMarcaById(id);
            if(marcaExiste == null)
            {
                throw new Exception("Marca não encontrada.");
            }
            _repository.DeletarMarca(id);
        }
    }
}
