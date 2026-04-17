using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record TransactionResponse(
    Guid id,
    Guid accountId,
    decimal amount,
    string type, 
    DateTime createdAt
)
{
 
    public static TransactionResponse FromDomain(Transaction t) => new(
        t.Id, 
        t.AccountId, 
        t.Amount, 
        t.Type.ToString(), 
        t.CreatedAt        
    );
}