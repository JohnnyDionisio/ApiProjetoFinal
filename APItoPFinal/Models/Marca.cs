using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.Models
{
    public class Marca
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(25, ErrorMessage = "Nome da marca deve ter no máximo 25 digitos")]
        public string Nome { get; set; }

        public ICollection<Instrumento>? Instrumentos { get; set; } = new List<Instrumento>();
    }
}
