using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Repository;

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

        public async Task<CompraDTO?> GetComprasDTOById(Guid id)
        {
            return await _repository.PuxarComprasById(id);
        }

        public async Task<Guid> AdicionarCompra(CompraInputDTO input)
        {
            if (input.InstrumentoIds == null || !input.InstrumentoIds.Any())
            {
                throw new Exception("É necessário informar ao menos um instrumento.");
            }

            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Nome = input.Nome,
                CPF = input.CPF,
                DataCompra = DateTime.Now,
                Instrumentos = new List<Instrumento>()
            };

            foreach (var instrumentoId in input.InstrumentoIds)
            {
                var instrumento = await _IRepository.GetByIdAsync(instrumentoId);
                if (instrumento == null)
                {
                    throw new Exception($"Instrumento {instrumentoId} não encontrado.");
                }
                if (!instrumento.Disponibilidade)
                {
                    throw new Exception($"O instrumento '{instrumento.Nome}' já foi comprado.");
                }

                instrumento.Disponibilidade = false;
                compra.Instrumentos.Add(instrumento);
            }

            await _repository.PostCompras(compra);
            return compra.Id;
        }

        public async Task AtualizarCompra(Guid id, CompraInputDTO input)
        {
            var compraExiste = await _repository.PuxarCompraEntityById(id);
            if (compraExiste == null)
            {
                throw new Exception("Compra não encontrada.");
            }

            if (input.InstrumentoIds == null || !input.InstrumentoIds.Any())
            {
                throw new Exception("É necessário informar ao menos um instrumento.");
            }

            // Libera os instrumentos que saíram da lista
            foreach (var instrumentoAntigo in compraExiste.Instrumentos.ToList())
            {
                if (!input.InstrumentoIds.Contains(instrumentoAntigo.Id))
                {
                    instrumentoAntigo.Disponibilidade = true;
                    compraExiste.Instrumentos.Remove(instrumentoAntigo);
                }
            }

            // Adiciona os novos instrumentos
            foreach (var instrumentoId in input.InstrumentoIds)
            {
                if (compraExiste.Instrumentos.Any(i => i.Id == instrumentoId))
                {
                    continue; // já está na compra, não faz nada
                }

                var instrumento = await _IRepository.GetByIdAsync(instrumentoId);
                if (instrumento == null)
                {
                    throw new Exception($"Instrumento {instrumentoId} não encontrado.");
                }
                if (!instrumento.Disponibilidade)
                {
                    throw new Exception($"O instrumento '{instrumento.Nome}' já foi comprado.");
                }

                instrumento.Disponibilidade = false;
                compraExiste.Instrumentos.Add(instrumento);
            }

            compraExiste.Nome = input.Nome;
            compraExiste.CPF = input.CPF;

            await _repository.SaveChangesAsync();
        }

        public async Task DeletarCompra(Guid id)
        {
            var compra = await _repository.PuxarCompraEntityById(id);
            if (compra == null)
            {
                throw new Exception("Compra não encontrada.");
            }

            foreach (var instrumento in compra.Instrumentos)
            {
                instrumento.Disponibilidade = true;
            }

            await _repository.DeleteCompras(id);
        }
    }
}