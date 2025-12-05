using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

public class ArticleGetAllHandlerTest
{
    private readonly Mock<IArticlesRepository> _mockArticlesRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ArticlesGetAllHandler _handler;

    public ArticleGetAllHandlerTest()
    {
        // Create mocks for the repository and mapper
        _mockArticlesRepository = new Mock<IArticlesRepository>();
        _mockMapper = new Mock<IMapper>();

        // Inject the mocks into the handler
        _handler = new ArticlesGetAllHandler(_mockArticlesRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnArticles_WhenArticlesExist()
    {
        // Arrange
        var dbArticles = new List<Articles>
        {
            new Articles { ArticleId = 1, Title = "Article 1" },
            new Articles { ArticleId = 2, Title = "Article 2" }
        };

        // Mock repository method to return a list of articles
        _mockArticlesRepository.Setup(repo => repo.GetAll()).Returns(dbArticles);

        // Mock mapper method to map the articles
        _mockMapper.Setup(m => m.Map<List<ArticlesGetAllOutput.Articles>>(It.IsAny<List<Articles>>()))
                   .Returns(dbArticles.Select(a => new ArticlesGetAllOutput.Articles { ArticleId = a.ArticleId, Title = a.Title }).ToList());

        // Act
        var result = _handler.Handle(new ArticlesGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.ArticlesList.Count);
        Assert.Equal(1, result.ArticlesList[0].ArticleId);
        Assert.Equal("Article 1", result.ArticlesList[0].Title);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoArticlesExist()
    {
        // Arrange
        var dbArticles = new List<Articles>(); // No articles

        // Mock repository method to return an empty list
        _mockArticlesRepository.Setup(repo => repo.GetAll()).Returns(dbArticles);

        // Act
        var result = _handler.Handle(new ArticlesGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.ArticlesList); // Should return an empty list
    }
}
