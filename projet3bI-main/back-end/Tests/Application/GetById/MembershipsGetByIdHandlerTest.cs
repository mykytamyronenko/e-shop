using Application.exceptions;
using Application.Queries.getById;

namespace Tests.Application.GetById;

using Moq;
using Xunit;
using AutoMapper;
using System;

public class MembershipsGetByIdHandlerTest
{
    private readonly Mock<IMembershipsRepository> _mockMembershipsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly MembershipsGetByIdHandler _handler;

    public MembershipsGetByIdHandlerTest()
    {
        // Mock dependencies
        _mockMembershipsRepository = new Mock<IMembershipsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Instantiate the handler
        _handler = new MembershipsGetByIdHandler(_mockMembershipsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnMembership_WhenMembershipExists()
    {
        // Arrange: Define a mock membership
        var membershipId = 1;
        var dbMembership = new Memberships { MembershipId = membershipId, Name= "Bronze", Description = "Premium membership" };
        var outputMembership = new MembershipsGetByIdOutput { MembershipId = membershipId, Name = "Bronze", Description = "Premium membership" };

        // Mock the repository to return the membership
        _mockMembershipsRepository.Setup(repo => repo.GetById(membershipId)).Returns(dbMembership);

        // Mock the mapper to map the entity to output DTO
        _mockMapper.Setup(m => m.Map<MembershipsGetByIdOutput>(dbMembership)).Returns(outputMembership);

        // Act: Call the handler's handle method
        var result = _handler.Handle(membershipId);

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(membershipId, result.MembershipId);
        Assert.Equal("Bronze", result.Name);
        Assert.Equal("Premium membership", result.Description);
    }

    [Fact]
    public void Handle_ShouldThrowMembershipNotFoundException_WhenMembershipDoesNotExist()
    {
        // Arrange: Define a non-existent membership ID
        var membershipId = 99;

        // Mock the repository to return null (membership not found)
        _mockMembershipsRepository.Setup(repo => repo.GetById(membershipId)).Returns((Memberships)null);

        // Act & Assert: Ensure the handler throws an exception
        Assert.Throws<MembershipNotFoundException>(() => _handler.Handle(membershipId));
    }
}
