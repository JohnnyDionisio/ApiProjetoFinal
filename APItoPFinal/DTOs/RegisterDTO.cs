using APItoPFinal.Helpers;
using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "O Nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O Nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Nome de usuário é obrigatório")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 30 caracteres.")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150, ErrorMessage = "O Email deve ter no máximo 150 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [CpfValidation]
        public string CPF { get; set; } = string.Empty;

        [Required(ErrorMessage = "A Senha é obrigatória")]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "A Senha deve ter entre 8 e 72 caracteres")]
        public string Senha { get; set; } = string.Empty;

    }
}
