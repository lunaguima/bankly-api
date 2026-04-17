using System;
using System.Collections.Generic;
using Bankly.Domain.Commom;

namespace Bankly.Domain.Entities;

public class AccountType : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    // Relacionamento
    public List<Account> Accounts { get; private set; } = [];

   
    protected AccountType() { }

    public AccountType(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new Exception("O nome do tipo de conta é obrigatório.");
        
        Name = name;
    }

  
    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName)) throw new Exception("O nome do tipo de conta é obrigatório.");
        Name = newName;
    }
}