using Bankly.Application.DTOs;
using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public interface ITransactionService
{
    Transaction Create(TransactionRequest request);
}