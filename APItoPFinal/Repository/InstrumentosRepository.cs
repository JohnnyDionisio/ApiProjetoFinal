using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Repository
{
    public class InstrumentosRepository
    {
        private readonly AppDbContext _context;

        public InstrumentosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<InstrumentoDTO>> GetAllAsync()
        {
            return await _context.Instrumentos
                .Include(i => i.Categoria)
                .Include(i => i.Marca)
                .Select(i => new InstrumentoDTO
                {
                    Id = i.Id,
                    Identificação = i.Identificação,
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Descricao = i.Descricao,
                    Disponibilidade = i.Disponibilidade,
                    Categoria = i.Categoria.Nome,
                    Marca = i.Marca.Nome
                })
                .ToListAsync();
        }

        // Usado só internamente (validações, update, delete) - entidade completa
        public async Task<Instrumento> GetByIdAsync(Guid id)
        {
            return await _context.Instrumentos
                .Include(i => i.Categoria)
                .Include(i => i.Marca)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        // Usado pelo Controller no GET por id - já devolve o DTO
        public async Task<InstrumentoDTO?> GetByIdDTOAsync(Guid id)
        {
            return await _context.Instrumentos
                .Include(i => i.Categoria)
                .Include(i => i.Marca)
                .Where(i => i.Id == id)
                .Select(i => new InstrumentoDTO
                {
                    Id = i.Id,
                    Identificação = i.Identificação,
                    Nome = i.Nome,
                    Preco = i.Preco,
                    Descricao = i.Descricao,
                    Disponibilidade = i.Disponibilidade,
                    Categoria = i.Categoria.Nome,
                    Marca = i.Marca.Nome
                })
                .FirstOrDefaultAsync();
        }

        public Instrumento? GetInstrumentoByIdentity(string identificacao)
        {
            return _context.Instrumentos.FirstOrDefault(i => i.Identificação == identificacao);
        }

        public void AdicionarInstrumento(Instrumento instrumento)
        {
            _context.Instrumentos.Add(instrumento);
            _context.SaveChanges();
        }

        public void AtualizarInstrumento(Guid id, Instrumento instrumento)
        {
            _context.Instrumentos.Update(instrumento);
            _context.SaveChanges();
        }

        public void DeletarInstrumento(Guid id)
        {
            var instrumento = _context.Instrumentos.FirstOrDefault(i => i.Id == id);
            if (instrumento != null)
            {
                _context.Instrumentos.Remove(instrumento);
                _context.SaveChanges();
            }
        }
    }
}