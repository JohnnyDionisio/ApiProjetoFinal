using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.DTOs
{
    public class CompraItemInputDTO
    {
        public Guid InstrumentoId { get; set; }

        [Range(1, 1000, ErrorMessage = "A quantidade deve ser entre 1 e 1000.")]
        public int Quantidade { get; set; } = 1;
    }
}
