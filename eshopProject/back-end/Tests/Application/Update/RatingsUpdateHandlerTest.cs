using Application.Commands.update;
using Application.exceptions;
using AutoMapper;

namespace Tests.Application.Update;

public class RatingsUpdateHandlerTest
{
    private readonly Mock<IRatingsRepository> _ratingsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly RatingUpdateHandler _handler;

    public RatingsUpdateHandlerTest()
    {
        _ratingsRepositoryMock = new Mock<IRatingsRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new RatingUpdateHandler(_ratingsRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateRating_WhenValidDataIsProvided()
    {
        // Arrange: Prepare the input command and mocked repository response
        var updateCommand = new RatingUpdateCommand
        {
            RatingId = 1,
            UserId = 1,
            ReviewerId = 2,
            Score = 4,
            Comment = "Great experience!",
            CreatedAt = DateTime.Now
        };

        var rating = new Ratings
        {
            RatingId = 1,
            UserId = 1,
            ReviewerId = 2,
            Score = 3,
            Comment = "Good experience.",
            CreatedAt = DateTime.Now.AddDays(-1)
        };

        _ratingsRepositoryMock.Setup(repo => repo.GetById(updateCommand.RatingId)).Returns(rating);

        // Act: Call the handler to update the rating
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated rating
        _ratingsRepositoryMock.Verify(repo => repo.Update(It.Is<Ratings>(r => 
            r.UserId == updateCommand.UserId &&
            r.ReviewerId == updateCommand.ReviewerId &&
            r.Score == updateCommand.Score &&
            r.Comment == updateCommand.Comment
        )), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenRatingNotFound()
    {
        // Arrange: Prepare the input command with a non-existing RatingId
        var updateCommand = new RatingUpdateCommand { RatingId = 99 };

        _ratingsRepositoryMock.Setup(repo => repo.GetById(updateCommand.RatingId)).Returns((Ratings)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<RatingNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("Rating not found with id: 99", exception.Message);
    }
}
