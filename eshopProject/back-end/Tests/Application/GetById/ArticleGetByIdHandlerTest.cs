using Application.exceptions;
using Application.Queries.getById;

namespace Tests.Application.GetById;

using Moq;
using Xunit;
using AutoMapper;
using System;

public class ArticleGetByIdHandlerTest
{
    private readonly Mock<IArticlesRepository> _mockArticlesRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ArticlesGetByIdHandler _handler;

    public ArticleGetByIdHandlerTest()
    {
        // Mock dependencies
        _mockArticlesRepository = new Mock<IArticlesRepository>();
        _mockMapper = new Mock<IMapper>();

        // Instantiate the handler
        _handler = new ArticlesGetByIdHandler(_mockArticlesRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnArticle_WhenArticleExists()
    {
        // Arrange: Define a mock article
        var articleId = 1;
        var dbArticle = new Articles { ArticleId = articleId, Title = "Test Article", Description = "Test Content" };
        var outputArticle = new ArticlesGetByIdOutput { ArticleId = articleId, Title = "Test Article", Description = "Test Content" };

        // Mock the repository to return the article
        _mockArticlesRepository.Setup(repo => repo.GetById(articleId)).Returns(dbArticle);

        // Mock the mapper to map the entity to output DTO
        _mockMapper.Setup(m => m.Map<ArticlesGetByIdOutput>(dbArticle)).Returns(outputArticle);

        // Act: Call the handler's handle method
        var result = _handler.Handle(articleId);

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(articleId, result.ArticleId);
        Assert.Equal("Test Article", result.Title);
        Assert.Equal("Test Content", result.Description);
    }

    [Fact]
    public void Handle_ShouldThrowArticleNotFoundException_WhenArticleDoesNotExist()
    {
        // Arrange: Define a non-existent article ID
        var articleId = 99;

        // Mock the repository to return null (article not found)
        _mockArticlesRepository.Setup(repo => repo.GetById(articleId)).Returns((Articles)null);

        // Act & Assert: Ensure the handler throws an exception
        Assert.Throws<ArticleNotFoundException>(() => _handler.Handle(articleId));
    }
}
