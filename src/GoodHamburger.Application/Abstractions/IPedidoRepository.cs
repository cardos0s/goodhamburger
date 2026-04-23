using GoodHamburger.Domain.Entities;

namespace GoodHamburger.Application.Abstractions;

public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Pedido>> ListarAsync(CancellationToken ct = default);
    Task AdicionarAsync(Pedido pedido, CancellationToken ct = default);
    Task RemoverAsync(Pedido pedido, CancellationToken ct = default);
    Task SalvarAsync(CancellationToken ct = default);
}