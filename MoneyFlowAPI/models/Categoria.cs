using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MoneyFlowAPI.Models
{
    public class Categoria
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; } = string.Empty;

        [MaxLength(255)]
        public string? Descricao { get; set; }

        [JsonIgnore]
        public ICollection<Renda> Rendas { get; set; } = new List<Renda>();
    }
}