using Application.exceptions;
using Application.Queries.getById;

namespace Tests.Application.GetById;

using Moq;
using Xunit;
using AutoMapper;
using System;

public class RatingsGetByIdHandlerTest
{
    private readonly Mock<IRatingsRepository> _mockRatingsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RatingsGetByIdHandler _handler;

    public RatingsGetByIdHandlerTest()
    {
        // Mock the dependencies
        _mockRatingsRepository = new Mock<IRatingsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Instantiate the handler
        _handler = new RatingsGetByIdHandler(_mockRatingsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnRating_WhenRatingExists()
    {
        // Arrange: Define a mock rating entity
        var ratingId = 1;
        var dbRating = new Ratings { RatingId = ratingId, UserId = 123,ReviewerId = 456 ,Score = 5, CreatedAt = DateTime.Now };
        var outputRating = new RatingsGetByIdOutput { RatingId = ratingId, UserId = 123,  ReviewerId = 456, Score = 5, CreatedAt = DateTime.Now };

        // Mock the repository to return the rating
        _mockRatingsRepository.Setup(repo => repo.GetById(ratingId)).Returns(dbRating);

        // Mock the mapper to map the entity to the output DTO
        _mockMapper.Setup(m => m.Map<RatingsGetByIdOutput>(dbRating)).Returns(outputRating);

        // Act: Call the handler's handle method
        var result = _handler.Handle(ratingId);

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(ratingId, result.RatingId);
        Assert.Equal(123, result.UserId);
        Assert.Equal(456, result.ReviewerId);
        Assert.Equal(5, result.Score);
    }

    [Fact]
    public void Handle_ShouldThrowRatingNotFoundException_WhenRatingDoesNotExist()
    {
        // Arrange: Define a non-existent rating ID
        var ratingId = 99;

        // Mock the repository to return null (rating not found)
        _mockRatingsRepository.Setup(repo => repo.GetById(ratingId)).Returns((Ratings)null);

        // Act & Assert: Ensure the handler throws an exception
        Assert.Throws<RatingNotFoundException>(() => _handler.Handle(ratingId));
    }
}
