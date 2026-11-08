using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Exceptions;
using APItoPFinal.Helpers;
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

        public async Task<PaginadoDTO<InstrumentoDTO>> GetInstrumentos(InstrumentoFiltroDTO filtro)
        {
            if (filtro.PrecoMin.HasValue && filtro.PrecoMax.HasValue && filtro.PrecoMin > filtro.PrecoMax)
            {
                throw new BadRequestException("O preço mínimo não pode ser maior que o preço máximo.");
            }

            var (itens, total) = await _repository.BuscarAsync(filtro);

            return new PaginadoDTO<InstrumentoDTO>
            {
                Itens = itens,
                Pagina = filtro.Pagina,
                Tamanho = filtro.Tamanho,
                TotalItens = total,
                TotalPaginas = (int)Math.Ceiling(total / (double)filtro.Tamanho)
            };
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
                throw new ConflictException("Instrumento já existe.");
            }
            var identificacao = _repository.GetInstrumentoByIdentity(instrumento.Identificação);
            if (identificacao != null)
            {
                throw new ConflictException("Instrumento já cadastrado com essa identificação.");
            }

            instrumento.Id = Guid.NewGuid();
            // o estoque inicial vem do corpo da requisição (0 se não for informado)

            _repository.AdicionarInstrumento(instrumento);
        }

        public async Task AtualizarInstrumento(Instrumento instrumento)
        {
            var instrumentoExiste = await _repository.GetByIdAsync(instrumento.Id);
            if (instrumentoExiste == null)
            {
                throw new NotFoundException("Instrumento não encontrado.");
            }
            var identity = _repository.GetInstrumentoByIdentity(instrumento.Identificação);
            if (identity != null && identity.Id != instrumento.Id)
            {
                throw new ConflictException("Instrumento já cadastrado com essa identificação.");
            }

            instrumentoExiste.Identificação = instrumento.Identificação;
            instrumentoExiste.Nome = instrumento.Nome;
            instrumentoExiste.Preco = instrumento.Preco;
            instrumentoExiste.Descricao = instrumento.Descricao;
            // o estoque NÃO é alterado aqui: ele só muda por compra, cancelamento ou ajuste de estoque

            _repository.AtualizarInstrumento(instrumento.Id, instrumentoExiste);
        }

        public async Task DeletarInstrumento(Guid id)
        {
            var instrumentoExiste = await _repository.GetByIdAsync(id);
            if (instrumentoExiste == null)
            {
                throw new NotFoundException("Instrumento não encontrado.");
            }
            if (_repository.EstaEmCompra(id))
            {
                throw new ConflictException("Não é possível excluir: este instrumento faz parte de uma ou mais compras.");
            }
            try
            {
                _repository.DeletarInstrumento(id);
            }
            catch (DbUpdateException ex) when (DbErros.ViolacaoDeFk(ex))
            {
                throw new ConflictException("Não é possível excluir: este instrumento está em uso");
            }
        }
    }
}