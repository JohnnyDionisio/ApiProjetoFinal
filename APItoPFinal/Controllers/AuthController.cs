using APItoPFinal.Models;
using APItoPFinal.Repository;
using APItoPFinal.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SymmetricSecurityKey _chave;
        private readonly JwtSettngs _jwt;
        private readonly UsuariosRepository _repository;

        public AuthController(SymmetricSecurityKey chave, JwtSettngs jwt, UsuariosRepository repository)
        {
            _chave = chave;
            _jwt = jwt;
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

            var expiracao = DateTime.UtcNow.AddMinutes(_jwt.ExpiraEmMinutos);
            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Name, usuario.CPF),
                    new Claim(ClaimTypes.Role, usuario.Cargo)
                },
                expires: expiracao,
                signingCredentials: new SigningCredentials(_chave, SecurityAlgorithms.HmacSha256)
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