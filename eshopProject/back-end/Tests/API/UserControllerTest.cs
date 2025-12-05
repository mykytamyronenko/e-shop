using API.Controllers;
using Application.Dtos;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class UserControllerTest
{
    private readonly UserController _controller;
    private readonly Mock<UserService> _mockUserService;

    public UserControllerTest()
    {
        _mockUserService = new Mock<UserService>();
        _controller = new UserController(_mockUserService.Object);
        
        // Set up the controller context to mock the response
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
    }

    [Fact]
    public void Login_ReturnsOkResult_WithToken_WhenUserIsAuthenticated()
    {
        // Arrange
        var userInput = new DtoInputUser { Username = "test@example.com", Password = "password123" };
        var token = "mock-jwt-token";
    
        // Mock du service UserService
        _mockUserService
            .Setup(service => service.Login(userInput))
            .Returns(token);

        // Act
        var result = _controller.Login(userInput) as ActionResult<DtoOutputConnectedUser>;

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        var responseData = Assert.IsType<DtoOutputConnectedUser>(okResult.Value);
        Assert.Equal(token, responseData.Token);
    }



    [Fact]
    public void Login_ReturnsUnauthorizedResult_WhenAuthenticationFails()
    {
        // Arrange
        var userInput = new DtoInputUser { Username = "invalid@example.com", Password = "wrongpassword" };
    
        _mockUserService
            .Setup(service => service.Login(userInput))
            .Returns((string)null);

        // Act
        var result = _controller.Login(userInput) as ActionResult<DtoOutputConnectedUser>;

        // Assert
        Assert.NotNull(result);
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedResult.StatusCode);
        Assert.Equal("Email, username or password incorrect.", unauthorizedResult.Value);
    }


    [Fact]
    public void Login_SetsCookie_WithCorrectToken_WhenUserIsAuthenticated()
    {
        // Arrange
        var userInput = new DtoInputUser { Username = "test@example.com", Password = "password123" };
        var token = "mock-jwt-token";
    
        _mockUserService
            .Setup(service => service.Login(userInput))
            .Returns(token);

        var cookieOptions = new CookieOptions
        {
            Secure = true,
            HttpOnly = false
        };

        // Act
        _controller.Login(userInput);

        // Assert
        Assert.True(_controller.Response.Headers.ContainsKey("Set-Cookie"));
    
        var cookies = _controller.Response.Headers["Set-Cookie"];
        var cookieValue = cookies.FirstOrDefault(c => c.Contains("cookie="));
        Assert.NotNull(cookieValue);
        Assert.Contains(token, cookieValue);
    }


    [Fact]
    public void Logout_DeletesCookie_AndReturnsOkResult()
    {
        // Arrange
        _controller.Response.Cookies.Append("cookie", "some-value");

        // Act
        var result = _controller.Logout() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        Assert.Equal("Logged out successfully", ((dynamic)result.Value).message);

        Assert.True(_controller.Response.Headers.ContainsKey("Set-Cookie"));

        var cookies = _controller.Response.Headers["Set-Cookie"];
        var deletedCookie = cookies.FirstOrDefault(c => c.Contains("cookie=;"));
        Assert.NotNull(deletedCookie);
        Assert.Contains("Max-Age=0", deletedCookie);
    }


}