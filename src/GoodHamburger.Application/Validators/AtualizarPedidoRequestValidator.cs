using FluentValidation;
using GoodHamburger.Application.DTOs;

namespace GoodHamburger.Application.Validators;

public class AtualizarPedidoRequestValidator : AbstractValidator<AtualizarPedidoRequest>
{
    public AtualizarPedidoRequestValidator()
    {
        RuleFor(x => x.ProdutoIds)
            .NotNull().WithMessage("A lista de produtos é obrigatória.")
            .NotEmpty().WithMessage("O pedido deve conter ao menos um item.");

        RuleForEach(x => x.ProdutoIds)
            .GreaterThan(0).WithMessage("IDs de produto devem ser positivos.");
    }
}