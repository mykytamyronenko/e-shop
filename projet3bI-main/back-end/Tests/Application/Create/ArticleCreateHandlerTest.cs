using Application.Commands.Create;
using Application.utils;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Tests.Application.Create;

public class ArticleCreateHandlerTest
{
    private readonly Mock<IArticlesRepository> _mockArticlesRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly ArticleCreateHandler _handler;

    public ArticleCreateHandlerTest()
    {
        _mockArticlesRepository = new Mock<IArticlesRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();
        _handler = new ArticleCreateHandler(
            _mockArticlesRepository.Object,
            _mockMapper.Object,
            _mockContext.Object
        );
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenInvalidState()
    {
        // Arrange
        var command = new ArticleCreateCommand
        {
            State = "invalidState",  // Invalid state
            Status = "available",
            Image = new FormFile(null, 0, 0, "file", "image.jpg"),
            Category = "Electronics",
            Title = "Test Article",
            Description = "Description",
            Price = 100,
            UserId = 1,
            Quantity = 10
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _handler.Handle(command));
        Assert.Equal("Invalid state. Allowed values are 'new' and 'used'.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenInvalidStatus()
    {
        // Arrange
        var command = new ArticleCreateCommand
        {
            State = "new",
            Status = "invalidStatus",  // Invalid status
            Image = new FormFile(null, 0, 0, "file", "image.jpg"),
            Category = "Electronics",
            Title = "Test Article",
            Description = "Description",
            Price = 100,
            UserId = 1,
            Quantity = 10
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _handler.Handle(command));
        Assert.Equal("Invalid status. Allowed values are 'available', 'sold', and 'removed'.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenImageIsNullOrEmpty()
    {
        // Arrange
        var command = new ArticleCreateCommand
        {
            State = "new",
            Status = "available",
            Image = null,  // No image provided
            Category = "Electronics",
            Title = "Test Article",
            Description = "Description",
            Price = 100,
            UserId = 1,
            Quantity = 10
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _handler.Handle(command));
        Assert.Equal("Image is required", exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrowArgumentException_WhenCategoryIsInvalid()
    {
        // Arrange
        var command = new ArticleCreateCommand
        {
            State = "new",
            Status = "available",
            Image = new FormFile(null, 0, 0, "file", "image.jpg"),
            Category = "InvalidCategory",  // Invalid category
            Title = "Test Article",
            Description = "Description",
            Price = 100,
            UserId = 1,
            Quantity = 10
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => _handler.Handle(command));
        Assert.Equal("Invalid category. Allowed categories are: " + string.Join(", ", Enum.GetNames(typeof(ArticleCategory))), exception.Message);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenArticleAlreadyExists()
    {
        // Arrange
        var command = new ArticleCreateCommand
        {
            State = "new",
            Status = "available",
            Image = new FormFile(null, 0, 0, "file", "image.jpg"),
            Category = "Electronics",
            Title = "Test Article",
            Description = "Description",
            Price = 100,
            UserId = 1,
            Quantity = 10
        };

        var article = new Articles { ArticleId = 1 };  // Existing article

        // Mock the context to return an existing article
        _mockContext.Setup(c => c.Articles.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns(article);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(command));
        Assert.Equal("Article already exists.", exception.Message);
    }

    [Fact]
    public void Handle_ShouldCreateArticleSuccessfully_WhenValidCommand()
    {
        // Arrange
        var command = new ArticleCreateCommand
        {
            State = "new",
            Status = "available",
            Image = new FormFile(null, 0, 0, "file", "image.jpg"),
            Category = "Electronics",
            Title = "Test Article",
            Description = "Description",
            Price = 100,
            UserId = 1,
            Quantity = 10
        };

        // Mock the repository to return a success
        _mockMapper.Setup(m => m.Map<ArticleCreateOutput>(It.IsAny<Articles>())).Returns(new ArticleCreateOutput());
        _mockArticlesRepository.Setup(r => r.Create(It.IsAny<Articles>())).Verifiable();

        // Act
        var result = _handler.Handle(command);

        // Assert
        _mockArticlesRepository.Verify(r => r.Create(It.IsAny<Articles>()), Times.Once); // Ensure Create method was called
        Assert.NotNull(result); // Ensure the result is not null
    }
}