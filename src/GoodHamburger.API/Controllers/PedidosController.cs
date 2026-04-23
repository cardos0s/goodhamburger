using FluentValidation;
using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PedidosController : ControllerBase
{
    private readonly IPedidoService _pedidos;
    private readonly IValidator<CriarPedidoRequest> _criarValidator;
    private readonly IValidator<AtualizarPedidoRequest> _atualizarValidator;

    public PedidosController(
        IPedidoService pedidos,
        IValidator<CriarPedidoRequest> criarValidator,
        IValidator<AtualizarPedidoRequest> atualizarValidator)
    {
        _pedidos = pedidos;
        _criarValidator = criarValidator;
        _atualizarValidator = atualizarValidator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PedidoDto>), StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<PedidoDto>> Listar(CancellationToken ct)
        => await _pedidos.ListarAsync(ct);

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<PedidoDto> Obter(int id, CancellationToken ct)
        => await _pedidos.ObterPorIdAsync(id, ct);

    [HttpPost]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PedidoDto>> Criar(CriarPedidoRequest request, CancellationToken ct)
    {
        await _criarValidator.ValidateAndThrowAsync(request, ct);
        var pedido = await _pedidos.CriarAsync(request, ct);
        return CreatedAtAction(nameof(Obter), new { id = pedido.Id }, pedido);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PedidoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<PedidoDto> Atualizar(int id, AtualizarPedidoRequest request, CancellationToken ct)
    {
        await _atualizarValidator.ValidateAndThrowAsync(request, ct);
        return await _pedidos.AtualizarAsync(id, request, ct);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken ct)
    {
        await _pedidos.RemoverAsync(id, ct);
        return NoContent();
    }
}