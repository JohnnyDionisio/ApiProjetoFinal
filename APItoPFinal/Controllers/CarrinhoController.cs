using System.Security.Claims;
using APItoPFinal.DTOs;
using APItoPFinal.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APItoPFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarrinhoController : ControllerBase
    {
        private readonly CarrinhoService _service;

        public CarrinhoController(CarrinhoService service)
        {
            _service = service;
        }

        // o carrinho é sempre o do usuário do token: não existe id de usuário na rota
        private Guid GetUsuarioLogadoId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        [HttpGet]
        public async Task<ActionResult<CarrinhoDTO>> GetCarrinho()
        {
            return await _service.GetCarrinho(GetUsuarioLogadoId());
        }

        [HttpPost("itens")]
        public async Task<ActionResult<CarrinhoDTO>> AdicionarItem(CarrinhoItemInputDTO dto)
        {
            return await _service.AdicionarItem(GetUsuarioLogadoId(), dto);
        }

        [HttpPut("itens/{instrumentoId}")]
        public async Task<ActionResult<CarrinhoDTO>> AlterarQuantidade(Guid instrumentoId, CarrinhoQuantidadeDTO dto)
        {
            return await _service.AlterarQuantidade(GetUsuarioLogadoId(), instrumentoId, dto.Quantidade);
        }

        [HttpDelete("itens/{instrumentoId}")]
        public async Task<IActionResult> RemoverItem(Guid instrumentoId)
        {
            await _service.RemoverItem(GetUsuarioLogadoId(), instrumentoId);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Esvaziar()
        {
            await _service.Esvaziar(GetUsuarioLogadoId());
            return NoContent();
        }

        [HttpPost("finalizar")]
        public async Task<ActionResult<CompraDTO>> Finalizar()
        {
            var compra = await _service.Finalizar(GetUsuarioLogadoId());
            return Created($"/api/compras/{compra.Id}", compra);
        }
    }
}