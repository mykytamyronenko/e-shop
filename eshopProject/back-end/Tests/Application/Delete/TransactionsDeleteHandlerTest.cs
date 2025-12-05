using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class TransactionsDeleteHandlerTest
{
    private readonly Mock<ITransactionsRepository> _mockTransactionsRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly TransactionDeleteHandler _handler;

    public TransactionsDeleteHandlerTest()
    {
        // Create mocks for the repository and context
        _mockTransactionsRepository = new Mock<ITransactionsRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject the mocks into the handler
        _handler = new TransactionDeleteHandler(_mockTransactionsRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteTransaction_WhenTransactionExists()
    {
        // Arrange
        int transactionId = 1;

        // Mock that the transaction exists in the repository
        _mockTransactionsRepository.Setup(repo => repo.GetById(transactionId)).Returns(new Transactions { TransactionId = transactionId });

        // Act
        _handler.Handle(transactionId);

        // Assert
        _mockTransactionsRepository.Verify(repo => repo.Delete(transactionId), Times.Once); // Verify Delete was called
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify SaveChanges was called
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenTransactionDoesNotExist()
    {
        // Arrange
        int transactionId = 1;

        // Mock that the transaction does not exist in the repository
        _mockTransactionsRepository.Setup(repo => repo.GetById(transactionId)).Returns((Transactions)null);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(transactionId));
        Assert.Equal("Transaction not found", exception.Message); // Verify the exception message
    }
}
