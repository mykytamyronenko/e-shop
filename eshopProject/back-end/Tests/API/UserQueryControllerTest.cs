using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Application.Queries.getByName;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class UserQueryControllerTest
{
    private readonly Mock<UsersQueryProcessor> _mockUsersQueryProcessor;
    private readonly UserQueryController _controller;

    public UserQueryControllerTest()
    {
        _mockUsersQueryProcessor = new Mock<UsersQueryProcessor>();
        _controller = new UserQueryController(_mockUsersQueryProcessor.Object);
    }
    
    [Fact]
    public void GetAll_ReturnsOk_WhenUsersExist()
    {
        // Arrange
        var expectedUsers = new List<UsersGetAllOutput.Users>
        {
            new UsersGetAllOutput.Users { UserId = 1, Username = "user1" },
            new UsersGetAllOutput.Users { UserId = 2, Username = "user2" }
        };

        _mockUsersQueryProcessor
            .Setup(p => p.GetAll(null))
            .Returns(new UsersGetAllOutput { UsersList = expectedUsers });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<List<UsersGetAllOutput.Users>>(result);
        Assert.Equal(expectedUsers.Count, okResult.Count);
        Assert.Equal(expectedUsers[0].Username, okResult[0].Username);
    }

    [Fact]
    public void GetAll_ReturnsForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1"), new Claim(ClaimTypes.Role, "user") }))
            }
        };

        // Act
        var result = _controller.GetAll();

        // Assert
        var forbiddenResult = Assert.IsType<ForbidResult>(result);
    }
    
    [Fact]
    public void GetById_ReturnsOk_WhenUserFoundAndAuthorized()
    {
        // Arrange
        var userIdFromToken = 1;
        var userIdToFetch = 1;
        var expectedUser = new UsersGetByIdOutput { UserId = userIdToFetch, Username = "user1" };

        _mockUsersQueryProcessor
            .Setup(p => p.GetById(userIdToFetch))
            .Returns(expectedUser);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", userIdFromToken.ToString()) }))
            }
        };

        // Act
        var result = _controller.GetById(userIdToFetch);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var userOutput = Assert.IsType<UsersGetByIdOutput>(okResult.Value);
        Assert.Equal(expectedUser.UserId, userOutput.UserId);
    }

    [Fact]
    public void GetById_ReturnsForbidden_WhenUserIsNotAuthorized()
    {
        // Arrange
        var userIdFromToken = 1;
        var userIdToFetch = 2; // Trying to access another user's info

        _mockUsersQueryProcessor
            .Setup(p => p.GetById(userIdToFetch))
            .Returns(new UsersGetByIdOutput { UserId = userIdToFetch, Username = "user2" });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", userIdFromToken.ToString()) }))
            }
        };

        // Act
        var result = _controller.GetById(userIdToFetch);

        // Assert
        var forbidResult = Assert.IsType<ForbidResult>(result.Result);
    }

    [Fact]
    public void GetById_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var invalidId = -1;

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetById(invalidId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("User ID must be greater than zero.", badRequestResult.Value);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = 999; // Non-existing user ID
        _mockUsersQueryProcessor
            .Setup(p => p.GetById(userId))
            .Throws(new UserNotFoundException("User not found"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetById(userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public void GetByUsername_ReturnsOk_WhenUserFound()
    {
        // Arrange
        var username = "user1";
        var expectedUser = new UsersGetByUsernameOutput { Username = username, UserId = 1 };

        _mockUsersQueryProcessor
            .Setup(p => p.GetByUsername(username))
            .Returns(expectedUser);

        // Act
        var result = _controller.GetByUsername(username);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var userOutput = Assert.IsType<UsersGetByUsernameOutput>(okResult.Value);
        Assert.Equal(username, userOutput.Username);
    }

    [Fact]
    public void GetByUsername_ReturnsNotFound_WhenUserNotFound()
    {
        // Arrange
        var username = "nonexistentuser";
        _mockUsersQueryProcessor
            .Setup(p => p.GetByUsername(username))
            .Throws(new UserNotFoundException("User not found"));

        // Act
        var result = _controller.GetByUsername(username);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("User not found", notFoundResult.Value);
    }

    [Fact]
    public void GetUsernameById_ReturnsOk_WhenUserFound()
    {
        // Arrange
        var userId = 1;
        var expectedUser = new UsersGetByIdOutput { UserId = userId, Username = "user1" };

        _mockUsersQueryProcessor
            .Setup(p => p.GetById(userId))
            .Returns(expectedUser);

        // Act
        var result = _controller.GetUsernameById(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<IDictionary<string, string>>(okResult.Value);
        Assert.Equal("user1", response["username"]);
    }

    [Fact]
    public void GetUsernameById_ReturnsNotFound_WhenUserNotFound()
    {
        // Arrange
        var userId = 999; // Non-existing user ID
        _mockUsersQueryProcessor
            .Setup(p => p.GetById(userId))
            .Throws(new UserNotFoundException("User not found"));

        // Act
        var result = _controller.GetUsernameById(userId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        var response = Assert.IsType<IDictionary<string, string>>(notFoundResult.Value);
        Assert.Equal("User not found", response["message"]);
    }


}