using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}
