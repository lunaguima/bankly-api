using System;
using Bankly.Domain.Entities;

namespace Bankly.Application.DTOs;


public record AccountTypeResponse(Guid id, string name)
{
  
    public static AccountTypeResponse FromDomain(AccountType type) => new(
        type.Id, 
        type.Name
    );
}