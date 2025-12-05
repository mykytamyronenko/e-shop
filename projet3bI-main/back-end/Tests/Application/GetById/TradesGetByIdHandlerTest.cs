using Application.exceptions;
using Application.Queries.getById;

namespace Tests.Application.GetById;

using Moq;
using Xunit;
using AutoMapper;
using System;

public class TradesGetByIdHandlerTest
{
    private readonly Mock<ITradesRepository> _mockTradesRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TradesGetByIdHandler _handler;

    public TradesGetByIdHandlerTest()
    {
        // Mock the dependencies
        _mockTradesRepository = new Mock<ITradesRepository>();
        _mockMapper = new Mock<IMapper>();

        // Instantiate the handler
        _handler = new TradesGetByIdHandler(_mockTradesRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnTrade_WhenTradeExists()
    {
        // Arrange: Define a mock trade entity
        var tradeId = 1;
        var dbTrade = new Trades { TradeId = tradeId, TraderId = 123, ReceiverId = 456, TraderArticlesIds = "1,2", ReceiverArticleId = 789, TradeDate = DateTime.Now, Status = "in progress" };
        var outputTrade = new TradesGetByIdOutput { TradeId = tradeId, TraderId = 123, ReceiverId = 456, TraderArticlesIds = "1,2", ReceiverArticleId = 789, TradeDate = DateTime.Now, Status = "in progress" };

        // Mock the repository to return the trade
        _mockTradesRepository.Setup(repo => repo.GetById(tradeId)).Returns(dbTrade);

        // Mock the mapper to map the entity to the output DTO
        _mockMapper.Setup(m => m.Map<TradesGetByIdOutput>(dbTrade)).Returns(outputTrade);

        // Act: Call the handler's handle method
        var result = _handler.Handle(tradeId);

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(tradeId, result.TradeId);
        Assert.Equal(123, result.TraderId);
        Assert.Equal(456, result.ReceiverId);
        Assert.Equal("1,2", result.TraderArticlesIds);
        Assert.Equal(789, result.ReceiverArticleId);
        Assert.Equal("in progress", result.Status);
    }

    [Fact]
    public void Handle_ShouldThrowTradeNotFoundException_WhenTradeDoesNotExist()
    {
        // Arrange: Define a non-existent trade ID
        var tradeId = 99;

        // Mock the repository to return null (trade not found)
        _mockTradesRepository.Setup(repo => repo.GetById(tradeId)).Returns((Trades)null);

        // Act & Assert: Ensure the handler throws an exception
        Assert.Throws<TradeNotFoundException>(() => _handler.Handle(tradeId));
    }
}
