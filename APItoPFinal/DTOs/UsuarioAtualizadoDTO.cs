using APItoPFinal.Helpers;
using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class UsuarioAtualizadoDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome de usuário é obrigatório")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 30 caracteres.")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O Email é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        [CpfValidation]
        public string CPF { get; set; } = string.Empty;
    }
}
