using APItoPFinal.DTOs;
using APItoPFinal.Exceptions;
using APItoPFinal.Models;
using APItoPFinal.Repository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Service
{
    public class CarrinhoService
    {
        private const int QuantidadeMaxima = 1000;

        private readonly CarrinhoRepository _repository;
        private readonly InstrumentosRepository _instrumentos;
        private readonly CompraService _compraService;

        public CarrinhoService(CarrinhoRepository repository, InstrumentosRepository instrumentos, CompraService compraService)
        {
            _repository = repository;
            _instrumentos = instrumentos;
            _compraService = compraService;
        }

        private static CarrinhoItemDTO ToDTO(CarrinhoItem c) => new CarrinhoItemDTO
        {
            InstrumentoId = c.InstrumentoId,
            Identificação = c.Instrumento!.Identificação,
            Nome = c.Instrumento.Nome,
            PrecoUnitario = c.Instrumento.Preco,   // preço atual: o preço da época só é gravado ao finalizar
            Quantidade = c.Quantidade,
            Estoque = c.Instrumento.Estoque
        };

        private async Task<Instrumento> BuscarInstrumento(Guid instrumentoId)
        {
            var lista = await _instrumentos.GetByIdsAsync(new[] { instrumentoId });
            var instrumento = lista.FirstOrDefault();
            if (instrumento == null)
            {
                throw new NotFoundException("Instrumento não encontrado.");
            }
            return instrumento;
        }

        private static void ValidarQuantidade(Instrumento instrumento, int quantidade)
        {
            if (quantidade > QuantidadeMaxima)
            {
                throw new BadRequestException($"A quantidade máxima por item é {QuantidadeMaxima}.");
            }
            if (instrumento.Estoque == 0)
            {
                throw new ConflictException($"O instrumento '{instrumento.Nome}' está sem estoque.");
            }
            if (instrumento.Estoque < quantidade)
            {
                throw new ConflictException(
                    $"Estoque insuficiente para '{instrumento.Nome}': restam {instrumento.Estoque}, e o carrinho ficaria com {quantidade}.");
            }
        }

        public async Task<CarrinhoDTO> GetCarrinho(Guid usuarioId)
        {
            var itens = await _repository.GetItensAsync(usuarioId);
            return new CarrinhoDTO { Itens = itens.Select(ToDTO).ToList() };
        }

        public async Task<CarrinhoDTO> AdicionarItem(Guid usuarioId, CarrinhoItemInputDTO dto)
        {
            var instrumento = await BuscarInstrumento(dto.InstrumentoId);

            var existente = await _repository.GetItemAsync(usuarioId, dto.InstrumentoId);
            var novaQuantidade = (existente?.Quantidade ?? 0) + dto.Quantidade;

            ValidarQuantidade(instrumento, novaQuantidade);

            try
            {
                if (existente == null)
                {
                    await _repository.AdicionarAsync(new CarrinhoItem
                    {
                        UsuarioId = usuarioId,
                        InstrumentoId = dto.InstrumentoId,
                        Quantidade = novaQuantidade,
                        DataAdicao = DateTime.UtcNow
                    });
                }
                else
                {
                    existente.Quantidade = novaQuantidade;
                    await _repository.SalvarAsync();
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                // duas requisições adicionaram o mesmo instrumento ao mesmo tempo
                throw new ConflictException("O carrinho foi alterado ao mesmo tempo. Tente novamente.");
            }

            return await GetCarrinho(usuarioId);
        }

        public async Task<CarrinhoDTO> AlterarQuantidade(Guid usuarioId, Guid instrumentoId, int quantidade)
        {
            var item = await _repository.GetItemAsync(usuarioId, instrumentoId);
            if (item == null)
            {
                throw new NotFoundException("Esse instrumento não está no carrinho.");
            }

            var instrumento = await BuscarInstrumento(instrumentoId);
            ValidarQuantidade(instrumento, quantidade);

            item.Quantidade = quantidade;
            await _repository.SalvarAsync();

            return await GetCarrinho(usuarioId);
        }

        public async Task RemoverItem(Guid usuarioId, Guid instrumentoId)
        {
            var removido = await _repository.RemoverItemAsync(usuarioId, instrumentoId);
            if (!removido)
            {
                throw new NotFoundException("Esse instrumento não está no carrinho.");
            }
        }

        public async Task Esvaziar(Guid usuarioId)
        {
            await _repository.EsvaziarAsync(usuarioId);
        }

        public async Task<CompraDTO> Finalizar(Guid usuarioId)
        {
            var itens = await _repository.GetItensAsync(usuarioId);
            if (itens.Count == 0)
            {
                throw new BadRequestException("O carrinho está vazio.");
            }

            var input = new CompraInputDTO
            {
                Itens = itens.Select(i => new CompraItemInputDTO
                {
                    InstrumentoId = i.InstrumentoId,
                    Quantidade = i.Quantidade
                }).ToList()
            };

            // mesma regra de qualquer compra: estoque conferido, baixa, movimentos, preço da época.
            // consumirCarrinho: true esvazia o carrinho dentro da mesma transação.
            var compraId = await _compraService.AdicionarCompra(input, usuarioId, consumirCarrinho: true);

            var compra = await _compraService.GetComprasDTOById(compraId, usuarioId, false);
            return compra!;
        }
    }
}