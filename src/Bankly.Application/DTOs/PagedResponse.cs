namespace Bankly.Application.DTOs;

public record PagedResponse<T>(
    int page,
    int pageSize,
    int totalItems,
    int totalPages,
    IReadOnlyList<T> items,
    bool hasPrevious,
    bool hasNext
)
{
    public static PagedResponse<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalItems)
    {
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        return new PagedResponse<T>(
            page,
            pageSize,
            totalItems,
            totalPages,
            items,
            hasPrevious: page > 1,
            hasNext: page < totalPages);
    }
}