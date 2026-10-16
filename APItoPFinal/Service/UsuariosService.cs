using APItoPFinal.DTOs;
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

        private static UsuarioDTO ToDTO(Usuario u) => new UsuarioDTO
        {
            Id = u.Id,
            Nome = u.Nome,
            NomeUsuario = u.NomeUsuario,
            Email = u.Email,
            CPF = u.CPF,
            Cargo = u.Cargo
        };

        public async Task<List<UsuarioDTO>> GetUsuarios()
        {
            var usuarios = await _repository.GetUsuarios();
            return usuarios.Select(ToDTO).ToList();
        }

        public async Task<UsuarioDTO?> GetUsuariosById(Guid id)
        {
            var usuario = await _repository.GetUsuariosById(id);
            return usuario == null? null : ToDTO(usuario);
        }

        public async Task<UsuarioDTO> AdicionarUsuarios(Usuario usuario)
        {
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
            return ToDTO(usuario);
        }

        public async Task AtualizarUsuarios(Guid id, UsuarioAtualizadoDTO dto)
        {
            var usuario = await _repository.GetUsuariosById(id);
            if(usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            var usuarioCpf = await _repository.GetUsuariosByCPF(dto.CPF);
            if(usuarioCpf != null || usuarioCpf.Id != id)
            {
                throw new Exception("Já existe um usuário cadastrado com esse CPF");
            }

            usuario.Nome = dto.Nome;
            usuario.NomeUsuario = dto.NomeUsuario;
            usuario.Email = dto.Email;
            usuario.CPF = dto.CPF;

            await _repository.PutUsuarios(id, usuario);
        }

        public async Task DeletarUsuarios(Guid id)
        {
            var usuarioExiste = await _repository.GetUsuariosById(id);
            if (usuarioExiste == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

             _repository.DeleteUsuario(id);
        }
    }
}