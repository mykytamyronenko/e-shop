using API.Controllers;
using Application.Commands;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class TradeQueryControllerTest
{
    private readonly Mock<TradesQueryProcessor> _mockTradesQueryProcessor;

    private readonly TradeQueryController _controller;

    public TradeQueryControllerTest()
    {
        _mockTradesQueryProcessor = new Mock<TradesQueryProcessor>();
        _controller = new TradeQueryController(_mockTradesQueryProcessor.Object);
    }
    
    [Fact]
    public void GetAllTrades_ReturnsOk_WhenTradesExist()
    {
        // Arrange
        var tradesList = new List<TradesGetAllOutput.Trades>
        {
            new TradesGetAllOutput.Trades { TradeId = 1, TraderId = 1, ReceiverId = 2, TraderArticlesIds = "1,2", ReceiverArticleId = 3, TradeDate = DateTime.UtcNow, Status = "in progress" },
            new TradesGetAllOutput.Trades { TradeId = 2, TraderId = 2, ReceiverId = 3, TraderArticlesIds = "4,5", ReceiverArticleId = 6, TradeDate = DateTime.UtcNow, Status = "accepted" }
        };

        _mockTradesQueryProcessor.Setup(p => p.GetAll(It.IsAny<TradesGetAllQuery>())).Returns(new TradesGetAllOutput { TradesList = tradesList });
            
        // Act
        var result = _controller.GetAllTrades();

        // Assert
        var actionResult = Assert.IsType<List<TradesGetAllOutput.Trades>>(result);
        Assert.Equal(2, actionResult.Count);
    }

    
    [Fact]
    public void GetByIdTrade_ReturnsOk_WhenTradeExists()
    {
        // Arrange
        var trade = new TradesGetByIdOutput
        {
            TradeId = 1,
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "1, 2",
            ReceiverArticleId = 3,
            TradeDate = DateTime.UtcNow,
            Status = "in progress"
        };
    
        _mockTradesQueryProcessor.Setup(p => p.GetById(1)).Returns(trade);

        // Act
        var result = _controller.GetByIdTrade(1);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<TradesGetByIdOutput>(actionResult.Value);
        Assert.Equal(1, value.TradeId);
    }

    [Fact]
    public void GetByIdTrade_ReturnsNotFound_WhenTradeDoesNotExist()
    {
        // Arrange
        _mockTradesQueryProcessor.Setup(p => p.GetById(It.IsAny<int>())).Throws(new TradeNotFoundException(999));
        // Act
        var result = _controller.GetByIdTrade(999);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Trade not found", actionResult.Value);
    }

    [Fact]
    public void GetByIdTrade_ReturnsBadRequest_WhenIdIsInvalid()
    {
        // Act
        var result = _controller.GetByIdTrade(-1);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The trade ID must be more than zero.", actionResult.Value);
    }


}