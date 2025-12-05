using API.Controllers;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class RatingQueryControllerTest
{
    private readonly Mock<RatingsQueryProcessor> _mockRatingsQueryProcessor;
    private readonly RatingQueryController _controller;

    public RatingQueryControllerTest()
    {
        _mockRatingsQueryProcessor = new Mock<RatingsQueryProcessor>();
        _controller = new RatingQueryController(_mockRatingsQueryProcessor.Object);
    }
    
    [Fact]
    public void GetAllRatings_ReturnsOk_WhenRatingsExist()
    {
        // Arrange
        var ratingsList = new List<RatingsGetAllOutput.Ratings>
        {
            new RatingsGetAllOutput.Ratings { RatingId = 1, ReviewerId = 1, Score = 5 },
            new RatingsGetAllOutput.Ratings { RatingId = 2, ReviewerId = 2, Score = 4 }
        };
    
        var ratingsOutput = new RatingsGetAllOutput { RatingsList = ratingsList };

        var ratingsGetAllQuery = new RatingsGetAllQuery(); 
    
        _mockRatingsQueryProcessor.Setup(p => p.GetAll(It.IsAny<RatingsGetAllQuery>())).Returns(ratingsOutput);

        // Act
        var result = _controller.GetAllRatings();

        // Assert
        var actionResult = Assert.IsType<List<RatingsGetAllOutput.Ratings>>(result);
        Assert.Equal(2, actionResult.Count);
    }


    [Fact]
    public void GetAllRatings_ReturnsOk_WhenNoRatingsExist()
    {
        // Arrange
        var ratingsOutput = new RatingsGetAllOutput { RatingsList = new List<RatingsGetAllOutput.Ratings>() };

        var ratingsGetAllQuery = new RatingsGetAllQuery();
    
        _mockRatingsQueryProcessor.Setup(p => p.GetAll(It.IsAny<RatingsGetAllQuery>())).Returns(ratingsOutput);

        // Act
        var result = _controller.GetAllRatings();

        // Assert
        var actionResult = Assert.IsType<List<RatingsGetAllOutput.Ratings>>(result);
        Assert.Empty(actionResult);
    }


    [Fact]
    public void GetByIdRating_ReturnsOk_WhenRatingExists()
    {
        // Arrange
        var ratingId = 1;
        var ratingOutput = new RatingsGetByIdOutput { RatingId = ratingId, ReviewerId = 1, Score = 5 };
    
        _mockRatingsQueryProcessor.Setup(p => p.GetById(ratingId)).Returns(ratingOutput);

        // Act
        var result = _controller.GetByIdRating(ratingId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<RatingsGetByIdOutput>(actionResult.Value);
        Assert.Equal(ratingId, value.RatingId);
    }

    [Fact]
    public void GetByIdRating_ReturnsNotFound_WhenRatingDoesNotExist()
    {
        // Arrange
        var ratingId = 999;
        _mockRatingsQueryProcessor.Setup(p => p.GetById(ratingId)).Throws(new RatingNotFoundException(ratingId));

        // Act
        var result = _controller.GetByIdRating(ratingId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Rating not found", actionResult.Value);
    }

}