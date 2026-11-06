namespace APItoPFinal.DTOs
{
    public class PaginadoDTO<T>
    {
        public List<T> Itens { get; set; } = new();
        public int Pagina { get; set; }
        public int Tamanho { get; set; }
        public int TotalItens { get; set; }
        public int TotalPaginas { get; set; }
    }
}
