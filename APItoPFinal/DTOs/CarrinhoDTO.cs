namespace APItoPFinal.DTOs
{
    public class CarrinhoDTO
    {
        public List<CarrinhoItemDTO> Itens { get; set; } = new();

        public int TotalUnidades => Itens.Sum(i => i.Quantidade);
        public decimal ValorTotal => Itens.Sum(i => i.Subtotal);

        // o carrinho só pode ser finalizado se tiver itens e todos tiverem estoque
        public bool PodeFinalizar => Itens.Count > 0 && Itens.All(i => i.EstoqueSuficiente);
    }
}
