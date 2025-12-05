namespace Tests.Infrastructure;

using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq;

public class RatingsRepositoryTest
{
    private readonly Mock<DbSet<Ratings>> _mockSet;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly RatingsRepository _repository;

    public RatingsRepositoryTest()
    {
        // Initialize mocks
        _mockSet = new Mock<DbSet<Ratings>>();
        _mockContext = new Mock<TradeShopContext>();

        // Setup the context to return the mocked DbSet
        _mockContext.Setup(m => m.Ratings).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new RatingsRepository(_mockContext.Object);
    }

    [Fact]
    public void Create_ShouldAddRatingAndReturnCreatedEntity()
    {
        // Arrange
        var newRating = new Ratings { RatingId = 1, UserId = 1, ReviewerId = 2, Score = 5, Comment = "Excellent", CreatedAt = DateTime.Now };
        
        // Mock the behavior of Add to return an EntityEntry that represents the entity being added
        var entityEntryMock = new Mock<EntityEntry<Ratings>>();
        _mockSet.Setup(m => m.Add(It.IsAny<Ratings>())).Returns(entityEntryMock.Object);

        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Create(newRating);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Excellent", result.Comment);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenRatingExists()
    {
        // Arrange
        var existingRating = new Ratings { RatingId = 1, UserId = 1, ReviewerId = 2, Score = 5, Comment = "Good", CreatedAt = DateTime.Now };
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Ratings, bool>>())).Returns(existingRating);
        
        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Update(existingRating);

        // Assert
        Assert.True(result); // Ensure the rating was updated successfully
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenRatingDoesNotExist()
    {
        // Arrange
        var nonExistingRating = new Ratings { RatingId = 999, UserId = 1, ReviewerId = 2, Score = 5, Comment = "Good", CreatedAt = DateTime.Now };
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Ratings, bool>>())).Returns((Ratings)null);
        
        // Act
        var result = _repository.Update(nonExistingRating);

        // Assert
        Assert.False(result); // Ensure the rating was not updated
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenRatingExists()
    {
        // Arrange
        var existingRating = new Ratings { RatingId = 1, UserId = 1, ReviewerId = 2, Score = 5, Comment = "Good", CreatedAt = DateTime.Now };
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Ratings, bool>>())).Returns(existingRating);
        
        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the rating was deleted successfully
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenRatingDoesNotExist()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Ratings, bool>>())).Returns((Ratings)null);
        
        // Act
        var result = _repository.Delete(999); // Non-existing ID

        // Assert
        Assert.False(result); // Ensure the rating was not deleted
    }
}
