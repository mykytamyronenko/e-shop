using Application.Commands.update;
using Application.exceptions;
using AutoMapper;

namespace Tests.Application.Update;

public class TradesUpdateHandlerTest
{
    private readonly Mock<ITradesRepository> _tradesRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<TradeShopContext> _contextMock;
    private readonly TradeUpdateHandler _handler;

    public TradesUpdateHandlerTest()
    {
        _tradesRepositoryMock = new Mock<ITradesRepository>();
        _mapperMock = new Mock<IMapper>();
        _contextMock = new Mock<TradeShopContext>();
        _handler = new TradeUpdateHandler(_tradesRepositoryMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public void Handle_ShouldUpdateTrade_WhenValidDataIsProvided()
    {
        // Arrange: Prepare the input command and mocked repository response
        var updateCommand = new TradeUpdateCommand
        {
            TradeId = 1,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "1,2,3",
            ReceiverArticleId = 5,
            TradeDate = DateTime.Now,
            Status = "accepted"
        };

        var trade = new Trades
        {
            TradeId = 1,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "1,2",
            ReceiverArticleId = 5,
            TradeDate = DateTime.Now.AddDays(-1),
            Status = "in progress"
        };

        _tradesRepositoryMock.Setup(repo => repo.GetById(updateCommand.TradeId)).Returns(trade);

        // Act: Call the handler to update the trade
        _handler.Handle(updateCommand);

        // Assert: Verify that the repository's update method is called with the updated trade
        _tradesRepositoryMock.Verify(repo => repo.Update(It.Is<Trades>(t =>
            t.TraderId == updateCommand.TraderId &&
            t.ReceiverId == updateCommand.ReceiverId &&
            t.TraderArticlesIds == updateCommand.TraderArticlesIds &&
            t.ReceiverArticleId == updateCommand.ReceiverArticleId &&
            t.TradeDate == updateCommand.TradeDate &&
            t.Status == updateCommand.Status
        )), Times.Once);

        // Assert: Verify SaveChanges is called
        _contextMock.Verify(context => context.SaveChanges(), Times.Once);
    }

    [Fact]
    public void Handle_ShouldThrowException_WhenTradeNotFound()
    {
        // Arrange: Prepare the input command with a non-existing TradeId
        var updateCommand = new TradeUpdateCommand { TradeId = 99 };

        _tradesRepositoryMock.Setup(repo => repo.GetById(updateCommand.TradeId)).Returns((Trades)null);

        // Act & Assert: Verify that the exception is thrown
        var exception = Assert.Throws<TradeNotFoundException>(() => _handler.Handle(updateCommand));
        Assert.Equal("Trade not found with id: 99", exception.Message);
    }
}
