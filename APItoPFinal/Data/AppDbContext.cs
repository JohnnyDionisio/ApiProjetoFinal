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

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>(e =>
            {
                e.Property(u => u.Nome).HasMaxLength(100);
                e.Property(u => u.NomeUsuario).HasMaxLength(30);
                e.Property(u => u.Email).HasMaxLength(150);
                e.Property(u => u.CPF).HasMaxLength(11);
                e.Property(u => u.Cargo).HasMaxLength(20);

                e.HasIndex(u => u.CPF).IsUnique();
                e.HasIndex(u => u.NomeUsuario).IsUnique();
                e.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Instrumento>(e =>
            {
                e.Property(i => i.Identificação).HasMaxLength(50);
                e.HasIndex(i => i.Identificação).IsUnique();
            })
        }
    }
}
