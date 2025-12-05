using Application.Commands.Create;
using AutoMapper;

namespace Tests.Application.Create;

public class TransactionsCreateHandlerTest
{
    private readonly Mock<ITransactionsRepository> _mockTransactionsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly TransactionCreateHandler _handler;

    public TransactionsCreateHandlerTest()
    {
        _mockTransactionsRepository = new Mock<ITransactionsRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();

        _handler = new TransactionCreateHandler(_mockTransactionsRepository.Object, _mockMapper.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenTransactionTypeIsInvalid()
    {
        // Arrange
        var command = new TransactionCreateCommand
        {
            TransactionType = "invalid",  // Invalid type
            Status = "in progress",
            ArticleId = 1,
            BuyerId = 1,
            SellerId = 2,
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenStatusIsInvalid()
    {
        // Arrange
        var command = new TransactionCreateCommand
        {
            TransactionType = "purchase",
            Status = "invalid",  // Invalid status
            ArticleId = 1,
            BuyerId = 1,
            SellerId = 2,
            Price = 100m
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenBuyerDoesNotHaveEnoughBalance()
    {
        // Arrange
        var command = new TransactionCreateCommand
        {
            TransactionType = "purchase",
            Status = "in progress",
            ArticleId = 1,
            BuyerId = 1,
            SellerId = 2,
            Price = 100m
        };

        // Mocks
        var buyer = new Users { UserId = 1, Balance = 50m }; // Insufficient balance
        var seller = new Users { UserId = 2, Balance = 100m };
        var article = new Articles { ArticleId = 1, Status = "available", Quantity = 10 };

        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns(buyer);
        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 2)).Returns(seller);
        _mockContext.Setup(c => c.Articles.FirstOrDefault(a => a.ArticleId == 1)).Returns(article);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldSuccessfullyCreateTransaction_WhenAllConditionsAreMet()
    {
        // Arrange
        var command = new TransactionCreateCommand
        {
            TransactionType = "purchase",
            Status = "in progress",
            ArticleId = 1,
            BuyerId = 1,
            SellerId = 2,
            Price = 100m
        };

        // Mocks
        var buyer = new Users { UserId = 1, Balance = 200m }; // Sufficient balance
        var seller = new Users { UserId = 2, Balance = 100m };
        var article = new Articles { ArticleId = 1, Status = "available", Quantity = 10 };

        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns(buyer);
        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 2)).Returns(seller);
        _mockContext.Setup(c => c.Articles.FirstOrDefault(a => a.ArticleId == 1)).Returns(article);
        _mockContext.Setup(c => c.UserMemberships.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>())).Returns((UserMemberships)null);

        var existingTransaction = _mockContext.Object.Transactions.FirstOrDefault(t => t.TransactionId == 1);
        _mockContext.Setup(c => c.Transactions.FirstOrDefault(It.IsAny<Func<Transactions, bool>>())).Returns(existingTransaction);

        _mockMapper.Setup(m => m.Map<TransactionCreateOutput>(It.IsAny<Transactions>())).Returns(new TransactionCreateOutput());

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.NotNull(result); // Ensure that the result is not null
        _mockContext.Verify(c => c.SaveChanges(), Times.Once); // Verify SaveChanges was called once
        _mockTransactionsRepository.Verify(r => r.Create(It.IsAny<Transactions>()), Times.Once); // Ensure transaction creation was invoked
    }
}
