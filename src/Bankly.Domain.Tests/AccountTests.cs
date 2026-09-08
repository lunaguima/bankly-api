using Bankly.Domain.Commom;
using Bankly.Domain.Entities;
using Bankly.Domain.Enums;
using Xunit;

namespace Bankly.Domain.Tests;

public class AccountTests
{
    [Fact]
    public void CriarConta_ComSaldoInicialValido_DeveCriarComSucesso()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountTypeId = Guid.NewGuid();

        // Act
        var account = new Account(userId, accountTypeId, "0001", "12345-6", 100m);

        // Assert
        Assert.Equal(100m, account.Balance);
        Assert.Equal(userId, account.UserId);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(-0.01)]
    public void CriarConta_ComSaldoNegativo_DeveLancarDomainException(decimal saldoInicial)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var accountTypeId = Guid.NewGuid();

        // Act & Assert
        Assert.Throws<DomainException>(() =>
            new Account(userId, accountTypeId, "0001", "12345-6", saldoInicial));
    }

    [Fact]
    public void ApplyTransaction_Saque_ComSaldoSuficiente_DeveDebitarValor()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), Guid.NewGuid(), "0001", "12345-6", 200m);
        var saque = new Transaction(account.Id, 50m, TransactionTypeEnum.SAQUE);

        // Act
        account.ApplyTransaction(saque);

        // Assert
        Assert.Equal(150m, account.Balance);
    }

    [Fact]
    public void ApplyTransaction_Saque_ComSaldoInsuficiente_DeveLancarDomainException()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), Guid.NewGuid(), "0001", "12345-6", 30m);
        var saque = new Transaction(account.Id, 50m, TransactionTypeEnum.SAQUE);

        // Act & Assert
        Assert.Throws<DomainException>(() => account.ApplyTransaction(saque));
    }
}