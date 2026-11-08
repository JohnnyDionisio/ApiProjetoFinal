using APItoPFinal.Models;

namespace APItoPFinal.DTOs
{
    public class CompraDTO
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public DateTime DataCompra { get; set; }

        public Guid UsuarioId { get; set; }

        public decimal ValorTotal { get; set; }

        public StatusCompra Status { get; set; }

        public DateTime? DataCancelamento { get; set; }

        public List<CompraItemDTO> Itens { get; set; }
    }
}