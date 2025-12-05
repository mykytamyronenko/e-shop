using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

public class TradesGetAllHandlerTest
{
    private readonly Mock<ITradesRepository> _mockTradeRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TradesGetAllHandler _handler;

    public TradesGetAllHandlerTest()
    {
        // Create mocks for the repository and mapper
        _mockTradeRepository = new Mock<ITradesRepository>();
        _mockMapper = new Mock<IMapper>();

        // Inject the mocks into the handler
        _handler = new TradesGetAllHandler(_mockTradeRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnTrades_WhenTradesExist()
    {
        // Arrange
        var dbTrades = new List<Trades>
        {
            new Trades { TradeId = 1, TraderId = 1, ReceiverId = 2, TraderArticlesIds = "1,2,3", ReceiverArticleId = 4, TradeDate = DateTime.Now, Status = "completed" },
            new Trades { TradeId = 2, TraderId = 2, ReceiverId = 3, TraderArticlesIds = "5,6", ReceiverArticleId = 7, TradeDate = DateTime.Now, Status = "pending" }
        };

        // Mock repository method to return a list of trades
        _mockTradeRepository.Setup(repo => repo.GetAll()).Returns(dbTrades);

        // Mock mapper method to map the trades
        _mockMapper.Setup(m => m.Map<List<TradesGetAllOutput.Trades>>(It.IsAny<List<Trades>>()))
                   .Returns(dbTrades.Select(t => new TradesGetAllOutput.Trades
                   {
                       TradeId = t.TradeId,
                       TraderId = t.TraderId,
                       ReceiverId = t.ReceiverId,
                       TraderArticlesIds = t.TraderArticlesIds,
                       ReceiverArticleId = t.ReceiverArticleId,
                       TradeDate = t.TradeDate,
                       Status = t.Status
                   }).ToList());

        // Act
        var result = _handler.Handle(new TradesGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.TradesList.Count);
        Assert.Equal(1, result.TradesList[0].TradeId);
        Assert.Equal("1,2,3", result.TradesList[0].TraderArticlesIds);
        Assert.Equal(4, result.TradesList[0].ReceiverArticleId);
        Assert.Equal("completed", result.TradesList[0].Status);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoTradesExist()
    {
        // Arrange
        var dbTrades = new List<Trades>(); // No trades

        // Mock repository method to return an empty list
        _mockTradeRepository.Setup(repo => repo.GetAll()).Returns(dbTrades);

        // Act
        var result = _handler.Handle(new TradesGetAllQuery());

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.TradesList); // Should return an empty list
    }
}
