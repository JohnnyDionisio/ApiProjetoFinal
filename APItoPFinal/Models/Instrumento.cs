using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [Range(0.01, 99999999.99, ErrorMessage = "O campo Preço deve ser maior que zero.")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [StringLength(100, ErrorMessage = "O campo Descrição deve ter no máximo 100 caracteres.")]
        public string Descricao { get; set; }

        [Range(0, 1000000, ErrorMessage = "O Estoque deve ser zero ou maior.")]
        public int Estoque { get; set; }


        public Guid IdCategoria { get; set; }
        [ForeignKey("IdCategoria")]
        public Categoria? Categoria { get; set; }

        public Guid IdMarca { get; set; }
        [ForeignKey("IdMarca")]
        public Marca? Marca { get; set; }

        [Timestamp]
        [JsonIgnore]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }
}