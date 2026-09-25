using APItoPFinal.Data;
using APItoPFinal.DTOs;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;

namespace APItoPFinal.Repository
{
    public class MarcaRepository
    {
        private readonly AppDbContext _context;

        public MarcaRepository(AppDbContext context)
        {
            _context = context;
        }

        public List<Marca> GetMarcas()
        {
            return _context.Marcas.ToList();
        }

        public Marca? GetMarcaById(Guid id)
        {
            return _context.Marcas.FirstOrDefault(m => m.Id == id);
        }

        // NOVO - usado pelo Controller no GET por id, já com os instrumentos
        public async Task<MarcaDTO?> GetMarcaDTOByIdAsync(Guid id)
        {
            return await _context.Marcas
                .Include(m => m.Instrumentos)
                .Where(m => m.Id == id)
                .Select(m => new MarcaDTO
                {
                    Id = m.Id,
                    Nome = m.Nome,
                    Instrumentos = m.Instrumentos.Select(i => new InstrumentoResumoDTO
                    {
                        Id = i.Id,
                        Identificação = i.Identificação,
                        Nome = i.Nome,
                        Preco = i.Preco,
                        Descricao = i.Descricao,
                        Disponibilidade = i.Disponibilidade
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public void AdicionarMarca(Marca marca)
        {
            _context.Marcas.Add(marca);
            _context.SaveChanges();
        }
        public void AtualizarMarca(Guid id, Marca marca)
        {
            _context.Marcas.Update(marca);
            _context.SaveChanges();
        }
        public void DeletarMarca(Guid id)
        {
            var marca = _context.Marcas.FirstOrDefault(m => m.Id == id);
            if (marca != null)
            {
                _context.Marcas.Remove(marca);
                _context.SaveChanges();
            }
        }
    }
}