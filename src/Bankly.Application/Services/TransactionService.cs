using Bankly.Application.DTOs;
using Bankly.Domain.Commom;
using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public class TransactionService : ITransactionService
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

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

    public IEnumerable<Transaction> GetAll() => _transactionRepository.GetAll();

    public PagedResponse<TransactionResponse> GetPaged(int page, int pageSize)
    {
        if (page < 1)
            throw new DomainException("O parâmetro 'page' deve ser um inteiro maior ou igual a 1.");

        if (pageSize < 1 || pageSize > MaxPageSize)
            throw new DomainException($"O parâmetro 'pageSize' deve estar entre 1 e {MaxPageSize}.");

        var (items, totalItems) = _transactionRepository.GetPaged(page, pageSize);
        var responseItems = items.Select(TransactionResponse.FromDomain).ToList();

        return PagedResponse<TransactionResponse>.Create(responseItems, page, pageSize, totalItems);
    }
}