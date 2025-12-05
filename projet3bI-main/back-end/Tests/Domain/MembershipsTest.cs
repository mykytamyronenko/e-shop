namespace Tests.Domain;

public class MembershipsTest
{
    [Fact]
    public void Name_ShouldThrowArgumentException_WhenInvalidMembershipName()
    {
        // Arrange
        var membership = new Memberships();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => membership.Name = "Platinum");
    }

    [Fact]
    public void Name_ShouldSetValue_WhenValidMembershipName()
    {
        // Arrange
        var membership = new Memberships();
            
        // Act
        membership.Name = "Bronze";
        membership.Name = "Silver";
        membership.Name = "Gold";

        // Assert
        Assert.Equal("Bronze", membership.Name);
        Assert.Equal("Silver", membership.Name);
        Assert.Equal("Gold", membership.Name);
    }
    
    [Fact]
    public void Price_ShouldThrowArgumentException_WhenLessThanOne()
    {
        // Arrange
        var membership = new Memberships();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => membership.Price = 0);
        Assert.Throws<ArgumentException>(() => membership.Price = -1);
    }

    [Fact]
    public void Price_ShouldSetValue_WhenValidPrice()
    {
        // Arrange
        var membership = new Memberships();
        var validPrice = 50.99m;

        // Act
        membership.Price = validPrice;

        // Assert
        Assert.Equal(validPrice, membership.Price);
    }
    
    [Fact]
    public void DiscountPercentage_ShouldThrowArgumentException_WhenInvalidDiscountPercentage()
    {
        // Arrange
        var membership = new Memberships();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => membership.DiscountPercentage = -0.1m); // Less than 0
        Assert.Throws<ArgumentException>(() => membership.DiscountPercentage = 1.1m);  // Greater than 1
    }

    [Fact]
    public void DiscountPercentage_ShouldSetValue_WhenValidDiscountPercentage()
    {
        // Arrange
        var membership = new Memberships();

        // Act
        membership.DiscountPercentage = 0.0m; // No discount
        membership.DiscountPercentage = 0.5m; // 50% discount
        membership.DiscountPercentage = 1.0m; // 100% discount

        // Assert
        Assert.Equal(0.0m, membership.DiscountPercentage);
        Assert.Equal(0.5m, membership.DiscountPercentage);
        Assert.Equal(1.0m, membership.DiscountPercentage);
    }
    [Fact]
    public void Description_ShouldThrowArgumentException_WhenNullOrEmpty()
    {
        // Arrange
        var membership = new Memberships();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => membership.Description = null);
        Assert.Throws<ArgumentException>(() => membership.Description = string.Empty);
        Assert.Throws<ArgumentException>(() => membership.Description = " ");
    }

    [Fact]
    public void Description_ShouldSetValue_WhenValidDescription()
    {
        // Arrange
        var membership = new Memberships();
        var validDescription = "A great membership with multiple benefits.";

        // Act
        membership.Description = validDescription;

        // Assert
        Assert.Equal(validDescription, membership.Description);
    }




}