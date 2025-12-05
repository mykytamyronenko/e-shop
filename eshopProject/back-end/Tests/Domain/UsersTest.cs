namespace Tests.Domain;

public class UsersTest
{
    [Fact]
    public void Username_ShouldThrowArgumentException_WhenUsernameIsTooShort()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Username = "a");  // Username is less than 3 characters
    }

    [Fact]
    public void Username_ShouldThrowArgumentException_WhenUsernameContainsInvalidCharacters()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Username = "invalid username"); // Contains spaces
        Assert.Throws<ArgumentException>(() => user.Username = "invalid@username"); // Contains @
        Assert.Throws<ArgumentException>(() => user.Username = "invalid#username"); // Contains #
    }

    [Fact]
    public void Username_ShouldSetValue_WhenValidUsername()
    {
        // Arrange
        var user = new Users();

        // Act
        user.Username = "valid_username";

        // Assert
        Assert.Equal("valid_username", user.Username);
    }
    
    [Fact]
    public void Email_ShouldThrowArgumentException_WhenEmailIsInvalid()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Email = "invalidemail"); // Invalid email format
        Assert.Throws<ArgumentException>(() => user.Email = "invalid@domain"); // Invalid email format
    }

    [Fact]
    public void Email_ShouldThrowArgumentException_WhenEmailIsNullOrEmpty()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Email = "");  // Empty email
        Assert.Throws<ArgumentException>(() => user.Email = null); // Null email
    }

    [Fact]
    public void Email_ShouldSetValue_WhenValidEmail()
    {
        // Arrange
        var user = new Users();

        // Act
        user.Email = "valid@example.com";

        // Assert
        Assert.Equal("valid@example.com", user.Email);
    }
    
    [Fact]
    public void Role_ShouldThrowArgumentException_WhenRoleIsInvalid()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Role = "guest");  // Invalid role
        Assert.Throws<ArgumentException>(() => user.Role = "moderator"); // Invalid role
    }

    [Fact]
    public void Role_ShouldSetValue_WhenValidRole()
    {
        // Arrange
        var user = new Users();

        // Act
        user.Role = "admin";
        user.Role = "connected_user";

        // Assert
        Assert.Equal("admin", user.Role);
        Assert.Equal("connected_user", user.Role);
    }

    [Fact]
    public void MembershipLevel_ShouldThrowArgumentException_WhenMembershipLevelIsInvalid()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.MembershipLevel = "Platinum"); // Invalid membership level
        Assert.Throws<ArgumentException>(() => user.MembershipLevel = "Diamond");  // Invalid membership level
    }

    [Fact]
    public void MembershipLevel_ShouldSetValue_WhenValidMembershipLevel()
    {
        // Arrange
        var user = new Users();

        // Act
        user.MembershipLevel = "Bronze";
        user.MembershipLevel = "Silver";
        user.MembershipLevel = "Gold";

        // Assert
        Assert.Equal("Bronze", user.MembershipLevel);
        Assert.Equal("Silver", user.MembershipLevel);
        Assert.Equal("Gold", user.MembershipLevel);
    }

    [Fact]
    public void Rating_ShouldThrowArgumentException_WhenRatingIsOutOfBounds()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Rating = -1); // Rating less than 0
        Assert.Throws<ArgumentException>(() => user.Rating = 6);  // Rating greater than 5
    }

    [Fact]
    public void Rating_ShouldSetValue_WhenValidRating()
    {
        // Arrange
        var user = new Users();

        // Act
        user.Rating = 0;
        user.Rating = 5;

        // Assert
        Assert.Equal(0, user.Rating);
        Assert.Equal(5, user.Rating);
    }

    [Fact]
    public void Status_ShouldThrowArgumentException_WhenStatusIsInvalid()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Status = "inactive");  // Invalid status
        Assert.Throws<ArgumentException>(() => user.Status = "banned");    // Invalid status
    }

    [Fact]
    public void Status_ShouldSetValue_WhenValidStatus()
    {
        // Arrange
        var user = new Users();

        // Act
        user.Status = "active";
        user.Status = "suspended";
        user.Status = "deleted";

        // Assert
        Assert.Equal("active", user.Status);
        Assert.Equal("suspended", user.Status);
        Assert.Equal("deleted", user.Status);
    }

    [Fact]
    public void Balance_ShouldThrowArgumentException_WhenBalanceIsNegative()
    {
        // Arrange
        var user = new Users();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => user.Balance = -1); // Negative balance
    }

    [Fact]
    public void Balance_ShouldSetValue_WhenBalanceIsPositive()
    {
        // Arrange
        var user = new Users();

        // Act
        user.Balance = 0;
        user.Balance = 100;

        // Assert
        Assert.Equal(0, user.Balance);
        Assert.Equal(100, user.Balance);
    }



}