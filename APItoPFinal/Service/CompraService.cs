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

        public async Task<List<Compra>> GetCompras()
        {
            var compras = await _repository.PuxarCompras();
            return compras.ToList();
        }

        public async Task<Compra?> GetComprasById(Guid id)
        {
            var compras = await _repository.PuxarCompras();
            return compras.FirstOrDefault(c => c.Id == id);
        }

        public async Task AdicionarCompra(Compra compra)
        {
            var instrumento = await _IRepository.GetByIdAsync(compra.InstrumentoId);
            if (instrumento == null)
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

        public async Task AtualizarCompra(Compra compra)
        {
            var compras = await _repository.PuxarCompras();
            var compraExiste = compras.FirstOrDefault(c => c.Id == compra.Id);
            if (compraExiste == null)
            {
                throw new Exception("Compra não encontrada.");
            }
            var instrumento = await _IRepository.GetByIdAsync(compra.InstrumentoId);
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

        public async Task DeletarCompra(Guid id)
        {
            var compras = await _repository.PuxarCompras();
            var compra = compras.FirstOrDefault(c => c.Id == id);
            if (compra == null)
            {
                throw new Exception("Compra não encontrada.");
            }
            var instrumento = await _IRepository.GetByIdAsync(compra.InstrumentoId);
            if (instrumento != null)
            {
                instrumento.Disponibilidade = true;
                _IRepository.AtualizarInstrumento(instrumento.Id, instrumento);
            }

            _repository.DeleteCompras(id);
        }
    }
}