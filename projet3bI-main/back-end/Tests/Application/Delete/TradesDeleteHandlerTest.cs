using Application.Commands.Delete;

namespace Tests.Application.Delete;

using Moq;
using Xunit;
using System;

public class TradesDeleteHandlerTest
{
    private readonly Mock<ITradesRepository> _mockTradesRepository;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly TradeDeleteHandler _handler;

    public TradesDeleteHandlerTest()
    {
        // Create mocks for the repository and context
        _mockTradesRepository = new Mock<ITradesRepository>();
        _mockContext = new Mock<TradeShopContext>();

        // Inject the mocks into the handler
        _handler = new TradeDeleteHandler(_mockTradesRepository.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldDeleteTrade_WhenTradeExists()
    {
        // Arrange
        int tradeId = 1;

        // Mock that the trade exists in the repository
        _mockTradesRepository.Setup(repo => repo.GetById(tradeId)).Returns(new Trades { TradeId = tradeId });

        // Act
        _handler.Handle(tradeId);

        // Assert
        _mockTradesRepository.Verify(repo => repo.Delete(tradeId), Times.Once); // Verify Delete was called
        _mockContext.Verify(context => context.SaveChanges(), Times.Once); // Verify SaveChanges was called
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenTradeDoesNotExist()
    {
        // Arrange
        int tradeId = 1;

        // Mock that the trade does not exist in the repository
        _mockTradesRepository.Setup(repo => repo.GetById(tradeId)).Returns((Trades)null);

        // Act & Assert
        var exception = Assert.Throws<Exception>(() => _handler.Handle(tradeId));
        Assert.Equal("Trade not found", exception.Message); // Verify the exception message
    }
}
