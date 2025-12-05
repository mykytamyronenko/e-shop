using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

public class UsersMembershipsGetAllHandlerTest
{
    private readonly Mock<IUserMembershipsRepository> _mockUserMembershipsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly UsersMembershipGetAllHandler _handler;

    public UsersMembershipsGetAllHandlerTest()
    {
        // Initialize mocks for repository and mapper
        _mockUserMembershipsRepository = new Mock<IUserMembershipsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Create handler with mocked dependencies
        _handler = new UsersMembershipGetAllHandler(_mockUserMembershipsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnUserMemberships_WhenUserMembershipsExist()
    {
        // Arrange: Define mock user membership data
        var dbUserMemberships = new List<UserMemberships>
        {
            new UserMemberships { UserMembershipId = 1, UserId = 1, MembershipId = 2, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Status = "active" },
            new UserMemberships { UserMembershipId = 2, UserId = 3, MembershipId = 4, StartDate = DateTime.Now, EndDate = DateTime.Now.AddMonths(1), Status = "suspended" }
        };

        // Mock repository to return the list of user memberships
        _mockUserMembershipsRepository.Setup(repo => repo.GetAll()).Returns(dbUserMemberships);

        // Mock mapper to map the db entities to output DTO
        _mockMapper.Setup(m => m.Map<List<UsersMembershipGetAllOutput.UserMemberships>>(It.IsAny<List<UserMemberships>>()))
                   .Returns(dbUserMemberships.Select(m => new UsersMembershipGetAllOutput.UserMemberships
                   {
                       UserMembershipId = m.UserMembershipId,
                       UserId = m.UserId,
                       MembershipId = m.MembershipId,
                       StartDate = m.StartDate,
                       EndDate = m.EndDate,
                       Status = m.Status
                   }).ToList());

        // Act: Call the handler method
        var result = _handler.Handle(new UsersMembershipGetAllQuery());

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(2, result.UserMembershipsList.Count);
        Assert.Equal(1, result.UserMembershipsList[0].UserMembershipId);
        Assert.Equal(2, result.UserMembershipsList[0].MembershipId);
        Assert.Equal("active", result.UserMembershipsList[0].Status);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoUserMembershipsExist()
    {
        // Arrange: Empty list of user memberships
        var dbUserMemberships = new List<UserMemberships>();

        // Mock repository to return an empty list
        _mockUserMembershipsRepository.Setup(repo => repo.GetAll()).Returns(dbUserMemberships);

        // Act: Call the handler method
        var result = _handler.Handle(new UsersMembershipGetAllQuery());

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Empty(result.UserMembershipsList); // Should return an empty list
    }
}
