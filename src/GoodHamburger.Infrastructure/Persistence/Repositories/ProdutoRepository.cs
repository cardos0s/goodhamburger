using GoodHamburger.Application.Abstractions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _db;

    public ProdutoRepository(AppDbContext db) => _db = db;

    public async Task<IReadOnlyList<Produto>> ListarAsync(CancellationToken ct = default)
        => await _db.Produtos.AsNoTracking().OrderBy(p => p.Id).ToListAsync(ct);

    public async Task<Produto?> ObterPorIdAsync(int id, CancellationToken ct = default)
        => await _db.Produtos.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<Produto>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken ct = default)
    {
        var idsSet = ids.ToHashSet();
        return await _db.Produtos
            .Where(p => idsSet.Contains(p.Id))
            .ToListAsync(ct);
    }
}