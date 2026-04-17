using Bankly.Application.Services;
using Bankly.Domain.Entities;
using Bankly.Infrastructure.Persistence;

namespace Bankly.Infrastructure.Repositories;

public sealed class AddressRepository(BanklyContext context) : GenericRepository<Address>(context), IAddressRepository { }