using System.Text.Json.Serialization;

namespace APItoPFinal.Models
{
    public class Compra
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }

        public string CPF { get; set; }

        public DateTime DataCompra { get; set; } = DateTime.Now;

        //Relaçao com o Usuario
        public Guid InstrumentoId { get; set; }

        [JsonIgnore]
        public Instrumento? Instrumento { get; set; }


    }
}
