using System.Text.Json;
using FluentValidation;
using GoodHamburger.Application.Exceptions;
using GoodHamburger.Domain.Exceptions;

namespace GoodHamburger.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ValidationException ex)
        {
            await EscreverRespostaAsync(context, StatusCodes.Status400BadRequest, new
            {
                erro = "Validação falhou.",
                detalhes = ex.Errors.Select(e => new { campo = e.PropertyName, mensagem = e.ErrorMessage })
            });
        }
        catch (RecursoNaoEncontradoException ex)
        {
            await EscreverRespostaAsync(context, StatusCodes.Status404NotFound, new { erro = ex.Message });
        }
        catch (DomainException ex)
        {
            await EscreverRespostaAsync(context, StatusCodes.Status400BadRequest, new { erro = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro não tratado em {Path}", context.Request.Path);
            await EscreverRespostaAsync(context, StatusCodes.Status500InternalServerError, new
            {
                erro = "Erro interno do servidor."
            });
        }
    }

    private static Task EscreverRespostaAsync(HttpContext context, int statusCode, object body)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var json = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        return context.Response.WriteAsync(json);
    }
}