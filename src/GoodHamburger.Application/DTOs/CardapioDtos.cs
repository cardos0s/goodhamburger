using GoodHamburger.Domain.Enums;

namespace GoodHamburger.Application.DTOs;

public record ProdutoDto(int Id, string Nome, decimal Preco, CategoriaProduto Categoria);