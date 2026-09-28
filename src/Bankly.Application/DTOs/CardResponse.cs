using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;

public record CardResponse(
    Guid Id,
    Guid AccountId,
    string CardNumber,
    DateTime ExpirationDate,
    char IsActive)
{
    public static CardResponse FromDomain(Card c) =>
        new CardResponse(
            c.Id,
            c.AccountId,
            Mask(c.CardNumber),
            c.ExpirationDate,
            c.IsActive
        );

    private static string Mask(string cardNumber)
    {
        if (string.IsNullOrEmpty(cardNumber) || cardNumber.Length <= 4)
            return cardNumber;

        return new string('*', cardNumber.Length - 4) + cardNumber[^4..];
    }
}