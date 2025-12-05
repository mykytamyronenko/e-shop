using Application.Commands.update;
using Application.exceptions;
using AutoMapper;

namespace Tests.Application.Update;

public class TransactionsUpdateHandlerTest
{
    private readonly Mock<ITransactionsRepository> _transactionsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly TransactionUpdateHandler _handler;

    public TransactionsUpdateHandlerTest()
    {
        _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new TransactionUpdateHandler(_transactionsRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateTransaction_WhenValidDataIsProvided()
    {
        // Arrange: Prepare the input command and mocked repository response
        var updateCommand = new TransactionUpdateCommand
        {
            TransactionId = 1,
            BuyerId = 1,
            SellerId = 2,
            ArticleId = 3,
            TransactionType = "purchase",
            Price = 100,
            Commission = 5,
            TransactionDate = DateTime.Now,
            Status = "finished"
        };

        var transaction = new Transactions
        {
            TransactionId = 1,
            BuyerId = 1,
            SellerId = 2,
            ArticleId = 3,
            TransactionType = "purchase",
            Price = 50,
            Commission = 2,
            TransactionDate = DateTime.Now.AddDays(-1),
            Status = "in progress"
        };

        _transactionsRepositoryMock.Setup(repo => repo.GetById(updateCommand.TransactionId)).Returns(transaction);

        // Act: Call the handler to update the transaction
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated transaction
        _transactionsRepositoryMock.Verify(repo => repo.Update(It.Is<Transactions>(t =>
            t.BuyerId == updateCommand.BuyerId &&
            t.SellerId == updateCommand.SellerId &&
            t.ArticleId == updateCommand.ArticleId &&
            t.TransactionType == updateCommand.TransactionType &&
            t.Price == updateCommand.Price &&
            t.Commission == updateCommand.Commission &&
            t.TransactionDate == updateCommand.TransactionDate &&
            t.Status == updateCommand.Status
        )), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenTransactionNotFound()
    {
        // Arrange: Prepare the input command with a non-existing TransactionId
        var updateCommand = new TransactionUpdateCommand { TransactionId = 99 };

        _transactionsRepositoryMock.Setup(repo => repo.GetById(updateCommand.TransactionId)).Returns((Transactions)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<TransactionNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("Transaction not found with id: 99", exception.Message);
    }
}
