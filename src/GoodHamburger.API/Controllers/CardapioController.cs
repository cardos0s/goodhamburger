using GoodHamburger.Application.DTOs;
using GoodHamburger.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoodHamburger.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CardapioController : ControllerBase
{
    private readonly ICardapioService _cardapio;

    public CardapioController(ICardapioService cardapio) => _cardapio = cardapio;

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ProdutoDto>), StatusCodes.Status200OK)]
    public async Task<IReadOnlyList<ProdutoDto>> Listar(CancellationToken ct)
        => await _cardapio.ObterCardapioAsync(ct);
}