using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class UserMembershipQueryControllerTest
{
    private readonly Mock<UserMembershipsQueryProcessor> _mockUserMembershipsQueryProcessor;
    private readonly UserMembershipQueryController _controller;

    public UserMembershipQueryControllerTest()
    {
        _mockUserMembershipsQueryProcessor = new Mock<UserMembershipsQueryProcessor>();
        _controller = new UserMembershipQueryController(_mockUserMembershipsQueryProcessor.Object);
    }
    
    [Fact]
    public void GetAllUserMemberships_ReturnsOk_WhenMembershipsExist()
    {
        // Arrange
        var expectedUserMemberships = new List<UsersMembershipGetAllOutput.UserMemberships>
        {
            new UsersMembershipGetAllOutput.UserMemberships { UserMembershipId = 1, UserId = 1, MembershipId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Status = "Active" },
            new UsersMembershipGetAllOutput.UserMemberships { UserMembershipId = 2, UserId = 2, MembershipId = 2, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Status = "Active" }
        };

        _mockUserMembershipsQueryProcessor
            .Setup(p => p.GetAll(null))
            .Returns(new UsersMembershipGetAllOutput { UserMembershipsList = expectedUserMemberships });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetAllUserMemberships();

        // Assert
        var okResult = Assert.IsType<List<UsersMembershipGetAllOutput.UserMemberships>>(result);
        Assert.Equal(expectedUserMemberships.Count, okResult.Count);
        Assert.Equal(expectedUserMemberships[0].UserMembershipId, okResult[0].UserMembershipId);
    }

    [Fact]
    public void GetAllUserMemberships_ReturnsEmptyList_WhenNoMembershipsExist()
    {
        // Arrange
        var expectedUserMemberships = new List<UsersMembershipGetAllOutput.UserMemberships>();

        _mockUserMembershipsQueryProcessor
            .Setup(p => p.GetAll(null))
            .Returns(new UsersMembershipGetAllOutput { UserMembershipsList = expectedUserMemberships });

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetAllUserMemberships();

        // Assert
        var okResult = Assert.IsType<List<UsersMembershipGetAllOutput.UserMemberships>>(result);
        Assert.Empty(okResult);
    }

    [Fact]
    public void GetByIdUserMembership_ReturnsOk_WhenUserMembershipFound()
    {
        // Arrange
        var userMembershipId = 1;
        var expectedUserMembership = new UserMembershipsGetByIdOutput
        {
            UserMembershipId = 1,
            UserId = 1,
            MembershipId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            Status = "active"
        };

        _mockUserMembershipsQueryProcessor
            .Setup(p => p.GetById(userMembershipId))
            .Returns(expectedUserMembership);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetByIdUserMembership(userMembershipId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var userMembership = Assert.IsType<UserMembershipsGetByIdOutput>(okResult.Value);
        Assert.Equal(expectedUserMembership.UserMembershipId, userMembership.UserMembershipId);
    }

    [Fact]
    public void GetByIdUserMembership_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var userMembershipId = -1; // Invalid ID

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetByIdUserMembership(userMembershipId);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("The id of the user membership must be greater than 0.", badRequestResult.Value);
    }

    [Fact]
    public void GetByIdUserMembership_ReturnsNotFound_WhenUserMembershipNotFound()
    {
        // Arrange
        var userMembershipId = 999; // Non-existing ID

        _mockUserMembershipsQueryProcessor
            .Setup(p => p.GetById(userMembershipId))
            .Throws(new UserMembershipNotFoundException(userMembershipId));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("userId", "1") }))
            }
        };

        // Act
        var result = _controller.GetByIdUserMembership(userMembershipId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal("User membership not found", notFoundResult.Value);
    }


}