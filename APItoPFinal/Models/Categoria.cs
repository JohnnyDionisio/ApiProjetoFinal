using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.Models
{
    public class Categoria
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Marca deve ter no máximo 15 digitos")]
        public string Nome { get; set; }

        public ICollection<Instrumento>? Instrumentos { get; set; } = new List<Instrumento>();
    }
}
