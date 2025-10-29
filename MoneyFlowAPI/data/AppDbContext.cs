using Microsoft.EntityFrameworkCore;
using MoneyFlowAPI.Models;

namespace MoneyFlowAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<Renda> Rendas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
