using FluentValidation;
using GoodHamburger.Application.Services;
using GoodHamburger.Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace GoodHamburger.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICardapioService, CardapioService>();
        services.AddScoped<IPedidoService, PedidoService>();

        services.AddValidatorsFromAssemblyContaining<CriarPedidoRequestValidator>();

        return services;
    }
}