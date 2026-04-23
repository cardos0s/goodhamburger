namespace GoodHamburger.Web.Models;

public enum CategoriaProduto
{
    Sanduiche = 1,
    Batata = 2,
    Refrigerante = 3
}

public record ProdutoDto(int Id, string Nome, decimal Preco, CategoriaProduto Categoria)
{
    public string ImagemUrl => Id switch
    {
        1 => "images/x-burger.jpg",
        2 => "images/x-egg.jpg",
        3 => "images/x-bacon.jpg",
        4 => "images/batata.jpg",
        5 => "images/refri.jpg",
        _ => "images/placeholder.jpg"
    };

    public string CategoriaLabel => Categoria switch
    {
        CategoriaProduto.Sanduiche => "Hambúrgueres",
        CategoriaProduto.Batata => "Acompanhamentos",
        CategoriaProduto.Refrigerante => "Bebidas",
        _ => "Outros"
    };
}

public record ItemPedidoDto(int ProdutoId, string NomeProduto, decimal Preco);

public record PedidoDto(
    int Id,
    DateTime CriadoEm,
    IReadOnlyList<ItemPedidoDto> Itens,
    decimal Subtotal,
    decimal Desconto,
    decimal Total);

public record CriarPedidoRequest(IReadOnlyList<int> ProdutoIds);

public record AtualizarPedidoRequest(IReadOnlyList<int> ProdutoIds);

public record ErroApi(string Erro);