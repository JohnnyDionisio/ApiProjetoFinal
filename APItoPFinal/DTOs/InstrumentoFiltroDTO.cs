using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class InstrumentoFiltroDTO
    {
        public string? Busca {  get; set; }

        public Guid? CategoriaId { get; set; }
        public Guid? MarcaId { get; set; }

        public decimal? PrecoMin {  get; set; }
        public decimal? PrecoMax { get; set; }

        public bool? Disponivel { get; set; }

        [RegularExpression("(?i)^(nome|pre[cç]o)$", ErrorMessage = "Ordenar deve ser por 'Nome' ou 'Preço'.")]
        public string? OrdenarPor { get; set; }

        public bool Decrescente { get; set; }

        [Range(1, 100000, ErrorMessage = "A página deve ser maior ou igual a 1.")]
        public int Pagina { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "O tamanho da página deve estar entre 1 e 100.")]
        public int Tamanho { get; set; } = 20;
    }
}
