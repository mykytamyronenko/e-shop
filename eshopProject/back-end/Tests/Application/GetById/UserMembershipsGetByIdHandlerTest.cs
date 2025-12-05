using Application.exceptions;
using Application.Queries.getById;

namespace Tests.Application.GetById;

using Moq;
using Xunit;
using AutoMapper;
using System;

public class UserMembershipsGetByIdHandlerTest
{
    private readonly Mock<IUserMembershipsRepository> _mockUserMembershipsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UserMembershipsGetByIdHandler _handler;

    public UserMembershipsGetByIdHandlerTest()
    {
        // Mock the dependencies
        _mockUserMembershipsRepository = new Mock<IUserMembershipsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Instantiate the handler
        _handler = new UserMembershipsGetByIdHandler(_mockUserMembershipsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnUserMembership_WhenUserMembershipExists()
    {
        // Arrange: Define a mock user membership entity
        var userMembershipId = 1;
        var dbUserMembership = new UserMemberships { UserMembershipId = userMembershipId, UserId = 123, MembershipId = 456, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1), Status = "active" };
        var outputUserMembership = new UserMembershipsGetByIdOutput { UserMembershipId = userMembershipId, UserId = 123, MembershipId = 456, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1), Status = "active" };

        // Mock the repository to return the user membership
        _mockUserMembershipsRepository.Setup(repo => repo.GetById(userMembershipId)).Returns(dbUserMembership);

        // Mock the mapper to map the entity to the output DTO
        _mockMapper.Setup(m => m.Map<UserMembershipsGetByIdOutput>(dbUserMembership)).Returns(outputUserMembership);

        // Act: Call the handler's handle method
        var result = _handler.Handle(userMembershipId);

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(userMembershipId, result.UserMembershipId);
        Assert.Equal(123, result.UserId);
        Assert.Equal(456, result.MembershipId);
        Assert.Equal("active", result.Status);
    }

    [Fact]
    public void Handle_ShouldThrowUserMembershipNotFoundException_WhenUserMembershipDoesNotExist()
    {
        // Arrange: Define a non-existent user membership ID
        var userMembershipId = 99;

        // Mock the repository to return null (user membership not found)
        _mockUserMembershipsRepository.Setup(repo => repo.GetById(userMembershipId)).Returns((UserMemberships)null);

        // Act & Assert: Ensure the handler throws an exception
        Assert.Throws<UserMembershipNotFoundException>(() => _handler.Handle(userMembershipId));
    }
}
