using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Tests.Infrastructure;

using Moq;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;

public class MembershipRepositoryTest
{
    private readonly Mock<DbSet<Memberships>> _mockSet;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly MembershipsRepository _repository;

    public MembershipRepositoryTest()
    {
        // Initialize mocks
        _mockSet = new Mock<DbSet<Memberships>>();
        _mockContext = new Mock<TradeShopContext>();

        // Setup the context to return the mocked DbSet
        _mockContext.Setup(m => m.Memberships).Returns(_mockSet.Object);

        // Initialize the repository with the mocked context
        _repository = new MembershipsRepository(_mockContext.Object);
    }

    [Fact]
    public void GetAll_ShouldReturnListOfMemberships()
    {
        // Arrange
        var memberships = new List<Memberships>
        {
            new Memberships { MembershipId = 1, Name = "Bronze", Price = 10.00M },
            new Memberships { MembershipId = 2, Name = "Silver", Price = 20.00M }
        }.AsQueryable();

        _mockSet.As<IQueryable<Memberships>>().Setup(m => m.Provider).Returns(memberships.Provider);
        _mockSet.As<IQueryable<Memberships>>().Setup(m => m.Expression).Returns(memberships.Expression);
        _mockSet.As<IQueryable<Memberships>>().Setup(m => m.ElementType).Returns(memberships.ElementType);
        _mockSet.As<IQueryable<Memberships>>().Setup(m => m.GetEnumerator()).Returns(memberships.GetEnumerator());

        // Act
        var result = _repository.GetAll();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void GetById_ShouldReturnMembership_WhenMembershipExists()
    {
        // Arrange
        var membership = new Memberships { MembershipId = 1, Name = "Bronze", Price = 10.00M };
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Memberships, bool>>())).Returns(membership);

        // Act
        var result = _repository.GetById(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.MembershipId);
    }

    [Fact]
    public void GetById_ShouldReturnNull_WhenMembershipDoesNotExist()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Memberships, bool>>())).Returns((Memberships)null);

        // Act
        var result = _repository.GetById(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Create_ShouldAddMembershipAndReturnCreatedEntity()
    {
        // Arrange
        var newMembership = new Memberships { MembershipId = 3, Name = "VIP", Price = 50.00M };
        
        // Mock the behavior of Add to return an EntityEntry that represents the entity being added
        var entityEntryMock = new Mock<EntityEntry<Memberships>>();
        _mockSet.Setup(m => m.Add(It.IsAny<Memberships>())).Returns(entityEntryMock.Object);
        
        // Mock SaveChanges to return 1, simulating a successful save
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var result = _repository.Create(newMembership);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("VIP", result.Name);
    }

    [Fact]
    public void Update_ShouldReturnTrue_WhenMembershipIsUpdated()
    {
        // Arrange
        var existingMembership = new Memberships { MembershipId = 1, Name = "Bronze", Price = 10.00M };
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Memberships, bool>>())).Returns(existingMembership);
        _mockContext.Setup(m => m.SaveChanges()).Returns(1);

        // Act
        var updated = _repository.Update(new Memberships { MembershipId = 1, Name = "Bronze Updated", Price = 12.00M });

        // Assert
        Assert.True(updated);
    }

    [Fact]
    public void Update_ShouldReturnFalse_WhenMembershipNotFound()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Memberships, bool>>())).Returns((Memberships)null);

        // Act
        var updated = _repository.Update(new Memberships { MembershipId = 999, Name = "Nonexistent", Price = 0.00M });

        // Assert
        Assert.False(updated);
    }

    [Fact]
    public void Delete_ShouldReturnTrue_WhenMembershipIsDeleted()
    {
        // Arrange
        var membership = new Memberships { MembershipId = 1, Name = "Bronze" };
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Memberships, bool>>())).Returns(membership);
        _mockSet.Setup(m => m.Remove(It.IsAny<Memberships>())).Returns((EntityEntry<Memberships>)null); // Mock Remove

        _mockContext.Setup(m => m.SaveChanges()).Returns(1); // Simulate successful deletion

        // Act
        var result = _repository.Delete(1);

        // Assert
        Assert.True(result); // Ensure the delete was successful
    }

    [Fact]
    public void Delete_ShouldReturnFalse_WhenMembershipDoesNotExist()
    {
        // Arrange
        _mockSet.Setup(m => m.FirstOrDefault(It.IsAny<Func<Memberships, bool>>())).Returns((Memberships)null);

        // Act
        var result = _repository.Delete(999);

        // Assert
        Assert.False(result); // Ensure no membership was deleted
    }
}
