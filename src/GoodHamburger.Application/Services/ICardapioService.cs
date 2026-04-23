using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Services;

public interface ICardapioService
{
    Task<IReadOnlyList<ProdutoDto>> ObterCardapioAsync(CancellationToken ct = default);
}