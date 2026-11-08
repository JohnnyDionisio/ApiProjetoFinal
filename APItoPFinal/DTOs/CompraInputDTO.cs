using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class CompraInputDTO
    {
        [MinLength(1, ErrorMessage = "Informe pelo menos 1 item.")]
        public List<CompraItemInputDTO> Itens { get; set; } = new();
    }
}
