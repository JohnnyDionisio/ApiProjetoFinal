namespace APItoPFinal.Models
{
    public class Categoria
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public ICollection<Instrumento>? Instrumentos { get; set; } = new List<Instrumento>();
    }
}
