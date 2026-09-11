using System.ComponentModel.DataAnnotations;

namespace APItoPFinal.Models
{
    public class Instrumento
    {

        public Guid Id { get; set; }

        public string Identificação { get; set; }

        public string Nome { get; set; }

        public double Preco { get; set; }

        public string Tipo { get; set; }

        public string Marca { get; set; }

        public string Descricao { get; set; }

        public bool Disponibilidade { get; set; } = true;

        public ICollection<Compra>? Compras { get; set; } = new List<Compra>();
    }
}
