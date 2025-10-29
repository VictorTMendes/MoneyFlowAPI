namespace MoneyFlowAPI.Models
{
    public class Despesa
    {
        public int Id { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataDespesa { get; set; } = DateTime.Now;
        public string CategoriaId { get; set; } = string.Empty;

        // Relação com o usuário
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
