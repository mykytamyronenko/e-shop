using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.getById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class UserCommandControllerTest
{
    private readonly Mock<UserCommandsProcessor> _mockUserCommandsProcessor;
    private readonly Mock<UsersQueryProcessor> _mockUsersQueryProcessor;
    private readonly UserCommandsController _controller;

    public UserCommandControllerTest()
    {
        _mockUserCommandsProcessor = new Mock<UserCommandsProcessor>();
        _mockUsersQueryProcessor = new Mock<UsersQueryProcessor>();
        _controller = new UserCommandsController(_mockUserCommandsProcessor.Object, _mockUsersQueryProcessor.Object);
    }

    [Fact]
    public void Create_ReturnsOk_WhenUserIsCreated()
    {
        // Arrange
        var command = new BasicUserCreateCommand { Username = "TestUser", Email = "test@example.com", Password = "hashedpassword" };
        var output = new UserCreateOutput { Username = "TestUser", Email = "test@example.com" };

        _mockUserCommandsProcessor.Setup(p => p.CreateUser(command)).Returns(output);

        // Act
        var result = _controller.Create(command).Result as OkObjectResult;
        var actualOutput = result?.Value as UserCreateOutput;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.NotNull(actualOutput);
        Assert.Equal(output.Username, actualOutput.Username);
        Assert.Equal(output.Email, actualOutput.Email);
    }


    [Fact]
    public void Create_ReturnsBadRequest_WhenCommandIsInvalid()
    {
        // Act
        var result = _controller.Create(null).Result as BadRequestObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public void Create_ReturnsInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var command = new BasicUserCreateCommand { Username = "TestUser", Email = "test@example.com", Password = "hashedpassword" };
        _mockUserCommandsProcessor.Setup(p => p.CreateUser(command)).Throws(new Exception());

        // Act
        var result = _controller.Create(command).Result as ObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.StatusCode);
    }

    [Fact]
    public void CreateAdmin_ReturnsOk_WhenAdminIsCreated()
    {
        var command = new UserCreateCommand { Username = "AdminUser", Email = "admin@example.com", Password = "hashedpassword" };
        var output = new UserCreateOutput { Username = "AdminUser", Email = "admin@example.com" };

        _mockUserCommandsProcessor.Setup(p => p.CreateAdmin(command)).Returns(output);

        var result = _controller.CreateAdmin(command).Result as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal(output, result.Value);
    }

    [Fact]
    public void CreateAdmin_ReturnsBadRequest_WhenFileIsTooLarge()
    {
        var command = new UserCreateCommand { ProfilePicture = new FormFile(null, 0, 3 * 1024 * 1024, "ProfilePicture", "profile.jpg") };

        var result = _controller.CreateAdmin(command).Result as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public void DeleteUser_ReturnsNoContent_WhenUserIsDeleted()
    {
        var userId = 1;
        _mockUserCommandsProcessor.Setup(p => p.DeleteUser(userId));

        var result = _controller.DeleteUser(userId) as NoContentResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status204NoContent, result.StatusCode);
    }

    [Fact]
    public void DeleteUser_ReturnsNotFound_WhenUserDoesNotExist()
    {
        var userId = 1;
        _mockUserCommandsProcessor.Setup(p => p.DeleteUser(userId)).Throws(new InvalidOperationException());

        var result = _controller.DeleteUser(userId) as NotFoundObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

    [Fact]
    public void UpdateUser_ReturnsOk_WhenUserIsUpdated()
    {
        var command = new UserUpdateCommand { UserId = 1 };

        var result = _controller.UpdateUser2(command) as OkObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal("User updated.", result.Value);
    }

    [Fact]
    public void UpdateUser_ReturnsBadRequest_WhenCommandIsInvalid()
    {
        var result = _controller.UpdateUser2(null) as BadRequestObjectResult;

        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status400BadRequest, result.StatusCode);
    }

    [Fact]
    public void UpdateUserStatus_ReturnsOk_WhenUserStatusIsUpdated()
    {
        // Arrange
        var userId = 1;
        var input = new UserUpdateCommandStatus { Status = "active" };

        var user = new UsersGetByIdOutput 
        { 
            UserId = 1, 
            Username = "TestUser", 
            Email = "test@example.com", 
            Password = "hashedpassword", 
            ProfilePicture = "profile.png", 
            MembershipLevel = "Bronze", 
            Rating = 4.5f, 
            Status = "active", 
            Balance = 100.0m 
        };

        _mockUsersQueryProcessor.Setup(q => q.GetById(userId)).Returns(user);
        _mockUserCommandsProcessor.Setup(p => p.UpdateUser(It.IsAny<UserUpdateCommand>()));

        // Act
        var result = _controller.UpdateUserStatus(userId, input) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
    }


    [Fact]
    public void UpdateUserStatus_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 1;

        _mockUsersQueryProcessor
            .Setup(q => q.GetById(userId))
            .Returns((UsersGetByIdOutput)null);

        // Act
        var result = _controller.UpdateUserStatus(userId, new UserUpdateCommandStatus()) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
    }

}