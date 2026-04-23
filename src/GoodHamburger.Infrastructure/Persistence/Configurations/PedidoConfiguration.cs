using GoodHamburger.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoodHamburger.Infrastructure.Persistence.Configurations;

public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Pedidos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.CriadoEm).IsRequired();

      
        builder.Ignore(p => p.Subtotal);
        builder.Ignore(p => p.Desconto);
        builder.Ignore(p => p.Total);

        
        builder.HasMany(p => p.Itens)
            .WithOne()
            .HasForeignKey(i => i.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(p => p.Itens)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_itens");
    }
}