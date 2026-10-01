using APItoPFinal.DTOs;
using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MarcasController : ControllerBase
    {
        private readonly MarcaService _service;

        public MarcasController(MarcaService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<Marca> GetMarcas()
        {
            return Ok(_service.GetMarcas());
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<MarcaDTO>> GetMarcaById(Guid id)
        {
            var marca = await _service.GetMarcaDTOById(id);
            if (marca == null)
            {
                return NotFound();
            }
            return Ok(marca);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Marca> AdicionarMarcas(Marca marca)
        {
            try
            {
                _service.AdicionarMarca(marca);
                return CreatedAtAction(nameof(GetMarcaById), new { id = marca.Id }, marca);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult AtualizarMarca(Guid id, Marca marca)
        {
            if(id != marca.Id)
            {
                return BadRequest("O ID da URL não confere com o objeto.");
            }

            try
            {
                _service.AtualizarMarca(marca);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletarMarca(Guid id)
        {
            try
            {
                _service.DeletarMarca(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
