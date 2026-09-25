namespace APItoPFinal.DTOs
{
    public class CategoriaDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public List<InstrumentoResumoDTO> Instrumentos { get; set; }
    }
}
