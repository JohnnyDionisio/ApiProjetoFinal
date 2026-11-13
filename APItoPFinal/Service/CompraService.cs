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
            if (input.Itens == null || !input.Itens.Any())
            {
                throw new BadRequestException("É necessário informar ao menos um instrumento.");
            }

            if (input.Itens.Any(i => i.Quantidade < 1))
            {
                throw new BadRequestException("A quantidade de cada item deve ser de pelo menos 1.");
            }

            var usuario = await _URepository.GetUsuariosById(usuarioId);
            if (usuario == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }

            // junta linhas repetidas do mesmo instrumento, somando as quantidades
            var pedidos = input.Itens
                .GroupBy(i => i.InstrumentoId)
                .Select(g => (InstrumentoId: g.Key, Quantidade: g.Sum(x => x.Quantidade)))
                .ToList();

            var instrumentos = (await _IRepository.GetByIdsAsync(pedidos.Select(p => p.InstrumentoId)))
                .ToDictionary(i => i.Id);

            var compra = new Compra
            {
                Id = Guid.NewGuid(),
                Nome = usuario.Nome,       // vem do usuário logado, não do body
                CPF = usuario.CPF,         // vem do usuário logado, não do body
                UsuarioId = usuario.Id,
                DataCompra = DateTime.UtcNow,
                Status = StatusCompra.Ativa,
                Itens = new List<CompraItem>()
            };

            foreach (var (instrumentoId, quantidade) in pedidos)
            {
                if (!instrumentos.TryGetValue(instrumentoId, out var instrumento))
                {
                    throw new NotFoundException($"Instrumento {instrumentoId} não encontrado.");
                }

                // checagem amigável, com mensagem clara; quem decide de verdade é o banco, logo abaixo
                if (instrumento.Estoque < quantidade)
                {
                    throw new ConflictException(instrumento.Estoque == 0
                        ? $"O instrumento '{instrumento.Nome}' está sem estoque."
                        : $"Estoque insuficiente para '{instrumento.Nome}': restam {instrumento.Estoque}, você pediu {quantidade}.");
                }

                compra.Itens.Add(new CompraItem
                {
                    CompraId = compra.Id,
                    InstrumentoId = instrumentoId,
                    Quantidade = quantidade,
                    PrecoUnitario = instrumento.Preco   // preço da época, gravado no item
                });
            }

            compra.ValorTotal = compra.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);

            var semEstoque = await _repository.CriarComBaixaDeEstoqueAsync(compra, usuarioId);
            if (semEstoque != null)
            {
                var nome = instrumentos[semEstoque.Value].Nome;
                throw new ConflictException($"O instrumento '{nome}' acabou de ficar sem estoque suficiente. Atualize e tente novamente.");
            }

            return compra.Id;
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

            var itens = compra.Itens.Select(i => (i.InstrumentoId, i.Quantidade)).ToList();

            var cancelada = await _repository.CancelarComEstornoAsync(id, itens, usuarioId, "Cancelamento de compra");
            if (!cancelada)
            {
                throw new ConflictException("Esta compra já foi cancelada.");
            }
        }

        public async Task DeletarCompra(Guid id, Guid usuarioId)
        {
            var compra = await _repository.PuxarCompraEntityById(id);
            if (compra == null)
            {
                throw new NotFoundException("Compra não encontrada.");
            }

            // compra ativa: devolve o estoque antes de apagar.
            // numa compra cancelada o estoque já foi devolvido, e não deve ser devolvido de novo.
            if (compra.Status == StatusCompra.Ativa)
            {
                var itens = compra.Itens.Select(i => (i.InstrumentoId, i.Quantidade)).ToList();
                await _repository.CancelarComEstornoAsync(id, itens, usuarioId, "Exclusão da compra pelo administrador");
            }

            await _repository.DeleteCompras(id);
        }
    }
}