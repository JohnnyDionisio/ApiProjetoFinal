namespace APItoPFinal.DTOs
{
    public class CompraInputDTO
    {
        public string Nome { get; set; }

        public string CPF { get; set; }

        public List<Guid> InstrumentoIds { get; set; }
    }
}
