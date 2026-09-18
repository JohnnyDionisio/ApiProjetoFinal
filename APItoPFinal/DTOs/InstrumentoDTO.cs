namespace APItoPFinal.DTOs
{
    public class InstrumentoDTO
    {
        public Guid Id { get; set; }
        public string Identificação { get; set; }
        public string Nome { get; set; }
        public double Preco { get; set; }
        public string Descricao { get; set; }
        public bool Disponibilidade { get; set; }
        public string Categoria { get; set; }
        public string Marca { get; set; }
    }
}
