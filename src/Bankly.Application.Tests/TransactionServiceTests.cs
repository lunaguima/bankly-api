using Bankly.Application.Services;
using Bankly.Domain.Commom;
using Bankly.Domain.Entities;
using Bankly.Domain.Enums;
using Moq;
using Xunit;

namespace Bankly.Application.Tests;

public class TransactionServicePagingTests
{
    private sealed class FakeTransactionRepository : ITransactionRepository
    {
        private readonly List<Transaction> _data;

        public int GetPagedCalls { get; private set; }

        public FakeTransactionRepository(int total)
        {
            _data = Enumerable.Range(0, total)
                .Select(_ => new Transaction(Guid.NewGuid(), 10m, TransactionTypeEnum.DEPOSITO))
                .ToList();
        }

        public (IReadOnlyList<Transaction> Items, int TotalItems) GetPaged(int page, int pageSize)
        {
            GetPagedCalls++;
            var items = _data.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return (items, _data.Count);
        }

        public Transaction? GetById(Guid id) => _data.FirstOrDefault(t => t.Id == id);
        public IEnumerable<Transaction> GetAll() => _data;
        public void Add(Transaction entity) => _data.Add(entity);
        public void Update(Transaction entity) { }
        public void Delete(Transaction entity) => _data.Remove(entity);
        public void SaveChanges() { }
    }

    private static (TransactionService Service, FakeTransactionRepository Repo) CreateSut(int total)
    {
        var repo = new FakeTransactionRepository(total);
        var accountRepo = new Mock<IAccountRepository>().Object;
        return (new TransactionService(repo, accountRepo), repo);
    }

    [Theory]
    [InlineData(0, 20)]
    [InlineData(-1, 20)]
    [InlineData(1, 0)]
    [InlineData(1, -5)]
    [InlineData(1, 101)]
    [InlineData(1, 9999)]
    public void GetPaged_ParametrosInvalidos_DeveLancarDomainExceptionENaoConsultarRepositorio(int page, int pageSize)
    {
        // Arrange
        var (service, repo) = CreateSut(10);

        // Act & Assert
        Assert.Throws<DomainException>(() => service.GetPaged(page, pageSize));
        Assert.Equal(0, repo.GetPagedCalls);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 20)]
    [InlineData(2, 5)]
    [InlineData(1, 100)]
    [InlineData(100, 10)]
    public void GetPaged_ParametrosValidos_NaoDeveLancarExcecao(int page, int pageSize)
    {
        // Arrange
        var (service, _) = CreateSut(50);

        // Act
        var response = service.GetPaged(page, pageSize);

        // Assert
        Assert.Equal(page, response.page);
        Assert.Equal(pageSize, response.pageSize);
        Assert.Equal(50, response.totalItems);
    }

    [Fact]
    public void GetPaged_137Itens_PageSize20_DeveCalcularTotaisEFlagsCorretamente()
    {
        // Arrange
        var (service, _) = CreateSut(137);

        // Act
        var primeira = service.GetPaged(1, 20);
        var ultima = service.GetPaged(7, 20);
        var alemDoTotal = service.GetPaged(8, 20);

        // Assert
        Assert.Equal(7, primeira.totalPages);
        Assert.Equal(20, primeira.items.Count);
        Assert.False(primeira.hasPrevious);
        Assert.True(primeira.hasNext);

        Assert.Equal(17, ultima.items.Count);
        Assert.True(ultima.hasPrevious);
        Assert.False(ultima.hasNext);

        Assert.Empty(alemDoTotal.items);
    }
}