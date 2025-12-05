using Application.Commands.Create;

namespace Tests.Application.Create;

using Moq;
using Xunit;
using System;
using System.IO;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Http;

public class UsersCreateHandlerTest
{
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly UserCreateHandler _handler;

    public UsersCreateHandlerTest()
    {
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();

        _handler = new UserCreateHandler(_mockUsersRepository.Object, _mockMapper.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserWithEmailOrUsernameAlreadyExists()
    {
        // Arrange
        var command = new BasicUserCreateCommand
        {
            Username = "existingUser",
            Email = "existingEmail@test.com",
            ProfilePicture = new FormFile(new MemoryStream(), 0, 0, "Profile", "image.jpg")
        };

        var existingUser = new Users { Username = "existingUser", Email = "existingEmail@test.com" };
        _mockContext.Setup(c => c.Users.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns(existingUser);

        // Act & Assert
        Assert.Throws<Exception>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldSuccessfullyCreateUser_WhenConditionsAreMet()
    {
        // Arrange
        var command = new BasicUserCreateCommand
        {
            Username = "newUser",
            Email = "newUser@test.com",
            ProfilePicture = new FormFile(new MemoryStream(), 0, 0, "Profile", "image.jpg")
        };

        var user = new Users
        {
            Username = "newUser",
            Email = "newUser@test.com",
            ProfilePicture = "profilePic/uploads/someFile.jpg"
        };

        _mockContext.Setup(c => c.Users.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns((Users)null);
        _mockMapper.Setup(m => m.Map<UserCreateOutput>(It.IsAny<Users>())).Returns(new UserCreateOutput());

        _mockUsersRepository.Setup(r => r.Create(It.IsAny<Users>()));

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        _mockContext.Verify(c => c.SaveChanges(), Times.Once); 
        _mockUsersRepository.Verify(r => r.Create(It.IsAny<Users>()), Times.Once);
    }

    [Fact]
    public void HandleAdmin_ShouldThrowException_WhenRoleIsInvalid()
    {
        // Arrange
        var command = new UserCreateCommand
        {
            Username = "adminUser",
            Email = "adminUser@test.com",
            Role = "invalid_role",
            ProfilePicture = new FormFile(new MemoryStream(), 0, 0, "Profile", "image.jpg")
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.HandleAdmin(command));
    }

    [Fact]
    public void HandleAdmin_ShouldSuccessfullyCreateAdminUser_WhenAllConditionsAreMet()
    {
        // Arrange
        var command = new UserCreateCommand
        {
            Username = "adminUser",
            Email = "adminUser@test.com",
            Role = "admin",
            Status = "active",
            ProfilePicture = new FormFile(new MemoryStream(), 0, 0, "Profile", "image.jpg")
        };

        var user = new Users
        {
            Username = "adminUser",
            Email = "adminUser@test.com",
            ProfilePicture = "profilePic/uploads/someFile.jpg",
            Role = "admin",
            Status = "active"
        };

        _mockContext.Setup(c => c.Users.FirstOrDefault(It.IsAny<Func<Users, bool>>())).Returns((Users)null);
        _mockMapper.Setup(m => m.Map<UserCreateOutput>(It.IsAny<Users>())).Returns(new UserCreateOutput());

        _mockUsersRepository.Setup(r => r.Create(It.IsAny<Users>()));

        // Act
        var result = _handler.HandleAdmin(command);

        // Assert
        Assert.NotNull(result);
        _mockContext.Verify(c => c.SaveChanges(), Times.Once);
        _mockUsersRepository.Verify(r => r.Create(It.IsAny<Users>()), Times.Once);
    }

    [Fact]
    public void HashPassword_ShouldReturnHashedPassword()
    {
        // Arrange
        var password = "TestPassword123";

        // Act
        var hashedPassword = _handler.HashPassword(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
    }
}
