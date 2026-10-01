using APItoPFinal.Models;
using APItoPFinal.Repository;

namespace APItoPFinal.Service
{
    public class UsuariosService
    {
        private readonly UsuariosRepository _repository;

        public UsuariosService(UsuariosRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Usuario>> GetUsuarios()
        {
            var usuario = await _repository.GetUsuarios();
            return usuario.ToList();
        }

        public async Task<Usuario?> GetUsuariosById(Guid id)
        {
            var usuario = await _repository.GetUsuarios();
            return usuario.FirstOrDefault(u => u.Id == id);
        }

        public async Task AdicionarUsuarios(Usuario usuario)
        {
            var usuarioExiste = await _repository.GetUsuariosById(usuario.Id);
            if (usuarioExiste != null)
            {
                throw new Exception("Usuario já existe.");
            }
            var usuarioCpf = await _repository.GetUsuariosByCPF(usuario.CPF);
            if (usuarioCpf != null)
            {
                throw new Exception("Já existe um usuário cadastrado com esse CPF");
            }
            var usuarioNome = await _repository.GetUsuariosByName(usuario.NomeUsuario);
            if (usuarioNome != null)
            {
                throw new Exception("Já existe um cadastro com esse nome de usuário");
            }

            usuario.Id = Guid.NewGuid();
            usuario.Cargo = "Cliente";

            await _repository.PostUsuarios(usuario);
        }

        public async Task AtualizarUsuarios(Usuario usuario)
        {
            var usuarioExiste = await _repository.GetUsuariosById(usuario.Id);
            if (usuarioExiste == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            var usuarioCpf = await _repository.GetUsuariosByCPF(usuario.CPF);
            if (usuarioCpf != null && usuarioCpf.Id != usuario.Id)
            {
                throw new Exception("CPF já cadastrado");
            }

            var usuarioNome = await _repository.GetUsuariosByName(usuario.NomeUsuario);
            if (usuarioNome != null && usuarioNome.Id != usuario.Id)
            {
                throw new Exception("Já existe um cadastro com esse nome de usuário");
            }

            usuarioExiste.NomeUsuario = usuario.NomeUsuario;
            usuarioExiste.CPF = usuario.CPF;

            await _repository.PutUsuarios(usuario.Id, usuarioExiste);
        }

        public async Task DeletarUsuarios(Guid id)
        {
            var usuarioExiste = await _repository.GetUsuariosById(id);
            if (usuarioExiste == null)
            {
                throw new Exception("Usuário não encontrado");
            }

             _repository.DeleteUsuario(id);
        }
    }
}