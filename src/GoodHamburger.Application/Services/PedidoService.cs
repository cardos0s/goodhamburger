using GoodHamburger.Application.Abstractions;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Services;

public class PedidoService : IPedidoService
{
    private readonly IPedidoRepository _pedidos;
    private readonly IProdutoRepository _produtos;

    public PedidoService(IPedidoRepository pedidos, IProdutoRepository produtos)
    {
        _pedidos = pedidos;
        _produtos = produtos;
    }

    public async Task<PedidoDto> CriarAsync(CriarPedidoRequest request, CancellationToken ct = default)
    {
        var produtos = await CarregarProdutosAsync(request.ProdutoIds, ct);

        var pedido = new Pedido();
        foreach (var produto in produtos)
            pedido.AdicionarItem(produto);

        await _pedidos.AdicionarAsync(pedido, ct);
        await _pedidos.SalvarAsync(ct);

        return Mapear(pedido);
    }

    public async Task<PedidoDto> AtualizarAsync(int id, AtualizarPedidoRequest request, CancellationToken ct = default)
    {
        var pedido = await _pedidos.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException($"Pedido {id} não encontrado.");

        var produtos = await CarregarProdutosAsync(request.ProdutoIds, ct);
        pedido.SubstituirItens(produtos);

        await _pedidos.SalvarAsync(ct);
        return Mapear(pedido);
    }

    public async Task<PedidoDto> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        var pedido = await _pedidos.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException($"Pedido {id} não encontrado.");

        return Mapear(pedido);
    }

    public async Task<IReadOnlyList<PedidoDto>> ListarAsync(CancellationToken ct = default)
    {
        var pedidos = await _pedidos.ListarAsync(ct);
        return pedidos.Select(Mapear).ToList();
    }

    public async Task RemoverAsync(int id, CancellationToken ct = default)
    {
        var pedido = await _pedidos.ObterPorIdAsync(id, ct)
            ?? throw new RecursoNaoEncontradoException($"Pedido {id} não encontrado.");

        await _pedidos.RemoverAsync(pedido, ct);
        await _pedidos.SalvarAsync(ct);
    }

    private async Task<List<Produto>> CarregarProdutosAsync(IReadOnlyList<int> ids, CancellationToken ct)
    {
        var produtos = await _produtos.ObterPorIdsAsync(ids, ct);

        // Preserva a ordem enviada pelo cliente
        var porId = produtos.ToDictionary(p => p.Id);
        var resultado = new List<Produto>();

        foreach (var id in ids)
        {
            if (!porId.TryGetValue(id, out var produto))
                throw new RecursoNaoEncontradoException($"Produto {id} não encontrado no cardápio.");
            resultado.Add(produto);
        }

        return resultado;
    }

    private static PedidoDto Mapear(Pedido p) => new(
        p.Id,
        p.CriadoEm,
        p.Itens.Select(i => new ItemPedidoDto(i.ProdutoId, i.Produto.Nome, i.Produto.Preco)).ToList(),
        p.Subtotal,
        p.Desconto,
        p.Total);
}