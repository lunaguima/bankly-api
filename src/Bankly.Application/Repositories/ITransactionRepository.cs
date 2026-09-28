using Bankly.Domain.Entities;

namespace Bankly.Application.Services;

public interface ITransactionRepository : IGenericRepository<Transaction>
{
    /// <summary>
    /// Devolve uma página de transações e o total de registros.
    /// O corte (Skip/Take) é feito no banco.
    /// </summary>
    (IReadOnlyList<Transaction> Items, int TotalItems) GetPaged(int page, int pageSize);
}