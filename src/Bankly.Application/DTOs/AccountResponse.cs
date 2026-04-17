using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record AccountResponse(
    Guid id,
    Guid userId,
    Guid accountTypeId,
    string branch,
    string accountNumber,
    decimal balance
)
{
   
    public static AccountResponse FromDomain(Account account) => new(
        account.Id,
        account.UserId,
        account.AccountTypeId,
        account.Branch,          
        account.AccountNumber, 
        account.Balance 
    );
}