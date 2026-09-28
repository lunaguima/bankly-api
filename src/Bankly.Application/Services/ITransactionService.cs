using Bankly.Application.DTOs;
using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public interface ITransactionService
{
    Transaction Create(TransactionRequest request);

    /// <summary>Lista completa (contrato da v1, deprecada).</summary>
    IEnumerable<Transaction> GetAll();

    /// <summary>Lista paginada (contrato da v2). Lança DomainException se page/pageSize forem inválidos.</summary>
    PagedResponse<TransactionResponse> GetPaged(int page, int pageSize);
}