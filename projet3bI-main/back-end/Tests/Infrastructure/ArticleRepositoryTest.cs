using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Tests.Infrastructure;

using Moq;
using Xunit;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;

public class ArticleRepositoryTest
{
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly Mock<DbSet<Articles>> _mockSet;
    private readonly ArticlesRepository _repository;

    public ArticleRepositoryTest()
    {
        _mockContext = new Mock<TradeShopContext>();
        _mockSet = new Mock<DbSet<Articles>>();

        // Setup the mock DbSet to be returned by the TradeShopContext
        _mockContext.Setup(m => m.Articles).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new ArticlesRepository(_mockContext.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnArticles()
    {
        // Arrange
        var articles = new List<Articles>
        {
            new Articles { ArticleId = 1, Title = "Article 1" },
            new Articles { ArticleId = 2, Title = "Article 2" }
        }.AsQueryable();

        _mockSet.As<IQueryable<Articles>>().Setup(m => m.Provider).Returns(articles.Provider);
        _mockSet.As<IQueryable<Articles>>().Setup(m => m.Expression).Returns(articles.Expression);
        _mockSet.As<IQueryable<Articles>>().Setup(m => m.ElementType).Returns(articles.ElementType);
        _mockSet.As<IQueryable<Articles>>().Setup(m => m.GetEnumerator()).Returns(articles.GetEnumerator());

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.NotNull(result); // Ensure result is not null
        Assert.Equal(2, result.Count); // Check if two articles are returned
        Assert.Equal("Article 1", result.First().Title); // First article should have the title "Article 1"
        Assert.Equal("Article 2", result.Last().Title); // Last article should have the title "Article 2"
    }

    [Fact]
    public void GetById_ShouldReturnArticle_WhenFound()
    {
        // Arrange
        var article = new Articles { ArticleId = 1, Title = "Article 1" };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns(article);

        // Act
        var result = _repository.GetById(1);

        // Assert
        Assert.NotNull(result); // Ensure the result is not null
        Assert.Equal(1, result.ArticleId); // Ensure the article ID is correct
        Assert.Equal("Article 1", result.Title); // Ensure the title is correct
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns((Articles)null);

        // Act
        var result = _repository.GetById(999);

        // Assert
        Assert.Null(result); // Ensure the result is null when article is not found
    }

    [Fact]
    public void Create_ShouldAddNewArticle()
    {
        // Arrange
        var newArticle = new Articles { ArticleId = 1, Title = "New Article", Price = 100 };

        _mockSet.Setup(m => m.Add(It.IsAny<Articles>())).Returns((Articles article) =>
        {
            article.ArticleId = 1; // Simulate auto-generated ID
            return article;
        });
        _mockContext.Setup(m => m.SaveChanges()).Returns(1); // Simulating successful save

        // Act
        var result = _repository.Create(newArticle);

        // Assert
        Assert.NotNull(result); // Ensure result is not null
        Assert.Equal(1, result.ArticleId); // Check if the ArticleId is set correctly
        Assert.Equal("New Article", result.Title); // Check if the title is correct
    }

    [Fact]
    public void Update_ShouldUpdateExistingArticle()
    {
        // Arrange
        var existingArticle = new Articles { ArticleId = 1, Title = "Old Title" };
        var updatedArticle = new Articles { ArticleId = 1, Title = "Updated Title" };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns(existingArticle);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1); // Simulating successful save

        // Act
        var result = _repository.Update(updatedArticle);

        // Assert
        Assert.True(result); // Ensure the update was successful
        Assert.Equal("Updated Title", existingArticle.Title); // Check if the title was updated
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenArticleNotFound()
    {
        // Arrange
        var updatedArticle = new Articles { ArticleId = 999, Title = "Non-existent Article" };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns((Articles)null);

        // Act
        var result = _repository.Update(updatedArticle);

        // Assert
        Assert.False(result); // Ensure the update returns false when article is not found
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenArticleIsDeleted()
    {
        // Arrange
        var article = new Articles { ArticleId = 1, Title = "Article to Delete" };

        // Mocking the DbSet methods
        var mockEntityEntry = new Mock<EntityEntry<Articles>>(); // Mocking EntityEntry
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns(article);
        _mockSet.Setup(m => m.Remove(It.IsAny<Articles>())).Returns(mockEntityEntry.Object); // Returning the mocked EntityEntry
        _mockContext.Setup(m => m.SaveChanges()).Returns(1); // Simulating successful delete

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the delete was successful
    }


    [Fact]
    public void Delete_ShouldReturnFalse_WhenArticleNotFound()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Articles, bool>>())).Returns((Articles)null);

        // Act
        var result = _repository.Delete(999);

        // Assert
        Assert.False(result); // Ensure delete returns false when article is not found
    }
}

