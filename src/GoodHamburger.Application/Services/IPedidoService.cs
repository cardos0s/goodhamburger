using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Services;

public interface IPedidoService
{
    Task<PedidoDto> CriarAsync(CriarPedidoRequest request, CancellationToken ct = default);
    Task<PedidoDto> AtualizarAsync(int id, AtualizarPedidoRequest request, CancellationToken ct = default);
    Task<PedidoDto> ObterPorIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<PedidoDto>> ListarAsync(CancellationToken ct = default);
    Task RemoverAsync(int id, CancellationToken ct = default);
}