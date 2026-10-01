using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _service.GetUsuarios();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuariosById(Guid id)
        {
            var usuario = await _service.GetUsuariosById(id);
            if(usuario == null)
            {
                return NotFound();
            }
            return usuario;
        }
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuarios(Usuario usuario)
        {
            usuario.Id = Guid.NewGuid();
            await _service.AdicionarUsuarios(usuario);
            return CreatedAtAction("GetUsuariosById", new {id = usuario.Id}, usuario);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuarios(Guid id, Usuario usuario)
        {
            if (id == usuario.Id)
            {
                return BadRequest();
            }

            try
            {
                await _service.AtualizarUsuarios(usuario);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuarios(Guid id)
        {
            var usuario = await _service.GetUsuariosById(id);
            if(usuario == null)
            {
                return NotFound();
            }
            await _service.DeletarUsuarios(id);
            return NoContent();
        }
    }
}
