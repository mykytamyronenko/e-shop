using Application.Commands.Create;
using Application.Utils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Tests.Application.Create;

public class MembershipCreateHandlerTest
{
 private readonly Mock<IMembershipsRepository> _mockMembershipsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly MembershipCreateHandler _handler;

    public MembershipCreateHandlerTest()
    {
        _mockMembershipsRepository = new Mock<IMembershipsRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();

        _handler = new MembershipCreateHandler(
            _mockMembershipsRepository.Object,
            _mockMapper.Object,
            _mockContext.Object
        );
    }

    [Fact]
    public void Handle_ShouldCreateMembershipAndReturnMappedOutput()
    {
        // Arrange
        var command = new MembershipCreateCommand
        {
            Name = "Premium",
            Price = 100.00M,
            DiscountPercentage = 10,
            Description = "Premium Membership"
        };

        var membership = new Memberships
        {
            MembershipId = 1,
            Name = command.Name,
            Price = command.Price,
            DiscountPercentage = command.DiscountPercentage,
            Description = command.Description
        };

        var mappedOutput = new MembershipCreateOutput
        {
            Name = membership.Name,
            Price = membership.Price,
            DiscountPercentage = membership.DiscountPercentage,
            Description = membership.Description
        };

        // Mock the repository to not find any existing membership
        _mockContext.Setup(c => c.Memberships)
            .Returns(DbSetMockHelper.GetMockDbSet(new[] { membership }).Object); // Helper to mock DbSet

        _mockMembershipsRepository.Setup(r => r.Create(It.IsAny<Memberships>()))
            .Returns(membership);

        _mockMapper.Setup(m => m.Map<MembershipCreateOutput>(It.IsAny<Memberships>()))
            .Returns(mappedOutput);

        // Act
        var result = _handler.Handle(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(mappedOutput.Name, result.Name);
        Assert.Equal(mappedOutput.Price, result.Price);
        Assert.Equal(mappedOutput.DiscountPercentage, result.DiscountPercentage);
        Assert.Equal(mappedOutput.Description, result.Description);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenMembershipAlreadyExists()
    {
        // Arrange
        var command = new MembershipCreateCommand
        {
            Name = "Premium",
            Price = 100.00M,
            DiscountPercentage = 10,
            Description = "Premium Membership"
        };

        // Mock existing membership with the same name
        var existingMembership = new Memberships
        {
            MembershipId = 1,
            Name = command.Name,
            Price = command.Price,
            DiscountPercentage = command.DiscountPercentage,
            Description = command.Description
        };

        _mockContext.Setup(c => c.Memberships)
            .Returns(DbSetMockHelper.GetMockDbSet(new[] { existingMembership }).Object); // Mock existing membership

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(command));
        Assert.Equal("Membership already exists.", exception.Message);
    }
}

    public static class DbSetMockHelper
    {
        public static Mock<DbSet<T>> GetMockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryableData = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryableData.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryableData.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryableData.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryableData.GetEnumerator());

            return mockSet;
        }
    }

