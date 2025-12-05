using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

public class MembershipsGetAllHandlerTest
{
    private readonly Mock<IMembershipsRepository> _mockMembershipsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MembershipGetAllHandler _handler;

    public MembershipsGetAllHandlerTest()
    {
        // Create mocks for the repository and mapper
        _mockMembershipsRepository = new Mock<IMembershipsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Inject the mocks into the handler
        _handler = new MembershipGetAllHandler(_mockMembershipsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnMemberships_WhenMembershipsExist()
    {
        // Arrange
        var dbMemberships = new List<Memberships>
        {
            new Memberships { MembershipId = 1, Name = "Bronze", Price = 10m },
            new Memberships { MembershipId = 2, Name = "Silver", Price = 20m }
        };

        // Mock repository method to return a list of memberships
        _mockMembershipsRepository.Setup(repo => repo.GetAll()).Returns(dbMemberships);

        // Mock mapper method to map the memberships
        _mockMapper.Setup(m => m.Map<List<MembershipGetAllOutput.Memberships>>(It.IsAny<List<Memberships>>()))
                   .Returns(dbMemberships.Select(m => new MembershipGetAllOutput.Memberships { MembershipId = m.MembershipId, Name = m.Name, Price = m.Price }).ToList());

        // Act
        var result = _handler.Handle(new MembershipGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.MembershipList.Count);
        Assert.Equal(1, result.MembershipList[0].MembershipId);
        Assert.Equal("Basic", result.MembershipList[0].Name);
        Assert.Equal(10m, result.MembershipList[0].Price);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoMembershipsExist()
    {
        // Arrange
        var dbMemberships = new List<Memberships>(); // No memberships

        // Mock repository method to return an empty list
        _mockMembershipsRepository.Setup(repo => repo.GetAll()).Returns(dbMemberships);

        // Act
        var result = _handler.Handle(new MembershipGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.MembershipList); // Should return an empty list
    }
}
