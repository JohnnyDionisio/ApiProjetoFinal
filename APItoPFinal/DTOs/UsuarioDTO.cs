namespace APItoPFinal.DTOs
{
    public class UsuarioDTO
    {
        public Guid Id { get; set; }

        public string Nome { get; set; }        

        public string NomeUsuario { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string CPF { get; set; } = string.Empty;

        public string Cargo { get; set; } = string.Empty;
    }
}
