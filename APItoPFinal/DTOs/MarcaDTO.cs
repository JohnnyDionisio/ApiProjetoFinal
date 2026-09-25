namespace APItoPFinal.DTOs
{
    public class MarcaDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public List<InstrumentoResumoDTO> Instrumentos { get; set; }
    }
}
