using APItoPFinal.Models;
using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class MovimentoFiltroDTO
    {
        public TipoMovimentoEstoque? Tipo { get; set; }

        [Range(1, 100000, ErrorMessage = "A página deve ser maior ou igual a 1.")]
        public int Pagina { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100.")]
        public int Tamanho { get; set; } = 20;
    }
}
