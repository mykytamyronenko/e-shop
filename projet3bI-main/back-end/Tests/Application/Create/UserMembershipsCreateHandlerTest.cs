using Application.Commands.Create;
using Application.exceptions;

namespace Tests.Application.Create;

using Moq;
using Xunit;
using System;
using AutoMapper;
using System.Linq;

public class UserMembershipsCreateHandlerTest
{
    private readonly Mock<IUserMembershipsRepository> _mockUserMembershipsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly UserMembershipCreateHandler _handler;

    public UserMembershipsCreateHandlerTest()
    {
        _mockUserMembershipsRepository = new Mock<IUserMembershipsRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();

        _handler = new UserMembershipCreateHandler(_mockUserMembershipsRepository.Object, _mockMapper.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenStatusIsInvalid()
    {
        // Arrange
        var command = new UserMembershipCreateCommand
        {
            Status = "invalid",  // Invalid status
            UserId = 1,
            MembershipId = 1
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var command = new UserMembershipCreateCommand
        {
            Status = "active",
            UserId = 1,
            MembershipId = 1
        };

        // Mocks
        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns((Users)null);

        // Act & Assert
        Assert.Throws<UserNotFoundException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenMembershipNotFound()
    {
        // Arrange
        var command = new UserMembershipCreateCommand
        {
            Status = "active",
            UserId = 1,
            MembershipId = 1
        };

        // Mocks
        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns(new Users { UserId = 1, Balance = 100 });
        _mockContext.Setup(c => c.Memberships.FirstOrDefault(m => m.MembershipId == 1)).Returns((Memberships)null); 

        // Act & Assert
        Assert.Throws<MembershipNotFoundException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserDoesNotHaveEnoughBalance()
    {
        // Arrange
        var command = new UserMembershipCreateCommand
        {
            Status = "active",
            UserId = 1,
            MembershipId = 1
        };

        // Mocks
        var buyer = new Users { UserId = 1, Balance = 50m }; // Insufficient balance
        var membership = new Memberships { MembershipId = 1, Price = 100m };

        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns(buyer);
        _mockContext.Setup(c => c.Memberships.FirstOrDefault(m => m.MembershipId == 1)).Returns(membership);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserAlreadySubscribed()
    {
        // Arrange
        var command = new UserMembershipCreateCommand
        {
            Status = "active",
            UserId = 1,
            MembershipId = 1
        };

        // Mocks
        var buyer = new Users { UserId = 1, Balance = 100m };
        var membership = new Memberships { MembershipId = 1, Price = 50m };

        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns(buyer);
        _mockContext.Setup(c => c.Memberships.FirstOrDefault(m => m.MembershipId == 1)).Returns(membership);
        _mockContext.Setup(c => c.UserMemberships.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>()))
                    .Returns(new UserMemberships { UserId = 1 });
                    
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _handler.Handle(command));
    }

    [Fact]
    public void Handle_ShouldSuccessfullyCreateUserMembership_WhenAllConditionsAreMet()
    {
        // Arrange
        var command = new UserMembershipCreateCommand
        {
            Status = "active",
            UserId = 1,
            MembershipId = 1
        };

        // Mocks
        var buyer = new Users { UserId = 1, Balance = 100m }; // Sufficient balance
        var membership = new Memberships { MembershipId = 1, Price = 50m };

        _mockContext.Setup(c => c.Users.FirstOrDefault(u => u.UserId == 1)).Returns(buyer);
        _mockContext.Setup(c => c.Memberships.FirstOrDefault(m => m.MembershipId == 1)).Returns(membership);
        _mockContext.Setup(c => c.UserMemberships.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>())).Returns((UserMemberships)null);

        var userMembership = new UserMemberships
        {
            UserId = 1,
            MembershipId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(30),
            Status = "active"
        };

        _mockUserMembershipsRepository.Setup(r => r.Create(It.IsAny<UserMemberships>()));

        _mockMapper.Setup(m => m.Map<UserMembershipCreateOutput>(It.IsAny<UserMemberships>())).Returns(new UserMembershipCreateOutput());

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.NotNull(result); 
        _mockContext.Verify(c => c.SaveChanges(), Times.Once); 
        _mockUserMembershipsRepository.Verify(r => r.Create(It.IsAny<UserMemberships>()), Times.Once);
    }
}
