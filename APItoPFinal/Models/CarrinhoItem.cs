using System.Text.Json.Serialization;

namespace APItoPFinal.Models
{
    public class CarrinhoItem
    {
        public Guid UsuarioId { get; set; }

        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        public Guid InstrumentoId { get; set; }

        [JsonIgnore]
        public Instrumento? Instrumento { get; set; }

        public int Quantidade { get; set; }

        public DateTime DataAdicao { get; set; }
    }
}
