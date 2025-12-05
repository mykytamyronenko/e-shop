using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

public class RatingsGetAllHandlerTest
{
    private readonly Mock<IRatingsRepository> _mockRatingsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly RatingsGetAllHandler _handler;

    public RatingsGetAllHandlerTest()
    {
        // Create mocks for the repository and mapper
        _mockRatingsRepository = new Mock<IRatingsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Inject the mocks into the handler
        _handler = new RatingsGetAllHandler(_mockRatingsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnRatings_WhenRatingsExist()
    {
        // Arrange
        var dbRatings = new List<Ratings>
        {
            new Ratings { RatingId = 1, Score = 5, Comment = "Excellent" },
            new Ratings { RatingId = 2, Score = 3, Comment = "Good" }
        };

        // Mock repository method to return a list of ratings
        _mockRatingsRepository.Setup(repo => repo.GetAll()).Returns(dbRatings);

        // Mock mapper method to map the ratings
        _mockMapper.Setup(m => m.Map<List<RatingsGetAllOutput.Ratings>>(It.IsAny<List<Ratings>>()))
                   .Returns(dbRatings.Select(r => new RatingsGetAllOutput.Ratings { RatingId = r.RatingId, Score = r.Score, Comment = r.Comment }).ToList());

        // Act
        var result = _handler.Handle(new RatingsGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.RatingsList.Count);
        Assert.Equal(1, result.RatingsList[0].RatingId);
        Assert.Equal(5, result.RatingsList[0].Score);
        Assert.Equal("Excellent", result.RatingsList[0].Comment);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoRatingsExist()
    {
        // Arrange
        var dbRatings = new List<Ratings>(); // No ratings

        // Mock repository method to return an empty list
        _mockRatingsRepository.Setup(repo => repo.GetAll()).Returns(dbRatings);

        // Act
        var result = _handler.Handle(new RatingsGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.RatingsList); // Should return an empty list
    }
}
