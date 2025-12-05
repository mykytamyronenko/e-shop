using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class MembershipCommandControllerTest
{
    private readonly Mock<MembershipCommandsProcessor> _mockMembershipCommandsProcessor;
    private readonly MembershipCommandsController _controller;

    public MembershipCommandControllerTest()
    {
        _mockMembershipCommandsProcessor = new Mock<MembershipCommandsProcessor>();
        _controller = new MembershipCommandsController(_mockMembershipCommandsProcessor.Object);
    }

    
    
    [Fact]
    public void CreateMembership_ReturnsOk_WhenValidData()
    {
        // Arrange
        var command = new MembershipCreateCommand
        {
            Name = "Premium",
            Price = 99.99M,
            DiscountPercentage = 10,
            Description = "Premium membership"
        };

        var membershipCreateOutput = new MembershipCreateOutput
        {
            Name = command.Name,
            Price = command.Price,
            DiscountPercentage = command.DiscountPercentage,
            Description = command.Description
        };

        _mockMembershipCommandsProcessor.Setup(p => p.CreateMembership(It.IsAny<MembershipCreateCommand>()))
            .Returns(membershipCreateOutput);

        // Act
        var result = _controller.CreateMembership(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<MembershipCreateOutput>(actionResult.Value); 
        Assert.Equal(membershipCreateOutput.Name, returnValue.Name);
    }

    [Fact]
    public void CreateMembership_ReturnsBadRequest_WhenCommandIsNull()
    {
        // Arrange
        MembershipCreateCommand command = null;

        // Act
        var result = _controller.CreateMembership(command);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid membership data.", actionResult.Value);
    }
    
    [Fact]
    public void UpdateMembership_ReturnsOk_WhenValidData()
    {
        // Arrange
        var command = new MembershipUpdateCommand
        {
              MembershipId = 1,
              Name = "Bronze",
              Price = 99.99M,
              DiscountPercentage = 0.25m,
              Description = "Bronze membership"
        };

        _mockMembershipCommandsProcessor.Setup(p => p.UpdateMembership(It.IsAny<MembershipUpdateCommand>()))
            .Verifiable();

        // Act
        var result = _controller.UpdateMembership(command);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockMembershipCommandsProcessor.Verify(p => p.UpdateMembership(It.IsAny<MembershipUpdateCommand>()), Times.Once);
    }

    [Fact]
    public void UpdateMembership_ReturnsBadRequest_WhenCommandIsInvalid()
    {
        // Arrange
        MembershipUpdateCommand command = null;

        // Act
        var result = _controller.UpdateMembership(command);

        // Assert
        Assert.IsType<BadRequestResult>(result);
    }
    [Fact]
    public void DeleteMembership_ReturnsNoContent_WhenDeletedSuccessfully()
    {
        // Arrange
        var membershipId = 1;

        _mockMembershipCommandsProcessor.Setup(p => p.DeleteMembership(membershipId))
            .Verifiable();

        // Act
        var result = _controller.DeleteMembership(membershipId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockMembershipCommandsProcessor.Verify(p => p.DeleteMembership(membershipId), Times.Once);
    }

    [Fact]
    public void DeleteMembership_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var membershipId = 999;

        _mockMembershipCommandsProcessor.Setup(p => p.DeleteMembership(membershipId))
            .Throws(new InvalidOperationException("User not found"));

        // Act
        var result = _controller.DeleteMembership(membershipId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"User with ID {membershipId} not found.", actionResult.Value);
    }



}