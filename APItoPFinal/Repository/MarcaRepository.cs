using APItoPFinal.Data;
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
        public void AdicionarMarca(Marca marca)
        {
            _context.Marcas.Add(marca);
            _context.SaveChanges();
        }
        public void AtualizarMarca(Guid id,Marca marca)
        {
            _context.Marcas.Update(marca);
            _context.SaveChanges();
        }
        public void DeletarMarca(Guid id)
        {
            var marca = _context.Marcas.FirstOrDefault(m => m.Id == id);
            if(marca != null)
            {
                _context.Marcas.Remove(marca);
                _context.SaveChanges();
            }
        }
    }
}
