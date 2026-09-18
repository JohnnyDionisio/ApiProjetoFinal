using APItoPFinal.Models;
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

        public AuthController(SymmetricSecurityKey chave)
        {
            this.chave = chave;
        }
        [HttpPost("login")]
        public IActionResult Login(LoginRequest dados)
        {
            if(dados.Usuario != "Johnny" || dados.Senha != "123456")
            {
                return Unauthorized();
            }

            var expiracao = DateTime.UtcNow.AddMinutes(60);
            var token = new JwtSecurityToken(
                issuer: "ExemploJwt",
                audience: "Alunos",
                claims: new[] { new Claim(ClaimTypes.Name, dados.Usuario) },
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
