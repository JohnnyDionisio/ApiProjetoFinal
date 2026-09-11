using APItoPFinal.Data;
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

        public List<Instrumento> GetInstrumentos()
        {
            return _repository.GetInstrumentos();
        }

        public Instrumento? GetInstrumentosById(Guid id)
        {
            return _repository.GetInstrumentosById(id);
        }

        public void AdicionarInstrumento(Instrumento instrumento)
        {
            var instrumentoExiste = _repository.GetInstrumentosById(instrumento.Id);
            if(instrumentoExiste != null)
                {
                    throw new Exception("Instrumento já existe.");
                }
            var identificacao = _repository.GetInstrumentoByIdentity(instrumento.Identificação);
            if(identificacao != null)
            {
                throw new Exception("Instrumento já cadastrado com essa identificação.");
            }

            instrumento.Id = Guid.NewGuid();
            instrumento.Disponibilidade = true;

            _repository.AdicionarInstrumento(instrumento);
        }
        public void AtualizarInstrumento(Instrumento instrumento)
        {
            var instrumentoExiste = _repository.GetInstrumentosById(instrumento.Id);
            if (instrumentoExiste == null)
            {
                throw new Exception("Instrumento não encontrado.");
            }
            var identity = _repository.GetInstrumentoByIdentity(instrumento.Identificação);
            if(identity != null && identity.Id != instrumento.Id)
            {
                throw new Exception("Instrumento já cadastrado com essa identificação.");
            }

            instrumentoExiste.Identificação = instrumento.Identificação;
            instrumentoExiste.Nome = instrumento.Nome;
            instrumentoExiste.Preco = instrumento.Preco;
            instrumentoExiste.Tipo = instrumento.Tipo;
            instrumentoExiste.Marca = instrumento.Marca;
            instrumentoExiste.Descricao = instrumento.Descricao;
            instrumentoExiste.Disponibilidade = instrumento.Disponibilidade;

            _repository.AtualizarInstrumento(instrumento.Id, instrumentoExiste);
        }

        public void DeletarInstrumento(Guid id)
        {
            var instrumentoExiste = _repository.GetInstrumentosById(id);
            if (instrumentoExiste == null)
            {
                throw new Exception("Instrumento não encontrado.");
            }
            _repository.DeletarInstrumento(id);
        }

    }
}
