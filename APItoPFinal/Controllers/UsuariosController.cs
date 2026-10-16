using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuariosService _service;

        public UsuariosController(UsuariosService service)
        {
            _service = service;
        }

        private Guid GetUsuarioLogado()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return Ok(await _service.GetUsuarios());
        }
        [HttpGet("me")]
        public async Task<ActionResult<UsuarioDTO>> GetMe()
        {
            var usuario = await _service.GetUsuariosById(GetUsuarioLogado());
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuariosById(Guid id)
        {
            if (!User.IsInRole("Admin") && id != GetUsuarioLogado())
            {
                return Forbid();
            }
            var usuario = await _service.GetUsuariosById(id);
            if(usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> PostUsuarios(Usuario usuario)
        {
            try
            {
                var criado = await _service.AdicionarUsuarios(usuario);
                return CreatedAtAction("GetUsuariosById", new { id = criado.Id }, criado);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(Guid id, UsuarioAtualizadoDTO dto)
        {
            try
            {
                await _service.AtualizarUsuarios(id, dto);
                return NoContent();
            }
            catch(KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuarios(Guid id)
        {
            try
            {
            await _service.DeletarUsuarios(id);
            return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
