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

        public DbSet<CompraItem> CompraItens { get; set; }

        public DbSet<MovimentoEstoque> MovimentosEstoque { get; set; }

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
                e.HasOne(i => i.Categoria).WithMany(c => c.Instrumentos).HasForeignKey(i => i.IdCategoria).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(i => i.Marca).WithMany(m => m.Instrumentos).HasForeignKey(i => i.IdMarca).OnDelete(DeleteBehavior.Restrict);
                e.Property(i => i.Preco).HasPrecision(18, 2); // Define a precisão do campo Preco como decimal(18,2)
                e.ToTable(t => t.HasCheckConstraint("CK_Instrumentos_Estoque", "[Estoque] >= 0"));
            });

            modelBuilder.Entity<Compra>(e =>
            {
                e.HasOne(c => c.Usuario)
                    .WithMany()
                    .HasForeignKey(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.Property(c => c.ValorTotal).HasPrecision(18, 2); // Define a precisão do campo ValorTotal como decimal(18,2)
            });

            modelBuilder.Entity<CompraItem>(e =>
            {
                // a chave é o par compra + instrumento: o mesmo instrumento aparece uma vez por compra, com a quantidade
                e.HasKey(ci => new { ci.CompraId, ci.InstrumentoId });

                e.Property(ci => ci.PrecoUnitario).HasPrecision(18, 2);

                // apagar uma compra apaga os itens dela
                e.HasOne(ci => ci.Compra)
                    .WithMany(c => c.Itens)
                    .HasForeignKey(ci => ci.CompraId)
                    .OnDelete(DeleteBehavior.Cascade);

                // mas não dá para apagar um instrumento que já está em alguma compra (histórico)
                e.HasOne(ci => ci.Instrumento)
                    .WithMany()
                    .HasForeignKey(ci => ci.InstrumentoId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.ToTable(t => t.HasCheckConstraint("CK_CompraItens_Quantidade", "[Quantidade] > 0"));
            });

            modelBuilder.Entity<MovimentoEstoque>(e =>
            {
                e.Property(m => m.Motivo).HasMaxLength(200);

                // apagar um instrumento que nunca foi vendido apaga o histórico dele
                e.HasOne<Instrumento>().WithMany().HasForeignKey(m => m.InstrumentoId).OnDelete(DeleteBehavior.Cascade);

                // apagar a compra ou o usuário preserva o movimento, só perde o vínculo
                e.HasOne<Compra>().WithMany().HasForeignKey(m => m.CompraId).OnDelete(DeleteBehavior.SetNull);
                e.HasOne<Usuario>().WithMany().HasForeignKey(m => m.UsuarioId).OnDelete(DeleteBehavior.SetNull);

                e.HasIndex(m => new { m.InstrumentoId, m.DataHora });

                e.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_MovimentosEstoque_Variacao", "[Variacao] <> 0");
                    t.HasCheckConstraint("CK_MovimentosEstoque_EstoqueApos", "[EstoqueApos] >= 0");
                });
            });
        }
    }
}
