using Application.Commands.update;
using Application.exceptions;
using Application.utils;
using AutoMapper;

namespace Tests.Application.Update;

public class ArticleUpdateHandlerTest
{
    private readonly Mock<IArticlesRepository> _articlesRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly ArticleUpdateHandler _handler;

    public ArticleUpdateHandlerTest()
    {
        _articlesRepositoryMock = new Mock<IArticlesRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new ArticleUpdateHandler(_articlesRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateArticle_WhenValidArticleIsProvided()
    {
        // Arrange: Prepare the input command and mocked repository response
        var updateCommand = new ArticleUpdateCommand
        {
            ArticleId = 1,
            Title = "Updated Title",
            Description = "Updated Description",
            Price = 100.0m,
            Category = "Electronics",
            State = "New",
            UserId = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow,
            Status = "active",
            MainImageUrl = "new_image_url",
            Quantity = 10
        };

        var article = new Articles
        {
            ArticleId = 1,
            Title = "Old Title",
            Description = "Old Description",
            Price = 50.0m,
            Category = ArticleCategory.Other,
            State = "Old",
            UserId = 2,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Status = "suspended",
            MainImageUrl = "old_image_url",
            Quantity = 5
        };

        _articlesRepositoryMock.Setup(repo => repo.GetById(updateCommand.ArticleId)).Returns(article);

        // Act: Call the handler
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated article
        _articlesRepositoryMock.Verify(repo => repo.Update(It.Is<Articles>(a => a.Title == updateCommand.Title &&
                                                                               a.Description == updateCommand.Description &&
                                                                               a.Price == updateCommand.Price &&
                                                                               a.Category == Enum.Parse<ArticleCategory>(updateCommand.Category, true) &&
                                                                               a.State == updateCommand.State &&
                                                                               a.UserId == updateCommand.UserId &&
                                                                               a.UpdatedAt >= DateTime.UtcNow &&
                                                                               a.Status == updateCommand.Status &&
                                                                               a.MainImageUrl == updateCommand.MainImageUrl &&
                                                                               a.Quantity == updateCommand.Quantity)), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenArticleNotFound()
    {
        // Arrange: Prepare the input command with a non-existing ArticleId
        var updateCommand = new ArticleUpdateCommand { ArticleId = 99 };

        _articlesRepositoryMock.Setup(repo => repo.GetById(updateCommand.ArticleId)).Returns((Articles)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<ArticleNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("Article not found with id: 99", exception.Message);
    }
}
