using System.Dynamic;
using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.getById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class TradeCommandControllerTest
{
    private readonly Mock<TradeCommandsProcessor> _mockTradeCommandsProcessor;
    private readonly Mock<TradesQueryProcessor> _mockTradesQueryProcessor;
    private readonly Mock<ArticlesQueryProcessor> _mockArticlesQueryProcessor;
    private readonly Mock<ArticleCommandsProcessor> _mockArticleCommandsProcessor;

    private readonly TradeCommandsController _controller;

    public TradeCommandControllerTest()
    {
        _mockTradeCommandsProcessor = new Mock<TradeCommandsProcessor>();
        _mockTradesQueryProcessor = new Mock<TradesQueryProcessor>();
        _mockArticlesQueryProcessor= new Mock<ArticlesQueryProcessor>();
        _mockArticleCommandsProcessor= new Mock<ArticleCommandsProcessor>();
        _controller = new TradeCommandsController(_mockTradeCommandsProcessor.Object,_mockTradesQueryProcessor.Object,_mockArticlesQueryProcessor.Object,_mockArticleCommandsProcessor.Object);
    }
    
    [Fact]
    public void CreateTrade_ReturnsOk_WhenTradeIsValid()
    {
        // Arrange
        var userIdFromToken = 1;
        var command = new TradeCreateCommand
        {
            TraderId = userIdFromToken,
            TraderArticlesIds = "1, 2",
            ReceiverArticleId = 3
        };

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        _mockArticlesQueryProcessor.Setup(p => p.GetById(It.IsAny<int>())).Returns(new ArticlesGetByIdOutput { ArticleId = 1, UserId = userIdFromToken });
            
        _mockTradeCommandsProcessor.Setup(p => p.CreateTrade(It.IsAny<TradeCreateCommand>())).Returns(new TradeCreateOutput());  // Remplacer TransactionCreateOutput par TradeCreateOutput

        // Act
        var result = _controller.CreateTrade(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(actionResult.Value);
    }

    
    [Fact]
    public void DeleteTrade_ReturnsNoContent_WhenTradeIsDeleted()
    {
        // Arrange
        var userIdFromToken = 1;
        var tradeId = 1;
    
        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        var trade = new TradesGetByIdOutput { TradeId = tradeId, TraderId = userIdFromToken, ReceiverId = userIdFromToken };
        _mockTradesQueryProcessor.Setup(p => p.GetById(tradeId)).Returns(trade);

        _mockTradeCommandsProcessor.Setup(p => p.DeleteTrade(It.IsAny<int>()));

        // Act
        var result = _controller.DeleteTrade(tradeId);

        // Assert
        var actionResult = Assert.IsType<NoContentResult>(result);
    }
    
    [Fact]
    public void UpdateTrade_ReturnsOk_WhenUserIsAuthorized()
    {
        // Arrange
        var tradeId = 1;
        var userIdFromToken = 1;
        var command = new TradeUpdateCommand
        {
            TradeId = tradeId,
            ReceiverId = userIdFromToken,
            Status = "in progress",
            TraderId = 2,
            TradeDate = DateTime.UtcNow,
            ReceiverArticleId = 3,
            TraderArticlesIds = "1, 2"
        };

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        _mockTradeCommandsProcessor.Setup(p => p.UpdateTrade(It.IsAny<TradeUpdateCommand>()));

        // Act
        var result = _controller.UpdateTrade(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = Assert.IsType<ExpandoObject>(actionResult.Value);
        var message = resultValue.GetType().GetProperty("message").GetValue(resultValue, null).ToString();
        Assert.Equal("Trade updated.", message);
    }

    [Fact]
    public void UpdateTradeStatus_ReturnsOk_WhenStatusIsAccepted()
    {
        // Arrange
        var tradeId = 1;
        var userIdFromToken = 1;
        var statusUpdateCommand = new TradeUpdateCommandStatus
        {
            Status = "accepted"
        };

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        var trade = new TradesGetByIdOutput
        {
            TradeId = tradeId,
            TraderId = 2,
            ReceiverId = userIdFromToken,
            TraderArticlesIds = "1, 2",
            ReceiverArticleId = 3
        };
        _mockTradesQueryProcessor.Setup(p => p.GetById(tradeId)).Returns(trade);

        _mockArticlesQueryProcessor.Setup(p => p.GetById(It.IsAny<int>())).Returns(new ArticlesGetByIdOutput());

        _mockTradeCommandsProcessor.Setup(p => p.UpdateTrade(It.IsAny<TradeUpdateCommand>()));

        // Act
        var result = _controller.UpdateTradeStatus(tradeId, statusUpdateCommand);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var resultValue = Assert.IsType<ExpandoObject>(actionResult.Value);
        var message = resultValue.GetType().GetProperty("message").GetValue(resultValue, null).ToString();
        Assert.Equal("Trade status and articles updated successfully.", message);
        Assert.Equal("accepted", resultValue.GetType().GetProperty("newStatus").GetValue(resultValue, null));
    }



}