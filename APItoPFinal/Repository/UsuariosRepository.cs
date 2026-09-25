using APItoPFinal.Data;
using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace APItoPFinal.Repository
{
    public class UsuariosRepository
    {
        private readonly AppDbContext _context;

        public UsuariosRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }
        public async Task<Usuario?> GetUsuariosById(Guid id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
        }
        public async Task<Usuario?> GetUsuariosByCPF(string cpf)
        {
            return _context.Usuarios.FirstOrDefault(u => u.CPF == cpf);
        }
        public async Task<Usuario?> GetUsuariosByName(string usuarioNome)
        {
            return _context.Usuarios.FirstOrDefault(u => u.NomeUsuario == usuarioNome);
        }
        public async Task<Usuario> PostUsuarios(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }
        public async Task<Usuario> PutUsuarios(Guid id, Usuario usuario)
        {
            _context.Entry(usuario).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DBConcurrencyException)
            {
                var usuarioTemp = _context.Usuarios.Any(u => u.Id == id);
                if (!usuarioTemp)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
            return usuario;
        }
        public async Task<Usuario?> DeleteUsuario(Guid id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if(usuario == null)
            {
                return null;
            }
            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }
    }
}
