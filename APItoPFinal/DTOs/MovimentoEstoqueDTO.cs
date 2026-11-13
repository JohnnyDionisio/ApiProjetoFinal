using APItoPFinal.Models;

namespace APItoPFinal.DTOs
{
    public class MovimentoEstoqueDTO
    {
        public Guid Id { get; set; }
        public Guid InstrumentoId { get; set; }
        public TipoMovimentoEstoque Tipo { get; set; }
        public int Variacao { get; set; }
        public int EstoqueApos { get; set; }
        public string? Motivo { get; set; }
        public Guid? CompraId { get; set; }
        public Guid? UsuarioId { get; set; }
        public DateTime DataHora { get; set; }
    }
}
