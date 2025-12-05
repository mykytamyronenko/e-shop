using Application.utils;

namespace Tests.Domain;

public class ArticlesTest
{
    [Fact]
    public void Title_ShouldThrowArgumentException_WhenNullOrEmpty()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.Title = null);
        Assert.Throws<ArgumentException>(() => article.Title = string.Empty);
        Assert.Throws<ArgumentException>(() => article.Title = " ");
    }

    [Fact]
    public void Title_ShouldSetValue_WhenValidTitle()
    {
        // Arrange
        var article = new Articles();
        var validTitle = "Valid Title";

        // Act
        article.Title = validTitle;

        // Assert
        Assert.Equal(validTitle, article.Title);
    }
    
    [Fact]
    public void Description_ShouldThrowArgumentException_WhenNullOrEmpty()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.Description = null);
        Assert.Throws<ArgumentException>(() => article.Description = string.Empty);
        Assert.Throws<ArgumentException>(() => article.Description = " ");
    }

    [Fact]
    public void Description_ShouldSetValue_WhenValidDescription()
    {
        // Arrange
        var article = new Articles();
        var validDescription = "Valid description";

        // Act
        article.Description = validDescription;

        // Assert
        Assert.Equal(validDescription, article.Description);
    }
    
    [Fact]
    public void Price_ShouldThrowArgumentException_WhenLessThanOrEqualToOne()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.Price = 1);
        Assert.Throws<ArgumentException>(() => article.Price = 0);
    }

    [Fact]
    public void Price_ShouldSetValue_WhenValidPrice()
    {
        // Arrange
        var article = new Articles();
        var validPrice = 10.99m;

        // Act
        article.Price = validPrice;

        // Assert
        Assert.Equal(validPrice, article.Price);
    }
    
    [Fact]
    public void Category_ShouldThrowArgumentException_WhenInvalidCategory()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.Category = (ArticleCategory)999); // invalid enum value
    }
    
    [Fact]
    public void State_ShouldThrowArgumentException_WhenInvalidState()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.State = "broken");
    }

    [Fact]
    public void State_ShouldSetValue_WhenValidState()
    {
        // Arrange
        var article = new Articles();

        // Act
        article.State = "new";

        // Assert
        Assert.Equal("new", article.State);

        // Act
        article.State = "used";

        // Assert
        Assert.Equal("used", article.State);
    }

    [Fact]
    public void CreatedAt_ShouldThrowArgumentException_WhenDateInFuture()
    {
        // Arrange
        var article = new Articles();
        var futureDate = DateTime.Now.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.CreatedAt = futureDate);
    }

    [Fact]
    public void CreatedAt_ShouldSetValue_WhenValidDate()
    {
        // Arrange
        var article = new Articles();
        var validDate = DateTime.Now;

        // Act
        article.CreatedAt = validDate;

        // Assert
        Assert.Equal(validDate, article.CreatedAt);
    }
    
    [Fact]
    public void UpdatedAt_ShouldThrowArgumentException_WhenEarlierThanCreatedAt()
    {
        // Arrange
        var article = new Articles();
        var validDate = DateTime.Now;
        var pastDate = DateTime.Now.AddMinutes(-1);

        // Act
        article.CreatedAt = validDate;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.UpdatedAt = pastDate);
    }

    [Fact]
    public void UpdatedAt_ShouldSetValue_WhenValidDate()
    {
        // Arrange
        var article = new Articles();
        var validDate = DateTime.Now;

        // Act
        article.CreatedAt = validDate;
        article.UpdatedAt = validDate.AddMinutes(1); // Should be after CreatedAt

        // Assert
        Assert.Equal(validDate.AddMinutes(1), article.UpdatedAt);
    }
    
    [Fact]
    public void Status_ShouldThrowArgumentException_WhenInvalidStatus()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.Status = "unknown");
    }

    [Fact]
    public void Status_ShouldSetValue_WhenValidStatus()
    {
        // Arrange
        var article = new Articles();

        // Act
        article.Status = "available";

        // Assert
        Assert.Equal("available", article.Status);

        // Act
        article.Status = "sold";

        // Assert
        Assert.Equal("sold", article.Status);

        // Act
        article.Status = "removed";

        // Assert
        Assert.Equal("removed", article.Status);
    }

    [Fact]
    public void MainImageUrl_ShouldThrowArgumentException_WhenNullOrEmpty()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.MainImageUrl = null);
        Assert.Throws<ArgumentException>(() => article.MainImageUrl = string.Empty);
        Assert.Throws<ArgumentException>(() => article.MainImageUrl = " ");
    }

    [Fact]
    public void MainImageUrl_ShouldSetValue_WhenValidUrl()
    {
        // Arrange
        var article = new Articles();
        var validUrl = "https://example.com/image.jpg";

        // Act
        article.MainImageUrl = validUrl;

        // Assert
        Assert.Equal(validUrl, article.MainImageUrl);
    }

    [Fact]
    public void Quantity_ShouldThrowArgumentException_WhenLessThanZero()
    {
        // Arrange
        var article = new Articles();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => article.Quantity = -1);
    }

    [Fact]
    public void Quantity_ShouldSetValue_WhenValidQuantity()
    {
        // Arrange
        var article = new Articles();
        var validQuantity = 10;

        // Act
        article.Quantity = validQuantity;

        // Assert
        Assert.Equal(validQuantity, article.Quantity);
    }




}