using System.Dynamic;
using API.Controllers;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class ArticleQueryControllerTest
{
    private readonly Mock<ArticlesQueryProcessor> _mockArticlesQueryProcessor;
    private readonly ArticleQueryController _controller;

    public ArticleQueryControllerTest()
    {
        _mockArticlesQueryProcessor = new Mock<ArticlesQueryProcessor>();
        _controller = new ArticleQueryController(_mockArticlesQueryProcessor.Object);
    }

    [Fact]
    public void GetAllArticles_ReturnsOk_WhenArticlesExist()
    {
        // Arrange
        var articlesList = new List<ArticlesGetAllOutput.Articles>
        {
            new ArticlesGetAllOutput.Articles { ArticleId = 1, Title = "Article 1", Description = "Description 1" },
            new ArticlesGetAllOutput.Articles { ArticleId = 2, Title = "Article 2", Description = "Description 2" }
        };

        var articlesOutput = new ArticlesGetAllOutput { ArticlesList = articlesList };

        var query = new ArticlesGetAllQuery();

        _mockArticlesQueryProcessor.Setup(q => q.GetAll(It.IsAny<ArticlesGetAllQuery>())).Returns(articlesOutput);

        // Act
        var result = _controller.GetAllArticles();

        // Assert
        var actionResult = Assert.IsType<List<ArticlesGetAllOutput.Articles>>(result);
        Assert.Equal(2, actionResult.Count);
    }


    [Fact]
    public void GetByIdArticle_ReturnsOk_WhenArticleFound()
    {
        // Arrange
        var articleId = 1;
        var articleOutput = new ArticlesGetByIdOutput
        {
            ArticleId = articleId,
            Title = "Article 1",
            Description = "Description 1"
        };
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Returns(articleOutput);

        // Act
        var result = _controller.GetByIdArticle(articleId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnValue = Assert.IsType<ArticlesGetByIdOutput>(actionResult.Value);
        Assert.Equal(articleId, returnValue.ArticleId);
    }

    [Fact]
    public void GetByIdArticle_ReturnsNotFound_WhenArticleNotFound()
    {
        // Arrange
        var articleId = 999;
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Throws(new ArticleNotFoundException(articleId));

        // Act
        var result = _controller.GetByIdArticle(articleId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("Article not found", actionResult.Value);
    }

    [Fact]
    public void GetTitleById_ReturnsOk_WhenArticleFound()
    {
        // Arrange
        var articleId = 1;
        var articleOutput = new ArticlesGetByIdOutput
        {
            ArticleId = articleId,
            Title = "Article 1",
            Description = "Description 1"
        };
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Returns(articleOutput);

        // Act
        var result = _controller.GetTitleById(articleId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<ExpandoObject>(actionResult.Value);
        var title = returnValue.GetType().GetProperty("title").GetValue(returnValue);
        Assert.Equal("Article 1", title);
    }

    [Fact]
    public void GetTitleById_ReturnsNotFound_WhenArticleNotFound()
    {
        // Arrange
        var articleId = 999;
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Throws(new ArticleNotFoundException(articleId));

        // Act
        var result = _controller.GetTitleById(articleId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        var returnValue = Assert.IsType<ExpandoObject>(actionResult.Value);
        var message = returnValue.GetType().GetProperty("message").GetValue(returnValue);
        Assert.Equal("Article not found", message);
    }
}
