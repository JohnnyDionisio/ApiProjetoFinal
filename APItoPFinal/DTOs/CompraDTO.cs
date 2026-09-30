namespace APItoPFinal.DTOs
{
    public class CompraDTO
    {
        public Guid Id { get; set;  }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public DateTime DataCompra { get; set; }

        public List<InstrumentoResumoDTO> Instrumentos { get; set; }
    }
}
