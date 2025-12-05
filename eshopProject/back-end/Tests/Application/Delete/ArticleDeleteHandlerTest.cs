using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class ArticleDeleteHandlerTest
{
    private readonly Mock<IArticlesRepository> _mockArticlesRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly ArticleDeleteHandler _handler;

    public ArticleDeleteHandlerTest()
    {
        // Create mocks
        _mockArticlesRepository = new Mock<IArticlesRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject mocks into the handler
        _handler = new ArticleDeleteHandler(_mockArticlesRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteArticle_WhenArticleExists()
    {
        // Arrange
        int articleId = 1;
        
        // Mock that the article exists in the repository
        _mockArticlesRepository.Setup(repo => repo.GetById(articleId)).Returns(new Articles { ArticleId = articleId });

        // Act
        _handler.Handle(articleId);

        // Assert
        _mockArticlesRepository.Verify(repo => repo.Delete(articleId), Times.Once); // Verify if Delete is called once
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify if SaveChanges is called once
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenArticleDoesNotExist()
    {
        // Arrange
        int articleId = 1;

        // Mock that the article does not exist in the repository
        _mockArticlesRepository.Setup(repo => repo.GetById(articleId)).Returns((Articles)null);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(articleId));
        Assert.Equal("Article not found", exception.Message); // Verify that the exception has the correct message
    }
}
