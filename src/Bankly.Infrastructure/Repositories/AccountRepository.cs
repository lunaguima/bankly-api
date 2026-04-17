using Bankly.Application.Services;
using Bankly.Domain.Entities;
using Bankly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bankly.Infrastructure.Repositories;

/// <summary>
/// Repositório para as operações de contas bancárias.
/// </summary>
public sealed class AccountRepository(BanklyContext context) 
    : GenericRepository<Account>(context), IAccountRepository
{
    public Account? GetAccountWithDetails(Guid accountId)
    {
        return _context.Accounts
            .Include(a => a.Cards)
            .Include(a => a.Transactions)
            .Include(a => a.AccountType)
            .FirstOrDefault(a => a.Id == accountId);
    }

    public IEnumerable<Account> GetAccountsByUserId(Guid userId)
    {
        return _context.Accounts
            .Where(a => a.UserId == userId)
            .Include(a => a.AccountType)
            .ToList();
    }
}