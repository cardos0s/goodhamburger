using AwesomeAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using Xunit;

namespace GoodHamburger.Tests.Domain.Services;

public class DescontoTests
{
    private static Produto Sanduiche() => new("X Burger", 5.00m, CategoriaProduto.Sanduiche);
    private static Produto Batata() => new("Batata", 2.00m, CategoriaProduto.Batata);
    private static Produto Refri() => new("Refri", 2.50m, CategoriaProduto.Refrigerante);

    public class ComboCompleto
    {
        private readonly Pedido _pedido = new();

        public ComboCompleto()
        {
            _pedido.AdicionarItem(Sanduiche());
            _pedido.AdicionarItem(Batata());
            _pedido.AdicionarItem(Refri());
        }
        
        [Fact] public void Aplica_20_porcento() => _pedido.Desconto.Should().Be(1.90m);
        [Fact] public void Total_desconta_do_subtotal() => _pedido.Total.Should().Be(7.60m);
    }

    public class SanduicheERefrigerante
    {
        private readonly Pedido _pedido = new();

        public SanduicheERefrigerante()
        {
            _pedido.AdicionarItem(Sanduiche());
            _pedido.AdicionarItem(Refri());
        }

      
        [Fact] public void Aplica_15_porcento() => _pedido.Desconto.Should().Be(1.13m);
        [Fact] public void Total_desconta_do_subtotal() => _pedido.Total.Should().Be(6.37m);
    }

    public class SanduicheEBatata
    {
        private readonly Pedido _pedido = new();

        public SanduicheEBatata()
        {
            _pedido.AdicionarItem(Sanduiche());
            _pedido.AdicionarItem(Batata());
        }

        [Fact] public void Aplica_10_porcento() => _pedido.Desconto.Should().Be(0.70m);
        [Fact] public void Total_desconta_do_subtotal() => _pedido.Total.Should().Be(6.30m);
    }

    public class SemDesconto
    {
        [Fact]
        public void Pedido_vazio()
            => new Pedido().Desconto.Should().Be(0);

        [Fact]
        public void So_sanduiche()
        {
            var p = new Pedido();
            p.AdicionarItem(Sanduiche());

            p.Desconto.Should().Be(0);
        }

        [Fact]
        public void So_batata()
        {
            var p = new Pedido();
            p.AdicionarItem(Batata());

            p.Desconto.Should().Be(0);
        }

        [Fact]
        public void So_refrigerante()
        {
            var p = new Pedido();
            p.AdicionarItem(Refri());

            p.Desconto.Should().Be(0);
        }

        [Fact]
        public void Batata_e_refrigerante_sem_sanduiche()
        {
            var p = new Pedido();
            p.AdicionarItem(Batata());
            p.AdicionarItem(Refri());

            p.Desconto.Should().Be(0);
        }
    }
}