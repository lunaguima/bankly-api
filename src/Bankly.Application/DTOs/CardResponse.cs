using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;

public record CardResponse(
    Guid Id,
    Guid AccountId,
    string CardNumber,
    string Cvv,
    DateTime ExpirationDate,
    char IsActive)
{
    public static CardResponse FromDomain(Card c) => 
        new CardResponse(
            c.Id, 
            c.AccountId, 
            c.CardNumber, 
            c.Cvv, 
            c.ExpirationDate, 
            c.IsActive
        );
}