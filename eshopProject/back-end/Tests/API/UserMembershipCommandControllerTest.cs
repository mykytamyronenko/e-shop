using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.getById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class UserMembershipCommandControllerTest
{
    
    private readonly Mock<UserMembershipCommandsProcessor> _mockUserMembershipCommandsProcessor;
    private readonly Mock<UserMembershipsQueryProcessor> _mockUserMembershipsQueryProcessor;
    private readonly UserMembershipCommandsController _controller;

    public UserMembershipCommandControllerTest()
    {
        _mockUserMembershipCommandsProcessor = new Mock<UserMembershipCommandsProcessor>();
        _mockUserMembershipsQueryProcessor = new Mock<UserMembershipsQueryProcessor>();
        _controller = new UserMembershipCommandsController(_mockUserMembershipCommandsProcessor.Object, _mockUserMembershipsQueryProcessor.Object);
    }
    
    [Fact]
    public void CreateUserMembership_ReturnsOk_WhenMembershipCreatedSuccessfully()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipCommand = new UserMembershipCreateCommand { UserId = userIdFromToken };
        var expectedOutput = new UserMembershipCreateOutput
        {
            UserId = userIdFromToken,
            MembershipId = 101,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddYears(1),
            Status = "active"
        };

        _mockUserMembershipCommandsProcessor
            .Setup(p => p.CreateUserMembership(userMembershipCommand))
            .Returns(expectedOutput);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.CreateUserMembership(userMembershipCommand);

        // Assert
        Assert.NotNull(result);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
    
        var responseData = Assert.IsType<UserMembershipCreateOutput>(okResult.Value);
    
        Assert.Equal(expectedOutput.UserId, responseData.UserId);
        Assert.Equal(expectedOutput.MembershipId, responseData.MembershipId);
        Assert.Equal(expectedOutput.StartDate.Date, responseData.StartDate.Date);
        Assert.Equal(expectedOutput.EndDate.Date, responseData.EndDate.Date);
        Assert.Equal(expectedOutput.Status, responseData.Status);
    }




    [Fact]
    public void CreateUserMembership_ReturnsBadRequest_WhenCommandIsNull()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = _controller.CreateUserMembership(null);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void CreateUserMembership_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipCommand = new UserMembershipCreateCommand { UserId = userIdFromToken };
        
        _mockUserMembershipCommandsProcessor
            .Setup(p => p.CreateUserMembership(userMembershipCommand))
            .Throws(new Exception("Internal error"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.CreateUserMembership(userMembershipCommand);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(StatusCodes.Status500InternalServerError, objectResult.StatusCode);
        Assert.Equal("Internal error", objectResult.Value);
    }


    [Fact]
    public void UpdateUserMembership_ReturnsOk_WhenMembershipUpdatedSuccessfully()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipCommand = new UserMembershipUpdateCommand { UserId = userIdFromToken };
        
        _mockUserMembershipCommandsProcessor
            .Setup(p => p.UpdateUserMembership(userMembershipCommand));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.UpdateUserMembership(userMembershipCommand);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void UpdateUserMembership_ReturnsUnauthorized_WhenUserIdInTokenIsMissing()
    {
        // Arrange
        var userMembershipCommand = new UserMembershipUpdateCommand();
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = _controller.UpdateUserMembership(userMembershipCommand);

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result);
    }

    [Fact]
    public void UpdateUserMembership_ReturnsForbidden_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipCommand = new UserMembershipUpdateCommand { UserId = 2 };
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.UpdateUserMembership(userMembershipCommand);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public void DeleteUserMembership_ReturnsNoContent_WhenMembershipDeletedSuccessfully()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipId = 1;

        var userMembershipOutput = new UserMembershipsGetByIdOutput
        {
            UserId = userIdFromToken,
            UserMembershipId = userMembershipId,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            Status = "active"
        };
    
        _mockUserMembershipsQueryProcessor
            .Setup(q => q.GetById(userMembershipId))
            .Returns(userMembershipOutput);
    
        _mockUserMembershipCommandsProcessor
            .Setup(p => p.DeleteUserMembership(userMembershipId));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.DeleteUserMembership(userMembershipId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }


    [Fact]
    public void DeleteUserMembership_ReturnsNotFound_WhenMembershipNotFound()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipId = 999;

        _mockUserMembershipsQueryProcessor
            .Setup(q => q.GetById(userMembershipId))
            .Returns((UserMembershipsGetByIdOutput)null);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.DeleteUserMembership(userMembershipId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }


    [Fact]
    public void DeleteUserMembership_ReturnsForbidden_WhenUserIdDoesNotMatch()
    {
        // Arrange
        var userIdFromToken = 1;
        var userMembershipId = 2;

        _mockUserMembershipsQueryProcessor
            .Setup(q => q.GetById(userMembershipId))
            .Returns(new UserMembershipsGetByIdOutput { UserId = 2 });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("userId", userIdFromToken.ToString())
                }))
            }
        };

        // Act
        var result = _controller.DeleteUserMembership(userMembershipId);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }



}