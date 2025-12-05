using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class RatingsDeleteHandlerTest
{
    private readonly Mock<IRatingsRepository> _mockRatingsRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly RatingDeleteHandler _handler;

    public RatingsDeleteHandlerTest()
    {
        // Create mocks for the repository and context
        _mockRatingsRepository = new Mock<IRatingsRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject the mocks into the handler
        _handler = new RatingDeleteHandler(_mockRatingsRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteRating_WhenRatingExists()
    {
        // Arrange
        int ratingId = 1;

        // Mock that the rating exists in the repository
        _mockRatingsRepository.Setup(repo => repo.GetById(ratingId)).Returns(new Ratings { RatingId = ratingId });

        // Act
        _handler.Handle(ratingId);

        // Assert
        _mockRatingsRepository.Verify(repo => repo.Delete(ratingId), Times.Once); // Verify Delete was called
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify SaveChanges was called
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenRatingDoesNotExist()
    {
        // Arrange
        int ratingId = 1;

        // Mock that the rating does not exist in the repository
        _mockRatingsRepository.Setup(repo => repo.GetById(ratingId)).Returns((Ratings)null);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(ratingId));
        Assert.Equal("Rating not found", exception.Message); // Verify the exception message
    }
}
