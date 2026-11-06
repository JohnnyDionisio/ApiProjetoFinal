using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class AlterarCargoDTO
    {
        [Required(ErrorMessage = "O cargo é obrigatório.")]
        public string Cargo { get; set; } = string.Empty;
    }
}
