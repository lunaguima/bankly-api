using Bankly.Application.DTOs;
using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IAccountRepository _accountRepository;

    public TransactionService(
        ITransactionRepository transactionRepository,
        IAccountRepository accountRepository)
    {
        _transactionRepository = transactionRepository;
        _accountRepository = accountRepository;
    }

    /// <summary>
    /// Registra uma nova transação em uma conta existente, atualizando o saldo.
    /// Lança KeyNotFoundException se a conta não existir (não persiste nada nesse caso).
    /// </summary>
    public Transaction Create(TransactionRequest request)
    {
        var account = _accountRepository.GetById(request.accountId);
        if (account == null)
            throw new KeyNotFoundException("Conta não encontrada.");

        var transaction = request.ToDomain();

        account.ApplyTransaction(transaction);

        _accountRepository.Update(account);
        _transactionRepository.Add(transaction);

        return transaction;
    }
}