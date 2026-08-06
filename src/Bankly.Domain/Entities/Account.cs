using System;
using System.Collections.Generic;
using Bankly.Domain.Commom;

namespace Bankly.Domain.Entities;

public class Account : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid AccountTypeId { get; private set; }
    public string Branch { get; private set; } = string.Empty;
    public string AccountNumber { get; private set; } = string.Empty;
    public decimal Balance { get; private set; }

    // Relacionamentos
    public User User { get; private set; }
    public AccountType AccountType { get; private set; }
    public List<Card> Cards { get; private set; } = [];
    public List<Transaction> Transactions { get; private set; } = [];

    protected Account() { }

    public Account(Guid userId, Guid accountTypeId, string branch, string accountNumber, decimal balance = 0)
    {
        if (userId == Guid.Empty) throw new DomainException("Id do usuário é obrigatório.");
        if (balance < 0) throw new DomainException("O saldo inicial não pode ser negativo.");

        UserId = userId;
        AccountTypeId = accountTypeId;
        Branch = branch;
        AccountNumber = accountNumber;
        Balance = balance;
    }

    public void UpdateDetails(string newBranch, Guid newAccountTypeId)
    {
        Branch = newBranch;
        AccountTypeId = newAccountTypeId;
    }
}