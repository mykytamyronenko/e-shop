using Application.exceptions;
using Application.Queries.getById;
using AutoMapper;

namespace Tests.Application.GetById;

public class UsersGetByIdHandlerTest
{
    private readonly Mock<IUsersRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UsersGetByIdHandler _handler;

    public UsersGetByIdHandlerTest()
    {
        // Mock the dependencies
        _userRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();

        // Create the handler with mocked dependencies
        _handler = new UsersGetByIdHandler(_userRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public void Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange: Create a mock user to return from the repository
        var userId = 1;
        var mockUser = new Users
        {
            UserId = userId,
            Username = "testuser",
            Email = "testuser@example.com",
            Password = "hashedpassword",
            Role = "User",
            ProfilePicture = "path/to/profile/pic",
            MembershipLevel = "Basic",
            Rating = 4.5f,
            Status = "Active",
            Balance = 100.00m
        };

        // Arrange: Mock the repository to return the user
        _userRepositoryMock.Setup(repo => repo.GetById(userId)).Returns(mockUser);

        // Arrange: Mock AutoMapper to map the user to UsersGetByIdOutput
        _mapperMock.Setup(mapper => mapper.Map<UsersGetByIdOutput>(mockUser)).Returns(new UsersGetByIdOutput
        {
            UserId = mockUser.UserId,
            Username = mockUser.Username,
            Email = mockUser.Email,
            Password = mockUser.Password,
            Role = mockUser.Role,
            ProfilePicture = mockUser.ProfilePicture,
            MembershipLevel = mockUser.MembershipLevel,
            Rating = mockUser.Rating,
            Status = mockUser.Status,
            Balance = mockUser.Balance
        });

        // Act: Call the handler to get the user
        var result = _handler.Handle(userId);

        // Assert: Check that the result matches the expected output
        Assert.Equal(mockUser.UserId, result.UserId);
        Assert.Equal(mockUser.Username, result.Username);
        Assert.Equal(mockUser.Email, result.Email);
        Assert.Equal(mockUser.Role, result.Role);
        Assert.Equal(mockUser.ProfilePicture, result.ProfilePicture);
        Assert.Equal(mockUser.MembershipLevel, result.MembershipLevel);
        Assert.Equal(mockUser.Rating, result.Rating);
        Assert.Equal(mockUser.Status, result.Status);
        Assert.Equal(mockUser.Balance, result.Balance);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange: Set up the repository to return null for a non-existent user
        var userId = 1;
        _userRepositoryMock.Setup(repo => repo.GetById(userId)).Returns((Users)null);

        // Act & Assert: Verify that an exception is thrown when user is not found
        var exception = Assert.Throws<UserNotFoundException>(() => _handler.Handle(userId));
        Assert.Equal("User not found with id: 1", exception.Message);
    }
}
