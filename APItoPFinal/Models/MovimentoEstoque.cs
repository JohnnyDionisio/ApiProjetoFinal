namespace APItoPFinal.Models
{
    public class MovimentoEstoque
    {
        public Guid Id { get; set; }

        public Guid InstrumentoId { get; set; }

        public TipoMovimentoEstoque Tipo { get; set; }

        // quanto mudou: positivo entra, negativo sai
        public int Variacao { get; set; }

        // quanto ficou no estoque depois deste movimento
        public int EstoqueApos { get; set; }

        public string? Motivo { get; set; }

        // preenchidos nas compras e cancelamentos
        public Guid? CompraId { get; set; }

        // quem fez (pode ficar vazio se o usuário for apagado, ou no saldo inicial da migração)
        public Guid? UsuarioId { get; set; }

        public DateTime DataHora { get; set; }
    }
}
