using Application.Commands.update;
using Application.exceptions;
using AutoMapper;

namespace Tests.Application.Update;

public class UserMembershipsUpdateHandlerTest
{
    private readonly Mock<IUserMembershipsRepository> _userMembershipsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly UserMembershipUpdateHandler _handler;

    public UserMembershipsUpdateHandlerTest()
    {
        _userMembershipsRepositoryMock = new Mock<IUserMembershipsRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new UserMembershipUpdateHandler(_userMembershipsRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateUserMembership_WhenValidDataIsProvided()
    {
        // Arrange: Prepare the input command and mock repository response
        var updateCommand = new UserMembershipUpdateCommand
        {
            UserMembershipId = 1,
            UserId = 2,
            MembershipId = 3,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddYears(1),
            Status = "active"
        };

        var userMembership = new UserMemberships
        {
            UserMembershipId = 1,
            UserId = 1,
            MembershipId = 2,
            StartDate = DateTime.Now.AddMonths(-1),
            EndDate = DateTime.Now.AddMonths(11),
            Status = "expired"
        };

        _userMembershipsRepositoryMock.Setup(repo => repo.GetById(updateCommand.UserMembershipId)).Returns(userMembership);

        // Act: Call the handler to update the UserMembership
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated UserMembership
        _userMembershipsRepositoryMock.Verify(repo => repo.Update(It.Is<UserMemberships>(um =>
            um.UserId == updateCommand.UserId &&
            um.MembershipId == updateCommand.MembershipId &&
            um.StartDate == updateCommand.StartDate &&
            um.EndDate == updateCommand.EndDate &&
            um.Status == updateCommand.Status
        )), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserMembershipNotFound()
    {
        // Arrange: Prepare the input command with a non-existing UserMembershipId
        var updateCommand = new UserMembershipUpdateCommand { UserMembershipId = 99 };

        _userMembershipsRepositoryMock.Setup(repo => repo.GetById(updateCommand.UserMembershipId)).Returns((UserMemberships)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<UserMembershipNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("User membership not found with id: 99", exception.Message);
    }
}
