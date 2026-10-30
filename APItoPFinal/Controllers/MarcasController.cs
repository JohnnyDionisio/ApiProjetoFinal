using APItoPFinal.DTOs;
using APItoPFinal.Exceptions;
using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<IEnumerable<Marca>> GetMarcas()
        {
            return Ok(_service.GetMarcas());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MarcaDTO>> GetMarcaById(Guid id)
        {
            var marca = await _service.GetMarcaDTOById(id);
            if (marca == null)
            {
                throw new NotFoundException("Marca não encontrada.");
            }
            return Ok(marca);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Marca> AdicionarMarcas(Marca marca)
        {
            _service.AdicionarMarca(marca);
            return CreatedAtAction(nameof(GetMarcaById), new { id = marca.Id }, marca);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult AtualizarMarca(Guid id, Marca marca)
        {
            if (id != marca.Id)
            {
                throw new BadRequestException("O ID da URL não confere com o objeto.");
            }

            _service.AtualizarMarca(marca);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeletarMarca(Guid id)
        {
            _service.DeletarMarca(id);
            return NoContent();
        }
    }
}