using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace APItoPFinal.Repository
{
    public class InstrumentosRepository
    {
        private readonly AppDbContext _context;

        public InstrumentosRepository(AppDbContext context)
        {
            _context = context;
        }
        public List<Instrumento> GetInstrumentos()
        {
            return _context.Instrumentos.ToList();
        }
        public Instrumento? GetInstrumentosById(Guid id)
        {
            return _context.Instrumentos.FirstOrDefault(i => i.Id == id);
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
            var instrumento = _context.Instrumentos.Find(i => i.Id == id);
            if (instrumento != null)
            {
                _context.Instrumentos.Remove(instrumento);
                _context.SaveChanges();
            }
        }
    }
}
