using Bankly.Domain.Commom;
using Bankly.Domain.Entities;
using Bankly.Domain.Enums;
using Xunit;

namespace Bankly.Domain.Tests;

public class TransactionTests
{
    [Fact]
    public void CriarTransacao_ComValorPositivo_DeveCriarComSucesso()
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act
        var transaction = new Transaction(accountId, 150m, TransactionTypeEnum.DEPOSITO);

        // Assert
        Assert.Equal(150m, transaction.Amount);
        Assert.Equal(TransactionTypeEnum.DEPOSITO, transaction.Type);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(-0.01)]
    public void CriarTransacao_ComValorInvalido_DeveLancarDomainException(decimal amount)
    {
        // Arrange
        var accountId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new Transaction(accountId, amount, TransactionTypeEnum.DEPOSITO));
    }
}