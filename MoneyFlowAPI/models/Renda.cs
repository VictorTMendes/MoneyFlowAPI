namespace MoneyFlowAPI.Models
{
    public class Renda
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string CategoriaId { get; set; } = string.Empty;

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
