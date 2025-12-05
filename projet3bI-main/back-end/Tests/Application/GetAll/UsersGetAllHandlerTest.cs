using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

public class UsersGetAllHandlerTest
{
    private readonly Mock<IUsersRepository> _mockUserRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UsersGetAllHandler _handler;

    public UsersGetAllHandlerTest()
    {
        // Initialize mocks for repository and mapper
        _mockUserRepository = new Mock<IUsersRepository>();
        _mockMapper = new Mock<IMapper>();

        // Create handler with mocked dependencies
        _handler = new UsersGetAllHandler(_mockUserRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnUsers_WhenUsersExist()
    {
        // Arrange: Define mock user data
        var dbUsers = new List<Users>
        {
            new Users { UserId = 1, Username = "john_doe", Email = "john@example.com", Role = "user", Status = "active", ProfilePicture = "profile1.jpg" },
            new Users { UserId = 2, Username = "jane_doe", Email = "jane@example.com", Role = "admin", Status = "active", ProfilePicture = "profile2.jpg" }
        };

        // Mock repository to return the list of users
        _mockUserRepository.Setup(repo => repo.GetAll()).Returns(dbUsers);

        // Mock mapper to map the db entities to output DTO
        _mockMapper.Setup(m => m.Map<List<UsersGetAllOutput.Users>>(It.IsAny<List<Users>>()))
                   .Returns(dbUsers.Select(u => new UsersGetAllOutput.Users
                   {
                       UserId = u.UserId,
                       Username = u.Username,
                       Email = u.Email,
                       Role = u.Role,
                       Status = u.Status,
                       ProfilePicture = u.ProfilePicture
                   }).ToList());

        // Act: Call the handler method
        var result = _handler.Handle(new UsersGetAllQuery());

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(2, result.UsersList.Count);
        Assert.Equal(1, result.UsersList[0].UserId);
        Assert.Equal("john_doe", result.UsersList[0].Username);
        Assert.Equal("john@example.com", result.UsersList[0].Email);
        Assert.Equal("user", result.UsersList[0].Role);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoUsersExist()
    {
        // Arrange: Empty list of users
        var dbUsers = new List<Users>();

        // Mock repository to return an empty list
        _mockUserRepository.Setup(repo => repo.GetAll()).Returns(dbUsers);

        // Act: Call the handler method
        var result = _handler.Handle(new UsersGetAllQuery());

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Empty(result.UsersList); // Should return an empty list
    }
}
