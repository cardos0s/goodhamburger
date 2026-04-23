using GoodHamburger.Application.Abstractions;
using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Services;

public class CardapioService : ICardapioService
{
    private readonly IProdutoRepository _produtos;

    public CardapioService(IProdutoRepository produtos) => _produtos = produtos;

    public async Task<IReadOnlyList<ProdutoDto>> ObterCardapioAsync(CancellationToken ct = default)
    {
        var produtos = await _produtos.ListarAsync(ct);
        return produtos
            .Select(p => new ProdutoDto(p.Id, p.Nome, p.Preco, p.Categoria))
            .ToList();
    }
}