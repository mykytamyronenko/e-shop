using Application.Commands.update;
using Application.exceptions;
using AutoMapper;

namespace Tests.Application.Update;

public class MembershipsUpdateHandlerTest
{
    private readonly Mock<IMembershipsRepository> _membershipsRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly MembershipUpdateHandler _handler;

    public MembershipsUpdateHandlerTest()
    {
        _membershipsRepositoryMock = new Mock<IMembershipsRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new MembershipUpdateHandler(_membershipsRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateMembership_WhenValidDataIsProvided()
    {
        // Arrange: Prepare the input command and mocked repository response
        var updateCommand = new MembershipUpdateCommand
        {
            MembershipId = 1,
            Name = "Gold",
            Price = 199.99m,
            DiscountPercentage = 10m,
            Description = "Exclusive access to premium features"
        };

        var membership = new Memberships
        {
            MembershipId = 1,
            Name = "Silver",
            Price = 99.99m,
            DiscountPercentage = 5m,
            Description = "Basic membership"
        };

        _membershipsRepositoryMock.Setup(repo => repo.GetById(updateCommand.MembershipId)).Returns(membership);

        // Act: Call the handler to update the membership
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated membership
        _membershipsRepositoryMock.Verify(repo => repo.Update(It.Is<Memberships>(m => 
            m.Name == updateCommand.Name &&
            m.Price == updateCommand.Price &&
            m.DiscountPercentage == updateCommand.DiscountPercentage &&
            m.Description == updateCommand.Description
        )), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenMembershipNotFound()
    {
        // Arrange: Prepare the input command with a non-existing MembershipId
        var updateCommand = new MembershipUpdateCommand { MembershipId = 99 };

        _membershipsRepositoryMock.Setup(repo => repo.GetById(updateCommand.MembershipId)).Returns((Memberships)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<MembershipNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("Membership not found with id: 99", exception.Message);
    }
}
