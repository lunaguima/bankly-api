using System;

namespace Bankly.Domain.Commom;

/// <summary>
/// Exceção lançada quando uma regra de negócio do domínio é violada
/// (ex: campo obrigatório vazio, valor inválido). Mapeada para HTTP 400
/// pelo GlobalExceptionHandler.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}