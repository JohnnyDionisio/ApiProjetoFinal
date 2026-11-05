using APItoPFinal.DTOs;
using APItoPFinal.Exceptions;
using APItoPFinal.Models;
using APItoPFinal.Repository;

namespace APItoPFinal.Service
{
    public class CompraService
    {
        private readonly CompraRepository _repository;
        private readonly InstrumentosRepository _IRepository;
        private readonly UsuariosRepository _URepository;

        public CompraService(CompraRepository repository, InstrumentosRepository IRepository, UsuariosRepository URepository)
        {
            _repository = repository;
            _IRepository = IRepository;
            _URepository = URepository;
        }

        public async Task<List<Compra>> GetCompras(Guid usuarioId, bool isAdmin)
        {
            var compras = await _repository.PuxarCompras();
            if (!isAdmin)
            {
                compras = compras.Where(c => c.UsuarioId == usuarioId).ToList();
            }

            return compras.ToList();
        }

        public async Task<CompraDTO?> GetComprasDTOById(Guid id, Guid usuarioId, bool isAdmin)
        {
            var compra = await _repository.PuxarComprasById(id);

            if (compra == null)
            {
                return null;
            }

            if (!isAdmin && compra.UsuarioId != usuarioId)
            {
                throw new UnauthorizedAccessException("Acesso negado. Você não tem permissão para acessar esta compra.");
            }
            return compra;
        }

        public async Task<Guid> AdicionarCompra(CompraInputDTO input, Guid usuarioId)
        {
            if (input.InstrumentoIds == null || !input.InstrumentoIds.Any())
            {
                throw new BadRequestException("É necessário informar ao menos um instrumento.");
            }

            var usuario = await _URepository.GetUsuariosById(usuarioId);
            if (usuario == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Nome = usuario.Nome,       // vem do usuário logado, não do body
                CPF = usuario.CPF,         // vem do usuário logado, não do body
                UsuarioId = usuario.Id,
                DataCompra = DateTime.UtcNow,
                Status = StatusCompra.Ativa,
                Instrumentos = new List<Instrumento>()
            };

            foreach (var instrumentoId in input.InstrumentoIds.Distinct())
            {
                var instrumento = await _IRepository.GetByIdAsync(instrumentoId);
                if (instrumento == null)
                {
                    throw new NotFoundException($"Instrumento {instrumentoId} não encontrado.");
                }
                if (!instrumento.Disponibilidade)
                {
                    throw new ConflictException($"O instrumento '{instrumento.Nome}' já foi comprado.");
                }

                instrumento.Disponibilidade = false;
                compra.Instrumentos.Add(instrumento);
            }

            // grava o valor cobrado na época da compra
            compra.ValorTotal = compra.Instrumentos.Sum(i => i.Preco);

            await _repository.PostCompras(compra);
            return compra.Id;
        }

        public async Task AtualizarCompra(Guid id, CompraInputDTO input)
        {
            var compraExiste = await _repository.PuxarCompraEntityById(id);
            if (compraExiste == null)
            {
                throw new NotFoundException("Compra não encontrada.");
            }

            if (compraExiste.Status != StatusCompra.Ativa)
            {
                throw new ConflictException("Só é possível alterar compras ativas.");
            }

            if (input.InstrumentoIds == null || !input.InstrumentoIds.Any())
            {
                throw new BadRequestException("É necessário informar ao menos um instrumento.");
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
                    throw new NotFoundException($"Instrumento {instrumentoId} não encontrado.");
                }
                if (!instrumento.Disponibilidade)
                {
                    throw new ConflictException($"O instrumento '{instrumento.Nome}' já foi comprado.");
                }

                instrumento.Disponibilidade = false;
                compraExiste.Instrumentos.Add(instrumento);
            }

            // correção feita por Admin: o total é recalculado com os preços atuais
            compraExiste.ValorTotal = compraExiste.Instrumentos.Sum(i => i.Preco);

            await _repository.SaveChangesAsync();
        }

        public async Task CancelarCompra(Guid id, Guid usuarioId, bool isAdmin)
        {
            var compra = await _repository.PuxarCompraEntityById(id);
            if (compra == null)
            {
                throw new NotFoundException("Compra não encontrada.");
            }

            if (!isAdmin && compra.UsuarioId != usuarioId)
            {
                throw new UnauthorizedAccessException("Acesso negado. Você não tem permissão para cancelar a compra de outro usuário.");
            }

            if (compra.Status == StatusCompra.Cancelada)
            {
                throw new ConflictException("Esta compra já foi cancelada.");
            }

            foreach (var instrumento in compra.Instrumentos)
            {
                instrumento.Disponibilidade = true;
            }

            compra.Status = StatusCompra.Cancelada;
            compra.DataCancelamento = DateTime.UtcNow;

            await _repository.SaveChangesAsync();
        }

        public async Task DeletarCompra(Guid id)
        {
            var compra = await _repository.PuxarCompraEntityById(id);
            if (compra == null)
            {
                throw new NotFoundException("Compra não encontrada.");
            }

            // Numa compra cancelada os instrumentos já foram liberados e podem ter
            // sido comprados de novo: só libera se a compra ainda estava ativa.
            if (compra.Status == StatusCompra.Ativa)
            {
                foreach (var instrumento in compra.Instrumentos)
                {
                    instrumento.Disponibilidade = true;
                }
            }

            await _repository.DeleteCompras(id);
        }
    }
}   