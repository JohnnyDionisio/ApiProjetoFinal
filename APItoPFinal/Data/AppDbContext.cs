using APItoPFinal.Models;
using Microsoft.EntityFrameworkCore;
namespace APItoPFinal.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Instrumento> Instrumentos { get; set; }

        public DbSet<Categoria> Categorias { get; set; }

        public DbSet<Marca> Marcas { get; set; }

        public DbSet<Compra> Compras { get; set; }
    }
}
