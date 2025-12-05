using System.Security.Claims;
using API.Controllers;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.Getall;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class TransactionCommandControllerTest
{
    
    private readonly Mock<TransactionCommandsProcessor> _mockTransactionCommandsProcessor;
    private readonly Mock<TradesQueryProcessor> _mockTradesQueryProcessor;



    private readonly TransactionCommandsController _controller;

    public TransactionCommandControllerTest()
    {
        _mockTransactionCommandsProcessor = new Mock<TransactionCommandsProcessor>();
        _mockTradesQueryProcessor = new Mock<TradesQueryProcessor>();

        _controller = new TransactionCommandsController(_mockTransactionCommandsProcessor.Object,_mockTradesQueryProcessor.Object);
    }
    
    
    [Fact]
    public void CreateTransaction_ReturnsUnauthorized_WhenUserIdNotInToken()
    {
        // Arrange
        var command = new TransactionCreateCommand();
        var controller = new TransactionCommandsController(_mockTransactionCommandsProcessor.Object, _mockTradesQueryProcessor.Object);

        // Act
        var result = controller.CreateTransaction(command);

        // Assert
        var actionResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid token: User ID not found.", actionResult.Value);
    }

    [Fact]
    public void CreateTransaction_ReturnsForbid_WhenBuyerIdNotEqualToUserId()
    {
        // Arrange
        var userIdFromToken = 1;
        var command = new TransactionCreateCommand { BuyerId = 2 };

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Act
        var result = _controller.CreateTransaction(command);

        // Assert
        Assert.IsType<ForbidResult>(result);
    }

    [Fact]
    public void CreateTransaction_ReturnsBadRequest_WhenCommandIsNull()
    {
        // Act
        var result = _controller.CreateTransaction(null);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Invalid transaction data.", actionResult.Value);
    }

    [Fact]
    public void CreateTransaction_ReturnsBadRequest_WhenArticleIsInProgressTrade()
    {
        // Arrange
        var userIdFromToken = 1;
        var command = new TransactionCreateCommand { BuyerId = userIdFromToken, ArticleId = 42 };
        
        var tradesList = new List<TradesGetAllOutput.Trades>
        {
            new TradesGetAllOutput.Trades 
            { 
                ReceiverArticleId = 42, 
                Status = "in progress", 
                TraderArticlesIds = "43" 
            }
        };

        _mockTradesQueryProcessor.Setup(p => p.GetAll(null)).Returns(new TradesGetAllOutput { TradesList = tradesList });

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Act
        var result = _controller.CreateTransaction(command);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("You can not buy an item that is waiting to be traded", actionResult.Value);
    }

    [Fact]
    public void CreateTransaction_ReturnsOk_WhenTransactionIsSuccessful()
    {
        // Arrange
        var userIdFromToken = 1;
        var command = new TransactionCreateCommand { BuyerId = userIdFromToken, ArticleId = 42 };

        var transactionResult = new TransactionCreateOutput();
        _mockTransactionCommandsProcessor.Setup(p => p.CreateTransaction(command)).Returns(transactionResult);

        var identity = new ClaimsIdentity(new Claim[] { new Claim("userId", userIdFromToken.ToString()) }, "mock");
        var principal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };

        // Act
        var result = _controller.CreateTransaction(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(actionResult.Value);
    }

    [Fact]
    public void CreateTransaction_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var command = new TransactionCreateCommand();
        _mockTransactionCommandsProcessor.Setup(p => p.CreateTransaction(command)).Throws(new Exception("Error occurred"));

        // Act
        var result = _controller.CreateTransaction(command);

        // Assert
        var actionResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, actionResult.StatusCode);
    }
    [Fact]
    public void UpdateTransaction_ReturnsOk_WhenTransactionIsUpdatedSuccessfully()
    {
        // Arrange
        var command = new TransactionUpdateCommand();

        // Act
        var result = _controller.UpdateTransaction(command);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Transaction updated.", actionResult.Value);
    }
    
    [Fact]
    public void DeleteTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        // Arrange
        var transactionId = 999;
        _mockTransactionCommandsProcessor.Setup(p => p.DeleteTransaction(transactionId)).Throws(new InvalidOperationException());

        // Act
        var result = _controller.DeleteTransaction(transactionId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal($"Transaction with ID {transactionId} not found.", actionResult.Value);
    }

    [Fact]
    public void DeleteTransaction_ReturnsNoContent_WhenTransactionIsDeletedSuccessfully()
    {
        // Arrange
        var transactionId = 1;

        // Act
        var result = _controller.DeleteTransaction(transactionId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public void DeleteTransaction_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var transactionId = 1;
        _mockTransactionCommandsProcessor.Setup(p => p.DeleteTransaction(transactionId)).Throws(new Exception("Error occurred"));

        // Act
        var result = _controller.DeleteTransaction(transactionId);

        // Assert
        var actionResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, actionResult.StatusCode);
    }



}