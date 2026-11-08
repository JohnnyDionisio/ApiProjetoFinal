namespace APItoPFinal.DTOs
{
    public class CompraItemDTO
    {
        public Guid InstrumentoId { get; set; }
        public string Identificação { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Quantidade { get; set; } 
        public decimal PrecoUnitario { get; set; }
        public decimal SubTotal => Quantidade * PrecoUnitario;
    }
}
