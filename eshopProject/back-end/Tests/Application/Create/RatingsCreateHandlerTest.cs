using Application.Commands.Create;
using AutoMapper;

namespace Tests.Application.Create;

public class RatingsCreateHandlerTest
{
    private readonly Mock<IRatingsRepository> _mockRatingsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly RatingCreateHandler _handler;

    public RatingsCreateHandlerTest()
    {
        _mockRatingsRepository = new Mock<IRatingsRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();
        _handler = new RatingCreateHandler(
            _mockRatingsRepository.Object,
            _mockMapper.Object,
            _mockContext.Object
        );
    }

    [Fact]
    public void Handle_ShouldThrowArgumentOutOfRangeException_WhenScoreIsInvalid()
    {
        // Arrange
        var command = new RatingCreateCommand
        {
            UserId = 1,
            ReviewerId = 2,
            Score = 0,  // Invalid score (should be between 1 and 5)
            Comment = "Great article!",
            CreatedAt = DateTime.UtcNow
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => _handler.Handle(command));
        Assert.Equal("Score must be between 1 and 5.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenRatingAlreadyExists()
    {
        // Arrange
        var command = new RatingCreateCommand
        {
            UserId = 1,
            ReviewerId = 2,
            Score = 4,
            Comment = "Great article!",
            CreatedAt = DateTime.UtcNow
        };

        var existingRating = new Ratings { UserId = 1, ReviewerId = 2 };

        // Mock the context to return an existing rating
        _mockContext.Setup(c => c.Ratings.FirstOrDefault(It.IsAny<Func<Ratings, bool>>())).Returns(existingRating);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(command));
        Assert.Equal("This rating already exists.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldCreateRatingSuccessfully_WhenValidCommand()
    {
        // Arrange
        var command = new RatingCreateCommand
        {
            UserId = 1,
            ReviewerId = 2,
            Score = 4,
            Comment = "Great article!",
            CreatedAt = DateTime.UtcNow
        };

        var rating = new Ratings
        {
            UserId = command.UserId,
            ReviewerId = command.ReviewerId,
            Score = command.Score,
            Comment = command.Comment,
            CreatedAt = command.CreatedAt
        };

        _mockMapper.Setup(m => m.Map<RatingCreateOutput>(It.IsAny<Ratings>())).Returns(new RatingCreateOutput());

        // Mock repository to simulate successful creation
        _mockRatingsRepository.Setup(r => r.Create(It.IsAny<Ratings>())).Verifiable();
        _mockContext.Setup(c => c.Ratings.FirstOrDefault(It.IsAny<Func<Ratings, bool>>())).Returns((Ratings)null);  // No existing rating

        // Act
        var result = _handler.Handle(command);

        // Assert
        _mockRatingsRepository.Verify(r => r.Create(It.IsAny<Ratings>()), Times.Once); // Ensure Create method was called
        Assert.NotNull(result); // Ensure the result is not null
    }

    [Fact]
    public void Handle_ShouldMapToRatingCreateOutput_WhenValidCommand()
    {
        // Arrange
        var command = new RatingCreateCommand
        {
            UserId = 1,
            ReviewerId = 2,
            Score = 4,
            Comment = "Great article!",
            CreatedAt = DateTime.UtcNow
        };

        var rating = new Ratings
        {
            UserId = command.UserId,
            ReviewerId = command.ReviewerId,
            Score = command.Score,
            Comment = command.Comment,
            CreatedAt = command.CreatedAt
        };

        _mockMapper.Setup(m => m.Map<RatingCreateOutput>(rating)).Returns(new RatingCreateOutput());

        // Act
        var result = _handler.Handle(command);

        // Assert
        _mockMapper.Verify(m => m.Map<RatingCreateOutput>(It.IsAny<Ratings>()), Times.Once); // Ensure Map method was called
        Assert.NotNull(result); // Ensure the result is not null
    }
}