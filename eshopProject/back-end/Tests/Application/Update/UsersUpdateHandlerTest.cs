using Application.Commands.update;
using Application.exceptions;
using AutoMapper;

namespace Tests.Application.Update;

public class UsersUpdateHandlerTest
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly UserUpdateHandler _handler;

    public UsersUpdateHandlerTest()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new UserUpdateHandler(_usersRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateUser_WhenValidDataIsProvided()
    {
        // Arrange: Prepare the input command and mock repository response
        var updateCommand = new UserUpdateCommand
        {
            UserId = 1,
            Username = "newUsername",
            Email = "newemail@example.com",
            Password = "newPassword",
            ProfilePicture = "newProfilePicture.jpg",
            MembershipLevel = "Gold",
            Rating = 4.5f,
            Status = "active",
            Balance = 100.50m
        };

        var user = new Users
        {
            UserId = 1,
            Username = "oldUsername",
            Email = "oldemail@example.com",
            Password = "oldPassword",
            ProfilePicture = "oldProfilePicture.jpg",
            MembershipLevel = "Silver",
            Rating = 3.5f,
            Status = "suspended",
            Balance = 50.00m
        };

        _usersRepositoryMock.Setup(repo => repo.GetById(updateCommand.UserId)).Returns(user);

        // Act: Call the handler to update the User
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated User
        _usersRepositoryMock.Verify(repo => repo.Update(It.Is<Users>(u =>
            u.Username == updateCommand.Username &&
            u.Email == updateCommand.Email &&
            u.Password != user.Password && // Ensure password is hashed
            u.ProfilePicture == updateCommand.ProfilePicture &&
            u.MembershipLevel == updateCommand.MembershipLevel &&
            u.Rating == updateCommand.Rating &&
            u.Status == updateCommand.Status &&
            u.Balance == updateCommand.Balance
        )), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange: Prepare the input command with a non-existing UserId
        var updateCommand = new UserUpdateCommand { UserId = 99 };

        _usersRepositoryMock.Setup(repo => repo.GetById(updateCommand.UserId)).Returns((Users)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<UserNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("User not found with id: 99", exception.Message);
    }
}
