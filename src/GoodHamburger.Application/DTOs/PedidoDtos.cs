namespace GoodHamburger.Application.DTOs;

public record CriarPedidoRequest(IReadOnlyList<int> ProdutoIds);

public record AtualizarPedidoRequest(IReadOnlyList<int> ProdutoIds);

public record PedidoDto(
    int Id,
    DateTime CriadoEm,
    IReadOnlyList<ItemPedidoDto> Itens,
    decimal Subtotal,
    decimal Desconto,
    decimal Total);