using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Repository;
using APItoPFinal.Exceptions;
using APItoPFinal.Helpers;
using Microsoft.EntityFrameworkCore;

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

        // NOVO
        public async Task<CategoriaDTO?> GetCategoriaDTOById(Guid id)
        {
            return await _repository.GetCategoriaDTOByIdAsync(id);
        }

        public void AdicionarCategoria(Categoria categoria)
        {
            var categoriaExiste = _repository.GetCategoriaById(categoria.Id);
            if (categoriaExiste != null)
            {
                throw new ConflictException("Categoria já existe.");
            }
            categoria.Id = Guid.NewGuid();
            _repository.AdicionarCategoria(categoria);
        }
        public void AtualizarCategoria(Categoria categoria)
        {
            var categoriaExiste = _repository.GetCategoriaById(categoria.Id);
            if (categoriaExiste == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }
            categoriaExiste.Nome = categoria.Nome;
            _repository.AtualizarCategoria(categoria.Id, categoriaExiste);
        }
        public void DeletarCategoria(Guid id)
        {
            var categoriaExiste = _repository.GetCategoriaById(id);
            if (categoriaExiste == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }
            if (_repository.PossuiInstrumento(id))
            {
                throw new ConflictException("Não é possível excluir: essa categoria possui instrumentos cadastrados");
            }
            try
            {
                _repository.DeletarCategoria(id);
            }
            catch (DbUpdateException ex) when (DbErros.ViolacaoDeFk(ex)) 
            {
                throw new ConflictException("Não é possível excluir: esta categoria ainda está em uso.");
            }
        }
    }
}