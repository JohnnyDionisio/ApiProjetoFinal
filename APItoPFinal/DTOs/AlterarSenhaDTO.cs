using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class AlterarSenhaDTO
    {
        [Required(ErrorMessage = "A senha atual é obrigatória.")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "A nova senha é obrigatória.")]
        [StringLength(72, MinimumLength = 8, ErrorMessage = "A nova senha deve ter entre 8 e 72 caracteres.")]
        public string NovaSenha { get; set; } = string.Empty;
    }
}
