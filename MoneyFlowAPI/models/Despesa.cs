namespace MoneyFlowAPI.Models
{
    public class Despesa
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public string Descricao { get; set; } = string.Empty;
        public DateTime DataDespesa { get; set; } = DateTime.Now;
        public int CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }


        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
