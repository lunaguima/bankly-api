using Bankly.Application.Services;
using Bankly.Domain.Entities;
using Bankly.Infrastructure.Persistence;

namespace Bankly.Infrastructure.Repositories;

public sealed class AccountTypeRepository(BanklyContext context) : GenericRepository<AccountType>(context), IAccountTypeRepository { }