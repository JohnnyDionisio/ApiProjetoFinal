using System.Security.Claims;
using APItoPFinal.DTOs;
using APItoPFinal.Exceptions;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosService _service;

        public UsuariosController(UsuariosService service)
        {
            _service = service;
        }

        private Guid GetUsuarioLogadoId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDTO>>> GetUsuarios()
        {
            return Ok(await _service.GetUsuarios());
        }

        [HttpGet("me")]
        public async Task<ActionResult<UsuarioDTO>> GetMe()
        {
            var usuario = await _service.GetUsuariosById(GetUsuarioLogadoId());
            if (usuario == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuariosById(Guid id)
        {
            if (!User.IsInRole("Admin") && id != GetUsuarioLogadoId())
            {
                throw new UnauthorizedAccessException("Você só pode acessar os seus próprios dados.");
            }

            var usuario = await _service.GetUsuariosById(id);
            if (usuario == null)
            {
                throw new NotFoundException("Usuário não encontrado.");
            }
            return Ok(usuario);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> PostUsuarios(RegisterDTO dto)
        {
            var criado = await _service.AdicionarUsuarios(dto);
            return CreatedAtAction("GetUsuariosById", new { id = criado.Id }, criado);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(Guid id, UsuarioAtualizadoDTO dto)
        {
            await _service.AtualizarUsuarios(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuarios(Guid id)
        {
            await _service.DeletarUsuarios(id);
            return NoContent();
        }
    }
}