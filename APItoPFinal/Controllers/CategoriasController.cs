using APItoPFinal.Models;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public ActionResult<Categoria> GetCategoriaById(Guid id)
        {
            var categoria = _service.GetCategoriaById(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }

        [HttpPost]
        public ActionResult<Categoria> PostCategoria(Categoria categoria)
        {
            try
            {
                _service.AdicionarCategoria(categoria);
                return CreatedAtAction(nameof(GetCategoriaById), new { id = categoria.Id }, categoria);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult PutCategoria(Guid id, Categoria categoria)
        {
            if (id != categoria.Id)
            {
                return BadRequest("O ID da URL não confere com o objeto.");
            }

            try
            {
                _service.AtualizarCategoria(categoria);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCategoria(Guid id)
        {
            try
            {
                _service.DeletarCategoria(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}