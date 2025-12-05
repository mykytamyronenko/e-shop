using API.Controllers;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class MembershipQueryControllerTest
{
    private readonly Mock<MembershipsQueryProcessor> _mockMembershipsQueryProcessor;
    private readonly MembershipQueryController _controller;

    public MembershipQueryControllerTest()
    {
        _mockMembershipsQueryProcessor = new Mock<MembershipsQueryProcessor>();
        _controller = new MembershipQueryController(_mockMembershipsQueryProcessor.Object);
    }

    [Fact]
    public void GetAllMemberships_ReturnsOk_WhenMembershipsExist()
    {
        // Arrange
        var membershipsList = new List<MembershipGetAllOutput.Memberships>
        {
            new MembershipGetAllOutput.Memberships { MembershipId = 1, Name = "Basic", Price = 29.99M },
            new MembershipGetAllOutput.Memberships { MembershipId = 2, Name = "Premium", Price = 49.99M }
        };

        var membershipsOutput = new MembershipGetAllOutput { MembershipList = membershipsList };

        var query = new MembershipGetAllQuery
        {
        };

        _mockMembershipsQueryProcessor.Setup(p => p.GetAll(It.IsAny<MembershipGetAllQuery>())).Returns(membershipsOutput);

        // Act
        var result = _controller.GetAllMemberships();

        // Assert
        var actionResult = Assert.IsType<List<MembershipGetAllOutput.Memberships>>(result);
        Assert.Equal(2, actionResult.Count);
    }


    [Fact]
    public void GetAllMemberships_ReturnsOk_WhenNoMembershipsExist()
    {
        // Arrange
        var membershipsOutput = new MembershipGetAllOutput { MembershipList = new List<MembershipGetAllOutput.Memberships>() };

        var query = new MembershipGetAllQuery
        {
        };

        _mockMembershipsQueryProcessor.Setup(p => p.GetAll(It.IsAny<MembershipGetAllQuery>())).Returns(membershipsOutput);

        // Act
        var result = _controller.GetAllMemberships();

        // Assert
        var actionResult = Assert.IsType<List<MembershipGetAllOutput.Memberships>>(result);
        Assert.Empty(actionResult);
    }

    
    [Fact]
    public void GetByIdMembership_ReturnsOk_WhenMembershipExists()
    {
        // Arrange
        var membershipId = 1;
        var membershipOutput = new MembershipsGetByIdOutput
        {
            MembershipId = membershipId,
            Name = "Premium",
            Price = 49.99M
        };

        _mockMembershipsQueryProcessor.Setup(p => p.GetById(membershipId)).Returns(membershipOutput);

        // Act
        var result = _controller.GetByIdMembership(membershipId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<MembershipsGetByIdOutput>(actionResult.Value);
        Assert.Equal(membershipId, returnValue.MembershipId);
        Assert.Equal("Premium", returnValue.Name);
    }

    [Fact]
    public void GetByIdMembership_ReturnsNotFound_WhenMembershipNotFound()
    {
        // Arrange
        var membershipId = 999;

        _mockMembershipsQueryProcessor.Setup(p => p.GetById(membershipId))
            .Throws(new MembershipNotFoundException(membershipId));

        // Act
        var result = _controller.GetByIdMembership(membershipId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Membership not found", actionResult.Value);
    }

    [Fact]
    public void GetByIdMembership_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Arrange
        var invalidId = -1;

        // Act
        var result = _controller.GetByIdMembership(invalidId);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The id of the membership must be greater than 0.", actionResult.Value);
    }


}