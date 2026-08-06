using System;
using Bankly.Domain.Commom;

namespace Bankly.Domain.Entities;

public class Card : BaseEntity
{
    public Guid AccountId { get; private set; }
    public string CardNumber { get; private set; } = string.Empty;
    public string Cvv { get; private set; } = string.Empty;
    public DateTime ExpirationDate { get; private set; }
    public char IsActive { get; private set; }

    // Relacionamento
    public Account Account { get; private set; }

    protected Card() { }

    public Card(Guid accountId, string cardNumber, string cvv, DateTime expirationDate, char isActive = 'Y')
    {
        if (string.IsNullOrWhiteSpace(cardNumber)) throw new DomainException("Número do cartão inválido.");
        if (expirationDate < DateTime.Now) throw new DomainException("Data de expiração não pode estar no passado.");

        AccountId = accountId;
        CardNumber = cardNumber;
        Cvv = cvv;
        ExpirationDate = expirationDate;
        IsActive = isActive;
    }

    public void ActivateCard() => IsActive = 'Y';
    public void DeactivateCard() => IsActive = 'N';
}