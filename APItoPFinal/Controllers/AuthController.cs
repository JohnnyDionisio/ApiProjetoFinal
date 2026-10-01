using APItoPFinal.Models;
using APItoPFinal.Repository;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SymmetricSecurityKey chave;
        private readonly UsuariosRepository _repository;

        public AuthController(SymmetricSecurityKey chave, UsuariosRepository repository)
        {
            this.chave = chave;
            _repository = repository;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(Usuario dados)
        {
            var usuario = await _repository.GetUsuariosByCPF(dados.CPF);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dados.Senha, usuario.Senha))
            {
                return Unauthorized();
            }

            var expiracao = DateTime.UtcNow.AddMinutes(60);
            var token = new JwtSecurityToken(
                issuer: "ExemploJwt",
                audience: "Alunos",
                claims: new[] {new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()), new Claim(ClaimTypes.Name, dados.CPF), new Claim(ClaimTypes.Role, usuario.Cargo) },
                expires: expiracao,
                signingCredentials: new SigningCredentials(chave, SecurityAlgorithms.HmacSha256)
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                tipo = "Bearer",
                expiraEm = expiracao
            });
        }
    }
}
