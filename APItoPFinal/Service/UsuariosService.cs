using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Repository;
using APItoPFinal.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

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

        private async Task GarantirUnicidade(string cpf, string email, string nomeUsuario, Guid? idAtual)
        {
            var porCpf = await _repository.GetUsuariosByCPF(cpf);
            if(porCpf != null && porCpf.Id != idAtual)
            {
                throw new Exception("Já existe um usuário cadastrado com esse CPF.");
            }

            var porEmail = await _repository.GetUsuariosByEmail(email);
            if(porEmail != null && porEmail.Id != idAtual)
            {
                throw new Exception("Já existe um usuário cadastrado com esse E-mail.");
            }

            var porNome = await _repository.GetUsuariosByName(nomeUsuario);
            if(porNome != null && porNome.Id != idAtual)
            {
                throw new Exception("Já existe um usuário cadastrado com esse nome de usuário.");
            }
        }

        public async Task<UsuarioDTO> AdicionarUsuarios(RegisterDTO dto)
        {
            var cpf = Cpf.Normalizar(dto.CPF);
            var email = dto.Email.Trim().ToLowerInvariant();
            var nomeUsuario = dto.NomeUsuario.Trim();

            await GarantirUnicidade(cpf, email, nomeUsuario, null);

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome.Trim(),
                NomeUsuario = nomeUsuario,
                Email = email,
                CPF = cpf,
                Senha = dto.Senha, // o hash é feito no PostUsuário do Repository
                Cargo = "Cliente"
            };

            try
            {
                await _repository.PostUsuarios(usuario);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                // duas requisições simultâneas passaram pela checagem acima: o índice único barrou
                throw new Exception("CPF, E-mail ou nome de usuário já cadastrados.");
            }

            return ToDTO(usuario);

        }

        public async Task AtualizarUsuarios(Guid id, UsuarioAtualizadoDTO dto)
        {
            var usuario = await _repository.GetUsuariosById(id);
            if(usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado");
            }

            var cpf = Cpf.Normalizar(dto.CPF);
            var email = dto.Email.Trim().ToLowerInvariant();
            var nomeUsuario = dto.NomeUsuario.Trim();

            await GarantirUnicidade(cpf, email, nomeUsuario, id);

            usuario.Nome = dto.Nome.Trim();
            usuario.NomeUsuario = nomeUsuario;
            usuario.Email = email;
            usuario.CPF = cpf;

            try
            {
                await _repository.PutUsuarios(id, usuario);
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
            {
                throw new Exception("CPF, e-mail ou nome de usuário já cadastrado.");
            } 
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