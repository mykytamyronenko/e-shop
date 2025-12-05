using API.Controllers;
using Application.exceptions;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Microsoft.AspNetCore.Mvc;

namespace Tests.API;

public class TransactionQueryControllerTest
{
    private readonly Mock<TransactionsQueryProcessor> _mockTransactionsQueryProcessor;



    private readonly TransactionQueryController _controller;

    public TransactionQueryControllerTest()
    {
        _mockTransactionsQueryProcessor = new Mock<TransactionsQueryProcessor>();

        _controller = new TransactionQueryController(_mockTransactionsQueryProcessor.Object);
    }
    
    [Fact]
    public void GetAllTransactions_ReturnsOk_WithEmptyList_WhenNoTransactionsExist()
    {
        // Arrange
        var transactionsList = new List<TransactionsGetAllOutput.Transactions>();
    
        _mockTransactionsQueryProcessor.Setup(p => p.GetAll(It.IsAny<TransactionsGetAllQuery>())).Returns(new TransactionsGetAllOutput { TransactionsList = transactionsList });

        // Act
        var result = _controller.GetAllTransactions();

        // Assert
        Assert.IsType<List<TransactionsGetAllOutput.Transactions>>(result);
        Assert.Empty(result);
    }


    [Fact]
    public void GetAllTransactions_ReturnsOk_WithListOfTransactions()
    {
        // Arrange
        var transactionsList = new List<TransactionsGetAllOutput.Transactions>
        {
            new TransactionsGetAllOutput.Transactions { TransactionId = 1, BuyerId = 1, SellerId = 2, ArticleId = 3, TransactionType = "purchase", Price = 100, Status = "finished" },
            new TransactionsGetAllOutput.Transactions { TransactionId = 2, BuyerId = 2, SellerId = 3, ArticleId = 4, TransactionType = "purchase", Price = 200, Status = "in progress" },
        };

        _mockTransactionsQueryProcessor.Setup(p => p.GetAll(It.IsAny<TransactionsGetAllQuery>())).Returns(new TransactionsGetAllOutput { TransactionsList = transactionsList });

        // Act
        var result = _controller.GetAllTransactions();

        // Assert
        Assert.IsType<List<TransactionsGetAllOutput.Transactions>>(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].TransactionId);
        Assert.Equal(2, result[1].TransactionId);
    }

    
    [Fact]
    public void GetByIdTransaction_ReturnsBadRequest_WhenTransactionIdIsLessThanZero()
    {
        // Arrange
        var invalidTransactionId = -1;

        // Act
        var result = _controller.GetByIdTransaction(invalidTransactionId);

        // Assert
        var actionResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("The transaction ID must be more than zero.", actionResult.Value);
    }

    [Fact]
    public void GetByIdTransaction_ReturnsNotFound_WhenTransactionDoesNotExist()
    {
        // Arrange
        var nonExistentTransactionId = 999;
        _mockTransactionsQueryProcessor.Setup(p => p.GetById(nonExistentTransactionId)).Throws(new TransactionNotFoundException(nonExistentTransactionId));

        // Act
        var result = _controller.GetByIdTransaction(nonExistentTransactionId);

        // Assert
        var actionResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Transaction not found", actionResult.Value);
    }

    [Fact]
    public void GetByIdTransaction_ReturnsOk_WhenTransactionIsFound()
    {
        // Arrange
        var transactionId = 1;
        var transaction = new TransactionsGetByIdOutput 
        { 
            TransactionId = transactionId, 
            BuyerId = 1, 
            SellerId = 2, 
            ArticleId = 3, 
            TransactionType = "purchase", 
            Price = 100, 
            Status = "finished" 
        };

        _mockTransactionsQueryProcessor.Setup(p => p.GetById(transactionId)).Returns(transaction);

        // Act
        var result = _controller.GetByIdTransaction(transactionId);

        // Assert
        var actionResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<TransactionsGetByIdOutput>(actionResult.Value);
        Assert.Equal(transaction.TransactionId, value.TransactionId);
        Assert.Equal(transaction.Price, value.Price);
        Assert.Equal(transaction.Status, value.Status);
    }

    [Fact]
    public void GetByIdTransaction_ReturnsInternalServerError_WhenExceptionOccurs()
    {
        // Arrange
        var transactionId = 1;
        _mockTransactionsQueryProcessor.Setup(p => p.GetById(transactionId)).Throws(new Exception("Error occurred"));

        // Act
        var result = _controller.GetByIdTransaction(transactionId);

        // Assert
        var actionResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, actionResult.StatusCode);
        Assert.Equal("Error occurred", actionResult.Value);
    }


}