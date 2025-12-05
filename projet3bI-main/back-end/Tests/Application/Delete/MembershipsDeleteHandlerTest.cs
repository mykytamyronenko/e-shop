using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class MembershipsDeleteHandlerTest
{
    private readonly Mock<IMembershipsRepository> _mockMembershipsRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly MembershipDeleteHandler _handler;

    public MembershipsDeleteHandlerTest()
    {
        // Create mocks for the repository and context
        _mockMembershipsRepository = new Mock<IMembershipsRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject the mocks into the handler
        _handler = new MembershipDeleteHandler(_mockMembershipsRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteMembership_WhenMembershipExists()
    {
        // Arrange
        int membershipId = 1;

        // Mock that the membership exists in the repository
        _mockMembershipsRepository.Setup(repo => repo.GetById(membershipId)).Returns(new Memberships { MembershipId = membershipId });

        // Act
        _handler.Handle(membershipId);

        // Assert
        _mockMembershipsRepository.Verify(repo => repo.Delete(membershipId), Times.Once); // Verify Delete was called
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify SaveChanges was called
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenMembershipDoesNotExist()
    {
        // Arrange
        int membershipId = 1;

        // Mock that the membership does not exist in the repository
        _mockMembershipsRepository.Setup(repo => repo.GetById(membershipId)).Returns((Memberships)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _handler.Handle(membershipId));
        Assert.Equal("Membership not found", exception.Message); // Verify the exception message
    }
}
