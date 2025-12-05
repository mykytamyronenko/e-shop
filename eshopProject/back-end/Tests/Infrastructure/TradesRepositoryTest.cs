namespace Tests.Infrastructure;

using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Linq;

public class TradesRepositoryTest
{
    private readonly Mock<DbSet<Trades>> _mockSet;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly TradesRepository _repository;

    public TradesRepositoryTest()
    {
        // Initialize mocks
        _mockSet = new Mock<DbSet<Trades>>();
        _mockContext = new Mock<TradeShopContext>();

        // Setup the context to return the mocked DbSet
        _mockContext.Setup(m => m.Trades).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new TradesRepository(_mockContext.Object);
    }

    [Fact]
    public void Create_ShouldAddTradeAndReturnCreatedEntity()
    {
        // Arrange
        var newTrade = new Trades 
        {
            TradeId = 1,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "5,6,7",
            ReceiverArticleId = 201,
            TradeDate = DateTime.Now,
            Status = "Pending"
        };

        // Mock the behavior of Add to return an EntityEntry that represents the entity being added
        var entityEntryMock = new Mock<EntityEntry<Trades>>();
        _mockSet.Setup(m => m.Add(It.IsAny<Trades>())).Returns(entityEntryMock.Object);

        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Create(newTrade);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.TradeId);
        Assert.Equal("Pending", result.Status);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenTradeExists()
    {
        // Arrange
        var existingTrade = new Trades 
        {
            TradeId = 1,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "5,6,7",
            ReceiverArticleId = 201,
            TradeDate = DateTime.Now,
            Status = "Pending"
        };

        // Mock the behavior of FirstOrDefault to return the existing trade
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Trades, bool>>())).Returns(existingTrade);

        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Update(existingTrade);

        // Assert
        Assert.True(result); // Ensure the trade was updated successfully
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenTradeDoesNotExist()
    {
        // Arrange
        var nonExistingTrade = new Trades
        {
            TradeId = 999,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "5,6,7",
            ReceiverArticleId = 201,
            TradeDate = DateTime.Now,
            Status = "Pending"
        };

        // Mock the behavior of FirstOrDefault to return null (indicating the trade does not exist)
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Trades, bool>>())).Returns((Trades)null);

        // Act
        var result = _repository.Update(nonExistingTrade);

        // Assert
        Assert.False(result); // Ensure the trade was not updated
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenTradeExists()
    {
        // Arrange
        var existingTrade = new Trades
        {
            TradeId = 1,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "5,6,7",
            ReceiverArticleId = 201,
            TradeDate = DateTime.Now,
            Status = "Pending"
        };

        // Mock the behavior of FirstOrDefault to return the existing trade
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Trades, bool>>())).Returns(existingTrade);

        // Mock SaveChanges to return 1, simulating a successful deletion
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the trade was deleted successfully
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenTradeDoesNotExist()
    {
        // Arrange
        // Mock the behavior of FirstOrDefault to return null (indicating the trade does not exist)
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Trades, bool>>())).Returns((Trades)null);

        // Act
        var result = _repository.Delete(999); // Non-existing ID

        // Assert
        Assert.False(result); // Ensure the trade was not deleted
    }
}
