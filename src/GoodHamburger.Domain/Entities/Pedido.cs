using GoodHamburger.Domain.Enums;
using GoodHamburger.Domain.Exceptions;
using GoodHamburger.Domain.Services;

namespace GoodHamburger.Domain.Entities;

public class Pedido
{
    private readonly List<ItemPedido> _itens = new();

    public int Id { get; private set; }
    public DateTime CriadoEm { get; private set; } = DateTime.UtcNow;

    public IReadOnlyList<ItemPedido> Itens => _itens.AsReadOnly();

    public decimal Subtotal => _itens.Sum(i => i.Produto.Preco);
    public decimal Desconto => CalculadoraDesconto.Calcular(this);
    public decimal Total => Subtotal - Desconto;

    public Pedido() { }

    public void AdicionarItem(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);

        if (_itens.Any(i => i.Produto.Categoria == produto.Categoria))
            throw new PedidoInvalidoException(
                $"O pedido já contém um item da categoria '{produto.Categoria}'. " +
                "Cada pedido pode conter apenas um sanduíche, uma batata e um refrigerante.");

        _itens.Add(new ItemPedido(produto));
    }

    public void RemoverItem(CategoriaProduto categoria)
    {
        var item = _itens.FirstOrDefault(i => i.Produto.Categoria == categoria);
        if (item is null)
            throw new PedidoInvalidoException(
                $"Não há item da categoria '{categoria}' neste pedido.");

        _itens.Remove(item);
    }

    public void SubstituirItens(IEnumerable<Produto> produtos)
    {
        ArgumentNullException.ThrowIfNull(produtos);
        _itens.Clear();
        foreach (var produto in produtos)
            AdicionarItem(produto);
    }
}