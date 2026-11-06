using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class PerfilUpdateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "O nome de usuário deve ter entre 3 e 30 caracteres.")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;
    }
}
