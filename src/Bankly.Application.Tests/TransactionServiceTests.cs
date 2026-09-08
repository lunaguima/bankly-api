using Bankly.Application.DTOs;
using Bankly.Application.Services;
using Bankly.Domain.Entities;
using Bankly.Domain.Enums;
using Moq;
using Xunit;

namespace Bankly.Application.Tests;

public class TransactionServiceTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IAccountRepository> _accountRepositoryMock;
    private readonly TransactionService _sut;

    public TransactionServiceTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _sut = new TransactionService(_transactionRepositoryMock.Object, _accountRepositoryMock.Object);
    }

    [Fact]
    public void Create_ContaInexistente_DeveLancarExcecaoENaoPersistir()
    {
        // Arrange
        var request = new TransactionRequest(Guid.NewGuid(), 100m, TransactionTypeEnum.DEPOSITO);
        _accountRepositoryMock.Setup(r => r.GetById(request.accountId)).Returns((Account)null);

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _sut.Create(request));

        _transactionRepositoryMock.Verify(r => r.Add(It.IsAny<Transaction>()), Times.Never);
        _accountRepositoryMock.Verify(r => r.Update(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public void Create_ContaExistente_DevePersistirUmaVez()
    {
        // Arrange
        var account = new Account(Guid.NewGuid(), Guid.NewGuid(), "0001", "12345-6", 200m);
        var request = new TransactionRequest(account.Id, 50m, TransactionTypeEnum.DEPOSITO);
        _accountRepositoryMock.Setup(r => r.GetById(account.Id)).Returns(account);

        // Act
        var result = _sut.Create(request);

        // Assert
        Assert.Equal(250m, account.Balance);
        _transactionRepositoryMock.Verify(r => r.Add(It.IsAny<Transaction>()), Times.Once);
        _accountRepositoryMock.Verify(r => r.Update(account), Times.Once);
    }
}