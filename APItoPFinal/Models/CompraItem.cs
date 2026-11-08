using System.Text.Json.Serialization;

namespace APItoPFinal.Models
{
    public class CompraItem
    {
        public Guid CompraId { get; set; }

        [JsonIgnore]
        public Compra? Compra { get; set; }
        public Guid InstrumentoId { get; set; }

        [JsonIgnore]
        public Instrumento? Instrumento { get; set; }

        public int Quantidade  { get; set; }

        // preço cobrado na época da compra: mudar o preço do instrumento depois não altera compras antigas
        public decimal PrecoUnitario { get; set; }
    }
}
