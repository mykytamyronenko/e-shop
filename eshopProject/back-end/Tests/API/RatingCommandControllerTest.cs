using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.getById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class RatingCommandControllerTest
{
    private readonly Mock<RatingCommandsProcessor> _mockRatingCommandsProcessor;
    private readonly Mock<RatingsQueryProcessor> _mockRatingsQueryProcessor;
    private readonly RatingCommandsController _controller;

    public RatingCommandControllerTest()
    {
        _mockRatingCommandsProcessor = new Mock<RatingCommandsProcessor>();
        _mockRatingsQueryProcessor = new Mock<RatingsQueryProcessor>();
        _controller = new RatingCommandsController(_mockRatingCommandsProcessor.Object,_mockRatingsQueryProcessor.Object);
    }
    [Fact]
    public void CreateRating_ReturnsOk_WhenRatingCreatedSuccessfully()
    {
        // Arrange
        var command = new RatingCreateCommand { ReviewerId = 1, Score = 5, Comment = "Excellent" };
        var userIdFromToken = 1;
        var expectedOutput = new RatingCreateOutput { ReviewerId = 1 };

        // Mock user identity
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Mock the command processor to return the expected output
        _mockRatingCommandsProcessor.Setup(p => p.CreateRating(command)).Returns(expectedOutput);

        // Act
        var result = _controller.CreateRating(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result); // Should return Ok
        var returnValue = Assert.IsType<TransactionCreateOutput>(actionResult.Value); // Should return the TransactionCreateOutput
        Assert.Equal(expectedOutput.ReviewerId, returnValue.BuyerId); // Verify the returned output
    }

    [Fact]
    public void CreateRating_ReturnsUnauthorized_WhenUserIdFromTokenIsMissing()
    {
        // Arrange
        var command = new RatingCreateCommand { ReviewerId = 1, Score = 5, Comment = "Excellent" };

        // Mock user identity without userId claim
        var identity = new ClaimsIdentity(new Claim[] { }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Act
        var result = _controller.CreateRating(command);

        // Assert
        var actionResult = Assert.IsType<UnauthorizedObjectResult>(result); // Should return Unauthorized
        Assert.Equal("Invalid token: User ID not found.", actionResult.Value); // Verify the message
    }

    [Fact]
    public void CreateRating_ReturnsForbid_WhenReviewerIdDoesNotMatchUserIdFromToken()
    {
        // Arrange
        var command = new RatingCreateCommand { ReviewerId = 1, Score = 5, Comment = "Excellent" };
        var userIdFromToken = 2; // Different from command.ReviewerId

        // Mock user identity with userIdFromToken = 2
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Act
        var result = _controller.CreateRating(command);

        // Assert
        var actionResult = Assert.IsType<ForbidResult>(result); // Should return Forbid
    }
    [Fact]
    public void UpdateRating_ReturnsOk_WhenRatingUpdatedSuccessfully()
    {
        // Arrange
        var command = new RatingUpdateCommand { RatingId = 1, ReviewerId = 1, Score = 4, Comment = "Good" };
        var userIdFromToken = 1;

        // Mock user identity
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Mock the command processor to update the rating
        _mockRatingCommandsProcessor.Setup(p => p.UpdateRating(command));

        // Act
        var result = _controller.UpdateRating(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result); // Should return Ok
        Assert.Equal("Rating updated.", actionResult.Value); // Verify the success message
    }

    [Fact]
    public void UpdateRating_ReturnsForbid_WhenReviewerIdDoesNotMatchUserIdFromToken()
    {
        // Arrange
        var command = new RatingUpdateCommand { RatingId = 1, ReviewerId = 1, Score = 4, Comment = "Good" };
        var userIdFromToken = 2; // Different from command.ReviewerId

        // Mock user identity with userIdFromToken = 2
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Act
        var result = _controller.UpdateRating(command);

        // Assert
        var actionResult = Assert.IsType<ForbidResult>(result); // Should return Forbid
    }

    [Fact]
    public void DeleteRating_ReturnsNoContent_WhenRatingDeletedSuccessfully()
    {
        // Arrange
        var ratingId = 1;
        var userIdFromToken = 1;

        // Create a mock RatingsGetByIdOutput object (this is likely the expected return type)
        var ratingOutput = new RatingsGetByIdOutput
        {
            RatingId = ratingId,
            ReviewerId = userIdFromToken,
            Score = 4,
            Comment = "Good"
        };

        // Mock user identity
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Mock the query processor to return the RatingsGetByIdOutput
        _mockRatingsQueryProcessor.Setup(p => p.GetById(ratingId)).Returns(ratingOutput);

        // Mock the command processor to delete the rating
        _mockRatingCommandsProcessor.Setup(p => p.DeleteRating(ratingId));

        // Act
        var result = _controller.DeleteRating(ratingId);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result); // Should return NoContent
    }


    [Fact]
    public void DeleteRating_ReturnsNotFound_WhenRatingDoesNotExist()
    {
        // Arrange
        var ratingId = 1;
        var userIdFromToken = 1;

        // Mock user identity
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Mock the query processor to return null (Rating not found)
        _mockRatingsQueryProcessor.Setup(p => p.GetById(ratingId)).Returns((RatingsGetByIdOutput)null);

        // Act
        var result = _controller.DeleteRating(ratingId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result); // Should return NotFound
        Assert.Equal($"Rating with ID {ratingId} not found.", actionResult.Value); // Verify the message
    }

    


}