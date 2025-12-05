using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class UsersDeleteHandlerTest
{
    private readonly Mock<IUsersRepository> _mockUsersRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly UserDeleteHandler _handler;

    public UsersDeleteHandlerTest()
    {
        // Create mocks for the repository and context
        _mockUsersRepository = new Mock<IUsersRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject the mocks into the handler
        _handler = new UserDeleteHandler(_mockUsersRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        int userId = 1;

        // Mock that the user exists in the repository
        _mockUsersRepository.Setup(repo => repo.GetById(userId)).Returns(new Users { UserId = userId });

        // Act
        _handler.Handle(userId);

        // Assert
        _mockUsersRepository.Verify(repo => repo.Delete(userId), Times.Once); // Verify Delete was called
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify SaveChanges was called
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        int userId = 1;

        // Mock that the user does not exist in the repository
        _mockUsersRepository.Setup(repo => repo.GetById(userId)).Returns((Users)null);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => _handler.Handle(userId));
        Assert.Equal("User not found", exception.Message); // Verify the exception message
    }
}
