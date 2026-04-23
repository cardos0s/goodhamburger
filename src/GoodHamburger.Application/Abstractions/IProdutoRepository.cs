using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Abstractions;

public interface IProdutoRepository
{
    Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct = default);
    Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Produto>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default);
}