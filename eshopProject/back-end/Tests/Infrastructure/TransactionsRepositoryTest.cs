namespace Tests.Infrastructure;

using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

public class TransactionsRepositoryTest
{
    private readonly Mock<DbSet<Transactions>> _mockSet;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly TransactionsRepository _repository;

    public TransactionsRepositoryTest()
    {
        // Initialize mocks
        _mockSet = new Mock<DbSet<Transactions>>();
        _mockContext = new Mock<TradeShopContext>();

        // Setup the context to return the mocked DbSet
        _mockContext.Setup(m => m.Transactions).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new TransactionsRepository(_mockContext.Object);
    }

    [Fact]
    public void Create_ShouldAddTransactionAndReturnCreatedEntity()
    {
        // Arrange
        var newTransaction = new Transactions
        {
            TransactionId = 1,
            BuyerId = 1,
            SellerId = 2,
            ArticleId = 101,
            TransactionType = "purchase",
            Price = 100.00M,
            Commission = 5.00M,
            TransactionDate = DateTime.Now,
            Status = "finished"
        };

        // Mock the behavior of Add to return an EntityEntry that represents the entity being added
        var entityEntryMock = new Mock<EntityEntry<Transactions>>();
        _mockSet.Setup(m => m.Add(It.IsAny<Transactions>())).Returns(entityEntryMock.Object);

        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Create(newTransaction);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TransactionId);
        Assert.Equal("finished", result.Status);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenTransactionExists()
    {
        // Arrange
        var existingTransaction = new Transactions
        {
            TransactionId = 1,
            BuyerId = 1,
            SellerId = 2,
            ArticleId = 101,
            TransactionType = "purchase",
            Price = 100.00M,
            Commission = 5.00M,
            TransactionDate = DateTime.Now,
            Status = "finished"
        };

        // Mock the behavior of FirstOrDefault to return the existing transaction
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Transactions, bool>>())).Returns(existingTransaction);

        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Update(existingTransaction);

        // Assert
        Assert.True(result); // Ensure the transaction was updated successfully
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenTransactionDoesNotExist()
    {
        // Arrange
        var nonExistingTransaction = new Transactions
        {
            TransactionId = 999,
            BuyerId = 1,
            SellerId = 2,
            ArticleId = 101,
            TransactionType = "purchase",
            Price = 100.00M,
            Commission = 5.00M,
            TransactionDate = DateTime.Now,
            Status = "finished"
        };

        // Mock the behavior of FirstOrDefault to return null (indicating the transaction does not exist)
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Transactions, bool>>())).Returns((Transactions)null);

        // Act
        var result = _repository.Update(nonExistingTransaction);

        // Assert
        Assert.False(result); // Ensure the transaction was not updated
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenTransactionExists()
    {
        // Arrange
        var existingTransaction = new Transactions
        {
            TransactionId = 1,
            BuyerId = 1,
            SellerId = 2,
            ArticleId = 101,
            TransactionType = "purchase",
            Price = 100.00M,
            Commission = 5.00M,
            TransactionDate = DateTime.Now,
            Status = "finished"
        };

        // Mock the behavior of FirstOrDefault to return the existing transaction
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Transactions, bool>>())).Returns(existingTransaction);

        // Mock SaveChanges to return 1, simulating a successful deletion
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the transaction was deleted successfully
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenTransactionDoesNotExist()
    {
        // Arrange
        // Mock the behavior of FirstOrDefault to return null (indicating the transaction does not exist)
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Transactions, bool>>())).Returns((Transactions)null);

        // Act
        var result = _repository.Delete(999); // Non-existing ID

        // Assert
        Assert.False(result); // Ensure the transaction was not deleted
    }
}
