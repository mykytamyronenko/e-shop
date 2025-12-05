namespace Tests.Infrastructure;

using Moq;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

public class UserMembershipRepositoryTest
{
    private readonly Mock<DbSet<UserMemberships>> _mockSet;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly UserMembershipsRepository _repository;

    public UserMembershipRepositoryTest()
    {
        // Initialize mocks
        _mockSet = new Mock<DbSet<UserMemberships>>();
        _mockContext = new Mock<TradeShopContext>();

        // Setup the context to return the mocked DbSet
        _mockContext.Setup(m => m.UserMemberships).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new UserMembershipsRepository(_mockContext.Object);
    }

    [Fact]
    public void Create_ShouldAddUserMembershipAndReturnCreatedEntity()
    {
        // Arrange
        var newUserMembership = new UserMemberships
        {
            UserMembershipId = 1,
            UserId = 1,
            MembershipId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            Status = "active"
        };

        var entityEntryMock = new Mock<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UserMemberships>>();
        _mockSet.Setup(m => m.Add(It.IsAny<UserMemberships>())).Returns(entityEntryMock.Object);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Create(newUserMembership);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.UserMembershipId);
        Assert.Equal("active", result.Status);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenUserMembershipExists()
    {
        // Arrange
        var existingUserMembership = new UserMemberships
        {
            UserMembershipId = 1,
            UserId = 1,
            MembershipId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            Status = "active"
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>())).Returns(existingUserMembership);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Update(existingUserMembership);

        // Assert
        Assert.True(result); // Ensure the user membership was updated successfully
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenUserMembershipDoesNotExist()
    {
        // Arrange
        var nonExistingUserMembership = new UserMemberships
        {
            UserMembershipId = 999,
            UserId = 1,
            MembershipId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            Status = "active"
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>())).Returns((UserMemberships)null);

        // Act
        var result = _repository.Update(nonExistingUserMembership);

        // Assert
        Assert.False(result); // Ensure the update did not occur for a non-existing membership
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenUserMembershipExists()
    {
        // Arrange
        var existingUserMembership = new UserMemberships
        {
            UserMembershipId = 1,
            UserId = 1,
            MembershipId = 1,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(1),
            Status = "active"
        };

        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>())).Returns(existingUserMembership);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the user membership was deleted successfully
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenUserMembershipDoesNotExist()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<UserMemberships, bool>>())).Returns((UserMemberships)null);

        // Act
        var result = _repository.Delete(999); // Non-existing ID

        // Assert
        Assert.False(result); // Ensure the delete did not occur for a non-existing membership
    }

    [Fact]
    public void GetExpiredMemberships_ShouldReturnExpiredMemberships()
    {
        // Arrange
        var now = DateTime.Now;
        var expiredMemberships = new List<UserMemberships>
        {
            new UserMemberships
            {
                UserMembershipId = 1,
                UserId = 1,
                MembershipId = 1,
                StartDate = now.AddMonths(-2),
                EndDate = now.AddMonths(-1),
                Status = "expired"
            },
            new UserMemberships
            {
                UserMembershipId = 2,
                UserId = 2,
                MembershipId = 2,
                StartDate = now.AddMonths(-3),
                EndDate = now.AddMonths(-1),
                Status = "expired"
            }
        }.AsQueryable();

        _mockSet.As<IQueryable<UserMemberships>>().Setup(m => m.Where(It.IsAny<Func<UserMemberships, bool>>()))
                .Returns(expiredMemberships.Where(m => m.EndDate <= now));

        // Act
        var result = _repository.GetExpiredMemberships(now);

        // Assert
        Assert.NotEmpty(result); // Ensure there are expired memberships returned
        Assert.All(result, membership => Assert.True(membership.EndDate <= now)); // All memberships should have expired
    }
}
