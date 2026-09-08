using Bankly.Domain.Commom;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bankly.Api.Exceptions;

/// <summary>
/// Trata globalmente as exceções não capturadas pelos controllers e as
/// converte em respostas padronizadas (RFC 7807 - ProblemDetails).
/// </summary>
public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _environment;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IWebHostEnvironment environment)
    {
        _logger = logger;
        _environment = environment;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        _logger.LogError(exception,
            "Erro não tratado. TraceId: {TraceId}, Path: {Path}, Mensagem: {Message}",
            traceId, httpContext.Request.Path, exception.Message);

        var (statusCode, title) = exception switch
        {
            DomainException => (StatusCodes.Status400BadRequest, "Erro de validação"),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor")
        };

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError
                ? "Ocorreu um erro inesperado. Tente novamente mais tarde."
                : exception.Message,
            Type = $"https://httpstatuses.com/{statusCode}"
        };

        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["traceId"] = traceId;
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}