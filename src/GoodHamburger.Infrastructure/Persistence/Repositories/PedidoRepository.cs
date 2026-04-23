using GoodHamburger.Application.Abstractions;
using GoodHamburger.Domain.Entities;
using GoodHamburger.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GoodHamburger.Infrastructure.Repositories;

public class PedidoRepository : IPedidoRepository
{
    private readonly AppDbContext _db;

    public PedidoRepository(AppDbContext db) => _db = db;

    public async Task<Pedido?> ObterPorIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Pedido>> ListarAsync(CancellationToken ct = default)
    {
        return await _db.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(p => p.CriadoEm)
            .ToListAsync(ct);
    }

    public Task AdicionarAsync(Pedido pedido, CancellationToken ct = default)
    {
        _db.Pedidos.Add(pedido);
        return Task.CompletedTask;
    }

    public Task RemoverAsync(Pedido pedido, CancellationToken ct = default)
    {
        _db.Pedidos.Remove(pedido);
        return Task.CompletedTask;
    }

    public Task SalvarAsync(CancellationToken ct = default) => _db.SaveChangesAsync(ct);
}