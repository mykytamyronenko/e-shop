using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class UserMembershipsDeleteHandlerTest
{
    private readonly Mock<IUserMembershipsRepository> _mockUserMembershipsRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly UserMembershipDeleteHandler _handler;

    public UserMembershipsDeleteHandlerTest()
    {
        // Create mocks for the repository and context
        _mockUserMembershipsRepository = new Mock<IUserMembershipsRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject the mocks into the handler
        _handler = new UserMembershipDeleteHandler(_mockUserMembershipsRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteUserMembership_WhenUserMembershipExists()
    {
        // Arrange
        int userMembershipId = 1;

        // Mock that the user membership exists in the repository
        _mockUserMembershipsRepository.Setup(repo => repo.GetById(userMembershipId)).Returns(new UserMemberships { UserMembershipId = userMembershipId });

        // Act
        _handler.Handle(userMembershipId);

        // Assert
        _mockUserMembershipsRepository.Verify(repo => repo.Delete(userMembershipId), Times.Once); // Verify Delete was called
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify SaveChanges was called
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserMembershipDoesNotExist()
    {
        // Arrange
        int userMembershipId = 1;

        // Mock that the user membership does not exist in the repository
        _mockUserMembershipsRepository.Setup(repo => repo.GetById(userMembershipId)).Returns((UserMemberships)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _handler.Handle(userMembershipId));
        Assert.Equal("User membership not found", exception.Message); // Verify the exception message
    }
}
