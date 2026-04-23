using System.Net;
using System.Net.Http.Json;
using GoodHamburger.Web.Models;

namespace GoodHamburger.Web.Services;

public class GoodHamburgerApi
{
    private readonly HttpClient _http;

    public GoodHamburgerApi(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<ProdutoDto>> ObterCardapioAsync()
    {
        var produtos = await _http.GetFromJsonAsync<List<ProdutoDto>>("api/cardapio");
        return produtos ?? new List<ProdutoDto>();
    }

    public async Task<IReadOnlyList<PedidoDto>> ListarPedidosAsync()
    {
        var pedidos = await _http.GetFromJsonAsync<List<PedidoDto>>("api/pedidos");
        return pedidos ?? new List<PedidoDto>();
    }

    public async Task<(PedidoDto? Pedido, string? Erro)> CriarPedidoAsync(IReadOnlyList<int> produtoIds)
    {
        var response = await _http.PostAsJsonAsync("api/pedidos", new CriarPedidoRequest(produtoIds));

        if (response.IsSuccessStatusCode)
        {
            var pedido = await response.Content.ReadFromJsonAsync<PedidoDto>();
            return (pedido, null);
        }

        var erro = await LerMensagemErroAsync(response);
        return (null, erro);
    }

    public async Task<bool> RemoverPedidoAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/pedidos/{id}");
        return response.IsSuccessStatusCode;
    }

    private static async Task<string> LerMensagemErroAsync(HttpResponseMessage response)
    {
        try
        {
            var erro = await response.Content.ReadFromJsonAsync<ErroApi>();
            return erro?.Erro ?? "Erro desconhecido.";
        }
        catch
        {
            return $"Erro HTTP {(int)response.StatusCode}.";
        }
    }
}