using AwesomeAssertions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;
using Xunit;

namespace GoodHamburger.Tests.Domain.Entities;

public class PedidoTests
{
    private static Produto Sanduiche() => new("X Burger", 5.00m, CategoriaProduto.Sanduiche);
    private static Produto OutroSanduiche() => new("X Egg", 4.50m, CategoriaProduto.Sanduiche);
    private static Produto Batata() => new("Batata", 2.00m, CategoriaProduto.Batata);
    private static Produto Refri() => new("Refri", 2.50m, CategoriaProduto.Refrigerante);

    public class RecemCriado
    {
        private readonly Pedido _pedido = new();

        [Fact] public void Nao_tem_itens() => _pedido.Itens.Should().BeEmpty();
        [Fact] public void Subtotal_eh_zero() => _pedido.Subtotal.Should().Be(0);
        [Fact] public void Desconto_eh_zero() => _pedido.Desconto.Should().Be(0);
        [Fact] public void Total_eh_zero() => _pedido.Total.Should().Be(0);
    }

    public class AoAdicionarItem
    {
        [Fact]
        public void Item_entra_na_lista()
        {
            var p = new Pedido();
            p.AdicionarItem(Sanduiche());

            p.Itens.Should().HaveCount(1);
        }

        [Fact]
        public void Produto_nulo_e_rejeitado()
        {
            var p = new Pedido();

            var acao = () => p.AdicionarItem(null!);

            acao.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Segundo_sanduiche_e_rejeitado()
        {
            var p = new Pedido();
            p.AdicionarItem(Sanduiche());

            var acao = () => p.AdicionarItem(OutroSanduiche());

            acao.Should().Throw<PedidoInvalidoException>()
                .WithMessage("*Sanduiche*");
        }

        [Fact]
        public void Segunda_batata_e_rejeitada()
        {
            var p = new Pedido();
            p.AdicionarItem(Batata());

            var acao = () => p.AdicionarItem(Batata());

            acao.Should().Throw<PedidoInvalidoException>();
        }

        [Fact]
        public void Segundo_refrigerante_e_rejeitado()
        {
            var p = new Pedido();
            p.AdicionarItem(Refri());

            var acao = () => p.AdicionarItem(Refri());

            acao.Should().Throw<PedidoInvalidoException>();
        }
    }

    public class AoRemoverItem
    {
        [Fact]
        public void Remove_item_da_categoria_indicada()
        {
            var p = new Pedido();
            p.AdicionarItem(Sanduiche());
            p.AdicionarItem(Batata());

            p.RemoverItem(CategoriaProduto.Batata);

            p.Itens.Should().ContainSingle()
                .Which.Produto.Categoria.Should().Be(CategoriaProduto.Sanduiche);
        }

        [Fact]
        public void Remover_categoria_ausente_lanca_excecao()
        {
            var p = new Pedido();
            p.AdicionarItem(Sanduiche());

            var acao = () => p.RemoverItem(CategoriaProduto.Refrigerante);

            acao.Should().Throw<PedidoInvalidoException>();
        }
    }

    public class AoSubstituirItens
    {
        [Fact]
        public void Lista_anterior_e_descartada()
        {
            var p = new Pedido();
            p.AdicionarItem(Sanduiche());

            p.SubstituirItens(new[] { Batata(), Refri() });

            p.Itens.Should().HaveCount(2);
            p.Itens.Should().NotContain(i => i.Produto.Categoria == CategoriaProduto.Sanduiche);
        }

        [Fact]
        public void Duplicata_na_substituicao_e_rejeitada()
        {
            var p = new Pedido();

            var acao = () => p.SubstituirItens(new[] { Sanduiche(), OutroSanduiche() });

            acao.Should().Throw<PedidoInvalidoException>();
        }
    }
}