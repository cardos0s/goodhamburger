using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.Persistence.Configurations;

public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.ToTable("Produtos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Preco)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(p => p.Categoria)
            .IsRequired()
            .HasConversion<int>();

        
        builder.HasData(
            new { Id = 1, Nome = "X Burger",     Preco = 5.00m, Categoria = CategoriaProduto.Sanduiche },
            new { Id = 2, Nome = "X Egg",        Preco = 4.50m, Categoria = CategoriaProduto.Sanduiche },
            new { Id = 3, Nome = "X Bacon",      Preco = 7.00m, Categoria = CategoriaProduto.Sanduiche },
            new { Id = 4, Nome = "Batata frita", Preco = 2.00m, Categoria = CategoriaProduto.Batata },
            new { Id = 5, Nome = "Refrigerante", Preco = 2.50m, Categoria = CategoriaProduto.Refrigerante }
        );
    }
}