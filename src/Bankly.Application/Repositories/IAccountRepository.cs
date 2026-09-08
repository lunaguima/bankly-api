using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public interface IAccountRepository : IGenericRepository<Account>
{
    Account? GetAccountWithDetails(Guid accountId);
    IEnumerable<Account> GetAccountsByUserId(Guid userId);
}