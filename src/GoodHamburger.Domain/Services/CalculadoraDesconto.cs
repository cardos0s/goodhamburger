using GoodHamburger.Domain.Entities;
using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Domain.Services;

public static class CalculadoraDesconto
{
    public static decimal Calcular(Pedido pedido)
    {
        ArgumentNullException.ThrowIfNull(pedido);

        var categorias = pedido.Itens
            .Select(i => i.Produto.Categoria)
            .ToHashSet();

        var temSanduiche    = categorias.Contains(CategoriaProduto.Sanduiche);
        var temBatata       = categorias.Contains(CategoriaProduto.Batata);
        var temRefrigerante = categorias.Contains(CategoriaProduto.Refrigerante);

        decimal percentual = (temSanduiche, temBatata, temRefrigerante) switch
        {
            (true, true,  true ) => 0.20m,
            (true, false, true ) => 0.15m,
            (true, true,  false) => 0.10m,
            _                    => 0.00m
        };

        return Math.Round(pedido.Subtotal * percentual, 2, MidpointRounding.AwayFromZero);
    }
}