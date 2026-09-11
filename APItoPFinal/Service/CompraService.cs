using APItoPFinal.Data;
using APItoPFinal.Models;
using APItoPFinal.Repository;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Service
{
    public class CompraService
    {
        private readonly CompraRepository _repository;
        private readonly InstrumentosRepository _IRepository;
        
        public CompraService(CompraRepository repository, InstrumentosRepository IRepository)
        {
            _repository = repository;
            _IRepository = IRepository;
        }

        public List<Compra> GetCompras()
        {
            return _repository.PuxarCompras().Result.ToList();
        }
        public Compra? GetComprasById(Guid id)
        {
            return _repository.PuxarCompras().Result.FirstOrDefault(c => c.Id == id);
        }
        public void AdicionarCompra(Compra compra)
        {
            var instrumento = _IRepository.GetInstrumentosById(compra.InstrumentoId);
            if(instrumento == null)
            {
                throw new Exception("Instrumento não encontrado.");
            }
            if (!instrumento.Disponibilidade)
            {
                throw new Exception("Este instrumento já foi comprado.");
            }
            
            instrumento.Disponibilidade = false;
            
            _IRepository.AtualizarInstrumento(instrumento.Id, instrumento);

            _repository.PostCompras(compra);
        }

        public void AtualizarCompra(Compra compra)
        {
            var compraExiste = _repository.PuxarCompras().Result.FirstOrDefault(c => c.Id == compra.Id);
            if (compraExiste == null)
            {
                throw new Exception("Compra não encontrada.");
            }
            var instrumento = _IRepository.GetInstrumentosById(compra.InstrumentoId);
            if (instrumento != null)
            {
                if (instrumento.Disponibilidade == false && instrumento.Id != compraExiste.InstrumentoId)
                {
                    throw new Exception("Instrumento indisponível para compra.");
                }
            }
            else
            {
                throw new Exception("Instrumento não encontrado.");
            }

            compraExiste.Nome = compra.Nome;
            compraExiste.CPF = compra.CPF;

            _repository.PutCompras(compra.Id, compra);
        }

        public void DeletarCompra(Guid id)
        {
            var compra = _repository.PuxarCompras().Result.FirstOrDefault(c => c.Id == id);
            if (compra == null)
            {
                throw new Exception("Compra não encontrada.");
            }
            var instrumento = _IRepository.GetInstrumentosById(compra.InstrumentoId);
            if (instrumento != null)
            {
                instrumento.Disponibilidade = true;
                _IRepository.AtualizarInstrumento(instrumento.Id, instrumento);
            }

            _repository.DeleteCompras(id);
        }

    }
}
