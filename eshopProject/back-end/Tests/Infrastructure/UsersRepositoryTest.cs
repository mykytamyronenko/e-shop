namespace Tests.Infrastructure;

using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class UsersRepositoryTest
{
    private readonly Mock<DbSet<Users>> _mockSet;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly UsersRepository _repository;

    public UsersRepositoryTest()
    {
        // Initialize mocks
        _mockSet = new Mock<DbSet<Users>>();
        _mockContext = new Mock<TradeShopContext>();

        // Setup the context to return the mocked DbSet
        _mockContext.Setup(m => m.Users).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new UsersRepository(_mockContext.Object);
    }

    [Fact]
    public void Create_ShouldAddUserAndReturnCreatedEntity()
    {
        // Arrange
        var newUser = new Users
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.Add(It.IsAny<Users>())).Returns((Users user) => user);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Create(newUser);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal("test@example.com", result.Email);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var existingUser = new Users
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns(existingUser);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Update(existingUser);

        // Assert
        Assert.True(result); // Ensure the user was updated successfully
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var nonExistingUser = new Users
        {
            UserId = 999,
            Username = "nonexistinguser",
            Email = "nonexisting@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "Active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns((Users)null);

        // Act
        var result = _repository.Update(nonExistingUser);

        // Assert
        Assert.False(result); // Ensure the update did not occur for a non-existing user
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var existingUser = new Users
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns(existingUser);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the user was deleted successfully
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns((Users)null);

        // Act
        var result = _repository.Delete(999); // Non-existing user ID

        // Assert
        Assert.False(result); // Ensure the delete did not occur for a non-existing user
    }

    [Fact]
    public void GetById_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new Users
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns(existingUser);

        // Act
        var result = _repository.GetById(1);

        // Assert
        Assert.NotNull(result); // Ensure the user was found
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns((Users)null);

        // Act
        var result = _repository.GetById(999); // Non-existing user ID

        // Assert
        Assert.Null(result); // Ensure null is returned for a non-existing user
    }

    [Fact]
    public void GetUserByUsername_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new Users
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.SingleOrDefault(It.IsAny<Func<Users, bool>>())).Returns(existingUser);

        // Act
        var result = _repository.GetUserByUsername("testuser");

        // Assert
        Assert.NotNull(result); // Ensure the user was found
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public void GetUserByEmail_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var existingUser = new Users
        {
            UserId = 1,
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            Role = "connected_user",
            ProfilePicture = null,
            MembershipLevel = "Bronze",
            Rating = 5,
            Status = "active",
            Balance = 100.0m
        };

        _mockSet.Setup(m => m.SingleOrDefault(It.IsAny<Func<Users, bool>>())).Returns(existingUser);

        // Act
        var result = _repository.GetUserByEmail("test@example.com");

        // Assert
        Assert.NotNull(result); // Ensure the user was found
        Assert.Equal("test@example.com", result.Email);
    }
}
