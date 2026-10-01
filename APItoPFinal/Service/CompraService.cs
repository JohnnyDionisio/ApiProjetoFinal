using APItoPFinal.DTOs;
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

            if(compra == null)
            {
                return null;
            }

            if(!isAdmin && compra.UsuarioId != usuarioId)
            {
                throw new UnauthorizedAccessException("Acesso negado. Você não tem permissão para acessar esta compra.");
            }
            return compra;
        }

        public async Task<Guid> AdicionarCompra(CompraInputDTO input, Guid usuarioId)
        {
            if (input.InstrumentoIds == null || !input.InstrumentoIds.Any())
            {
                throw new Exception("É necessário informar ao menos um instrumento.");
            }

            var usuario = await _URepository.GetUsuariosById(usuarioId);
            if(usuario == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Nome = usuario.Nome,       // vem do usuário logado, não do body
                CPF = usuario.CPF,         // vem do usuário logado, não do body
                UsuarioId = usuario.Id,
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