using System.ComponentModel.DataAnnotations;
using Bankly.Domain.Entities;
using System;

namespace Bankly.Application.DTOs;

public record CardRequest(
    [Required] Guid AccountId,
    [Required] string CardNumber,
    [Required] string Cvv,
    [Required] DateTime ExpirationDate,
    [Required] bool IsActive 
)
{
    public Card ToDomain() => new Card(
        this.AccountId,
        this.CardNumber,
        this.Cvv,
        this.ExpirationDate,
        this.IsActive ? 'S' : 'N' 
    );
}