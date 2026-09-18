using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APItoPFinal.Models
{
    public class Compra
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Nome pode ter até 100 digitos")]
        public string Nome { get; set; }

        [Required]
        [StringLength(11, ErrorMessage="Cpf pode ter apenas no máximo 11 digitos")]
        public string CPF { get; set; }

        public DateTime DataCompra { get; set; } = DateTime.Now;

        //Relaçao com o Usuario
        public Guid InstrumentoId { get; set; }

        [JsonIgnore]
        public Instrumento? Instrumento { get; set; }


    }
}
