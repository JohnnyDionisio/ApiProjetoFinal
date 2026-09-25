namespace APItoPFinal.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string NomeUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public string Senha { get; set; } = string.Empty;
    }
}
