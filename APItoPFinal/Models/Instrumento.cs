using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.Models
{
    public class Instrumento
    {

        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo Identificação é obrigatório.")]
        public string Identificação { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        [StringLength(50, ErrorMessage = "O campo Nome deve ter no máximo 50 caracteres.")]
        public string Nome { get; set; } 

        [Required(ErrorMessage = "O campo Preço é obrigatório.")]
        public double Preco { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo Descrição deve ter no máximo 100 caracteres.")]
        public string Descricao { get; set; }

        public bool Disponibilidade { get; set; } = true;

        public Guid IdCategoria { get; set; }
        public Categoria? Categoria { get; set; }

        public Guid IdMarca { get; set; }
        public Marca? Marca { get; set; }

        public ICollection<Compra>? Compras { get; set; } = new List<Compra>();
    }
}
