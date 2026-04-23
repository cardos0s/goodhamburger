namespace GoodHamburger.Domain.Entities;

public class ItemPedido
{
    public int Id { get; private set; }
    public int PedidoId { get; private set; }
    public int ProdutoId { get; private set; }
    public Produto Produto { get; private set; } = null!;

    private ItemPedido() { }

    public ItemPedido(Produto produto)
    {
        ArgumentNullException.ThrowIfNull(produto);
        Produto = produto;
        ProdutoId = produto.Id;
    }
}