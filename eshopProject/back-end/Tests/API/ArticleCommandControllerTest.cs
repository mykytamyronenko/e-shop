using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.getById;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.API;

public class ArticleCommandControllerTest
{
    private readonly Mock<ArticleCommandsProcessor> _mockArticleCommandsProcessor;
    private readonly Mock<ArticlesQueryProcessor> _mockArticlesQueryProcessor;
    private readonly ArticleCommandsController _controller;
    
    public ArticleCommandControllerTest()
    {
        _mockArticleCommandsProcessor = new Mock<ArticleCommandsProcessor>();
        _mockArticlesQueryProcessor = new Mock<ArticlesQueryProcessor>();
        
        _controller = new ArticleCommandsController(
            _mockArticleCommandsProcessor.Object,
            _mockArticlesQueryProcessor.Object
        );
    }
    
    [Fact]
    public void CreateArticle_ReturnsOk_WhenArticleCreatedSuccessfully()
    {
        // Arrange
        var mockArticleCreateCommand = new ArticleCreateCommand { UserId = 1, Title = "Test Article", Description = "Test", Price = 10 };
        var mockResult = new ArticleCreateOutput {UserId  = 1 };

        _mockArticleCommandsProcessor.Setup(p => p.CreateArticle(It.IsAny<ArticleCreateCommand>())).Returns(mockResult);

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", "1") }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = principal } };

        // Act
        var result = _controller.CreateArticle(mockArticleCreateCommand);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(mockResult, okResult.Value);
    }
    
    [Fact]
    public void CreateArticle_ReturnsBadRequest_WhenCommandIsNull()
    {
        // Arrange
        ArticleCreateCommand command = null;

        // Act
        var result = _controller.CreateArticle(command);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }
    
    [Fact]
    public void CreateArticle_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var mockArticleCreateCommand = new ArticleCreateCommand { UserId = 1, Title = "Test Article", Description = "Test", Price = 10 };
        _mockArticleCommandsProcessor.Setup(p => p.CreateArticle(It.IsAny<ArticleCreateCommand>())).Throws(new Exception("Internal error"));

        // Act
        var result = _controller.CreateArticle(mockArticleCreateCommand);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }
    
    [Fact]
    public void UpdateArticle_ReturnsOk_WhenArticleUpdatedSuccessfully()
    {
        // Arrange
        var command = new ArticleUpdateCommand { ArticleId = 1, UserId = 1, Title = "Updated Title" };

        var mockArticleOutput = new ArticlesGetByIdOutput
        {
            ArticleId = 1,
            UserId = 1,
            Title = "Old Title",
            Description = "Old Description",
            Price = 100.0m,
            Category = "Other",
            State = "new",
            CreatedAt = DateTime.Now.AddDays(-1),
            UpdatedAt = DateTime.Now,
            Status = "available",
            MainImageUrl = "http://example.com/image.jpg",
            Quantity = 10
        };

        _mockArticlesQueryProcessor.Setup(q => q.GetById(1)).Returns(mockArticleOutput);

        _mockArticleCommandsProcessor.Setup(p => p.UpdateArticle(It.IsAny<ArticleUpdateCommand>()));

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", "1") }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = principal } };

        // Act
        var result = _controller.UpdateArticle(command);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = okResult.Value as dynamic;
        Assert.Equal("Article updated.", value.message);

        _mockArticleCommandsProcessor.Verify(p => p.UpdateArticle(It.IsAny<ArticleUpdateCommand>()), Times.Once);
    }

    [Fact]
    public void UpdateArticle_ReturnsUnauthorized_WhenUserIsNotAuthorized()
    {
        // Arrange
        var command = new ArticleUpdateCommand { ArticleId = 1, UserId = 1, Title = "Updated Title" };
        var article = new ArticlesGetByIdOutput { UserId = 2, ArticleId = 1 };
        _mockArticlesQueryProcessor.Setup(q => q.GetById(1)).Returns(article);

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", "1") }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = principal } };
            
        // Act
        var result = _controller.UpdateArticle(command);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void DeleteArticle_ReturnsNoContent_WhenArticleDeletedSuccessfully()
    {
        // Arrange
        var articleId = 1;
    
        var article = new ArticlesGetByIdOutput
        {
            ArticleId = articleId,
            UserId = 1,
            Title = "Sample Title",
            Description = "Sample Description",
            Price = 9.99m,
            Category = "Other",
            State = "new",
            Status = "active",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            MainImageUrl = "http://example.com/image.jpg",
            Quantity = 5
        };
    
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Returns(article);
    
        _mockArticleCommandsProcessor.Setup(p => p.DeleteArticle(articleId));

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", "1") }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = principal } };

        // Act
        var result = _controller.DeleteArticle(articleId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }


    [Fact]
    public void DeleteArticle_ReturnsNotFound_WhenArticleDoesNotExist()
    {
        // Arrange
        var articleId = 999;
    
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Returns((ArticlesGetByIdOutput)null);

        // Act
        var result = _controller.DeleteArticle(articleId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }


    [Fact]
    public void UpdateArticleStatus_ReturnsOk_WhenArticleStatusUpdatedSuccessfully()
    {
        // Arrange
        var articleId = 1;
        var input = new ArticleUpdateCommandStatus { Status = "removed" };
    
        var article = new ArticlesGetByIdOutput
        {
            ArticleId = articleId,
            UserId = 1,
            Status = "available",
            Title = "Article Title",
            Description = "Article Description",
            Price = 10.99m,
            Category = "Other",
            State = "new",
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            MainImageUrl = "http://example.com/image.jpg",
            Quantity = 5
        };

        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Returns(article);
    
        _mockArticleCommandsProcessor.Setup(p => p.UpdateArticle(It.IsAny<ArticleUpdateCommand>()));

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", "1") }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() { User = principal } };

        // Act
        var result = _controller.UpdateArticleStatus(articleId, input);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }


    [Fact]
    public void UpdateArticleStatus_ReturnsNotFound_WhenArticleNotFound()
    {
        // Arrange
        var articleId = 999;
        var input = new ArticleUpdateCommandStatus { Status = "removed" };
        _mockArticlesQueryProcessor.Setup(q => q.GetById(articleId)).Returns((ArticlesGetByIdOutput)null);

        // Act
        var result = _controller.UpdateArticleStatus(articleId, input);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    
   





    
    




}