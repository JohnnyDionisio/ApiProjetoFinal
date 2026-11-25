namespace APItoPFinal.DTOs
{
    public class CarrinhoItemDTO
    {
        public Guid InstrumentoId { get; set; }
        public string Identificação { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal PrecoUnitario { get; set; }
        public int Quantidade { get; set; }
        public int Estoque { get; set; }

        public decimal Subtotal => Quantidade * PrecoUnitario;
        public bool EstoqueSuficiente => Estoque >= Quantidade;

        public string? Aviso =>
            Estoque == 0 ? "Sem estoque."
            : Estoque < Quantidade ? $"Estoque insuficiente: restam {Estoque}."
            : null;
    }
}
