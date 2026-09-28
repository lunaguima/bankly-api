using Bankly.Application.Services;
using Bankly.Domain.Entities;
using Bankly.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bankly.Infrastructure.Repositories;

public sealed class TransactionRepository(BanklyContext context)
    : GenericRepository<Transaction>(context), ITransactionRepository
{
    public (IReadOnlyList<Transaction> Items, int TotalItems) GetPaged(int page, int pageSize)
    {
        var totalItems = _dbSet.Count();

        // Evita overflow de int quando page é gigante: página além do total = lista vazia
        var skip = (long)(page - 1) * pageSize;
        if (skip >= totalItems)
            return (Array.Empty<Transaction>(), totalItems);

        var items = _dbSet
            .AsNoTracking()
            .OrderBy(t => t.CreatedAt)
            .ThenBy(t => t.Id)          // desempate: páginas nunca se sobrepõem
            .Skip((int)skip)
            .Take(pageSize)
            .ToList();                  // só materializa depois do Skip/Take

        return (items, totalItems);
    }
}