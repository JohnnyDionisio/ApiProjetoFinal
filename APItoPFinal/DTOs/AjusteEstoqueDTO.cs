using APItoPFinal.Models;
using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class AjusteEstoqueDTO
    {
        [Required(ErrorMessage = "O tipo é obrigatório (Entrada ou Ajuste).")]
        public TipoMovimentoEstoque? Tipo { get; set; }

        [Range(-100000, 100000, ErrorMessage = "A quantidade deve estar entre -100000 e 100000.")]
        public int Quantidade { get; set; }

        [Required(ErrorMessage = "O motivo é obrigatório.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "O motivo deve ter entre 3 e 200 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
