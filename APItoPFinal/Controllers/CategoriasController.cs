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
    public class CategoriasController : ControllerBase
    {
        private readonly CategoriaService _service;

        public CategoriasController(CategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Categoria>> GetCategorias()
        {
            return Ok(_service.GetCategorias());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDTO>> GetCategoriaById(Guid id)
        {
            var categoria = await _service.GetCategoriaDTOById(id);
            if (categoria == null)
            {
                throw new NotFoundException("Categoria não encontrada.");
            }
            return Ok(categoria);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult<Categoria> PostCategoria(Categoria categoria)
        {
            _service.AdicionarCategoria(categoria);
            return CreatedAtAction(nameof(GetCategoriaById), new { id = categoria.Id }, categoria);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult PutCategoria(Guid id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                throw new BadRequestException("O ID da URL não confere com o objeto.");
            }

            _service.AtualizarCategoria(categoria);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteCategoria(Guid id)
        {
            _service.DeletarCategoria(id);
            return NoContent();
        }
    }
}