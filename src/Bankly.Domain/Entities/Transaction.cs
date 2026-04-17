using System;
using Bankly.Domain.Commom;
using Bankly.Domain.Enums;

namespace Bankly.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid AccountId { get; private set; }
    public decimal Amount { get; private set; }
    public TransactionTypeEnum Type { get; private set; }

    // Relacionamento
    public Account Account { get; private set; }

    protected Transaction() { }

    public Transaction(Guid accountId, decimal amount, TransactionTypeEnum type)
    {
        if (amount <= 0) throw new Exception("O valor da transação deve ser maior que zero.");

        AccountId = accountId;
        Amount = amount;
        Type = type;
        
    }
}