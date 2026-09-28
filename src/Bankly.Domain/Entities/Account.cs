using System;
using System.Collections.Generic;
using Bankly.Domain.Commom;
using Bankly.Domain.Enums;

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

    /// <summary>
    /// Aplica uma transação (depósito, saque ou transferência de saída)
    /// no saldo da conta, validando saldo suficiente quando necessário.
    /// </summary>
    public void ApplyTransaction(Transaction transaction)
    {
        switch (transaction.Type)
        {
            case TransactionTypeEnum.DEPOSITO:
                Balance += transaction.Amount;
                break;

            case TransactionTypeEnum.SAQUE:
            case TransactionTypeEnum.TRANSFERENCIA:
                if (Balance < transaction.Amount)
                    throw new DomainException("Saldo insuficiente para realizar a operação.");

                Balance -= transaction.Amount;
                break;

            default:
                throw new DomainException("Tipo de transação inválido.");
        }
    }
}