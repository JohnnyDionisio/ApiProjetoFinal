using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.Models
{
    public class LoginRequest
    {
        [Required]
        public string Usuario { get; set; } = string.Empty;

        [Required]
        public string Senha { get; set; } = string.Empty;
    }
}
