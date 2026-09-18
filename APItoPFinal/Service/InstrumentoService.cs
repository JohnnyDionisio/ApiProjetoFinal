using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Service
{
    public class InstrumentoService
    {
        private readonly InstrumentosRepository _repository;

        public InstrumentoService(InstrumentosRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<InstrumentoDTO>> GetInstrumentos()
        {
            return await _repository.GetAllAsync();
        }

        // Para o Controller (GET por id) - devolve DTO
        public async Task<InstrumentoDTO?> GetInstrumentoDTOById(Guid id)
        {
            return await _repository.GetByIdDTOAsync(id);
        }

        // Uso interno (validações) - devolve entidade completa
        public async Task<Instrumento?> GetInstrumentosById(Guid id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AdicionarInstrumento(Instrumento instrumento)
        {
            var instrumentoExiste = await _repository.GetByIdAsync(instrumento.Id);
            if (instrumentoExiste != null)
            {
                throw new Exception("Instrumento já existe.");
            }
            var identificacao = _repository.GetInstrumentoByIdentity(instrumento.Identificação);
            if (identificacao != null)
            {
                throw new Exception("Instrumento já cadastrado com essa identificação.");
            }

            instrumento.Id = Guid.NewGuid();
            instrumento.Disponibilidade = true;

            _repository.AdicionarInstrumento(instrumento);
        }

        public async Task AtualizarInstrumento(Instrumento instrumento)
        {
            var instrumentoExiste = await _repository.GetByIdAsync(instrumento.Id);
            if (instrumentoExiste == null)
            {
                throw new Exception("Instrumento não encontrado.");
            }
            var identity = _repository.GetInstrumentoByIdentity(instrumento.Identificação);
            if (identity != null && identity.Id != instrumento.Id)
            {
                throw new Exception("Instrumento já cadastrado com essa identificação.");
            }

            instrumentoExiste.Identificação = instrumento.Identificação;
            instrumentoExiste.Nome = instrumento.Nome;
            instrumentoExiste.Preco = instrumento.Preco;
            instrumentoExiste.Descricao = instrumento.Descricao;
            instrumentoExiste.Disponibilidade = instrumento.Disponibilidade;

            _repository.AtualizarInstrumento(instrumento.Id, instrumentoExiste);
        }

        public async Task DeletarInstrumento(Guid id)
        {
            var instrumentoExiste = await _repository.GetByIdAsync(id);
            if (instrumentoExiste == null)
            {
                throw new Exception("Instrumento não encontrado.");
            }
            _repository.DeletarInstrumento(id);
        }
    }
}