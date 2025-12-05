using Application.Queries.Getall;

namespace Tests.Application.GetAll;

using Moq;
using Xunit;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;

public class TransactionsGetAllHandlerTest
{
    private readonly Mock<ITransactionsRepository> _mockTransactionsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TransactionsGetAllHandler _handler;

    public TransactionsGetAllHandlerTest()
    {
        // Initialize mocks for repository and mapper
        _mockTransactionsRepository = new Mock<ITransactionsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Create handler with mocked dependencies
        _handler = new TransactionsGetAllHandler(_mockTransactionsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnTransactions_WhenTransactionsExist()
    {
        // Arrange: Define mock transaction data
        var dbTransactions = new List<Transactions>
        {
            new Transactions { TransactionId = 1, BuyerId = 1, SellerId = 2, Price = 100.00m, TransactionDate = DateTime.Now, Status = "completed" },
            new Transactions { TransactionId = 2, BuyerId = 3, SellerId = 4, Price = 50.00m, TransactionDate = DateTime.Now, Status = "pending" }
        };

        // Mock repository to return the list of transactions
        _mockTransactionsRepository.Setup(repo => repo.GetAll()).Returns(dbTransactions);

        // Mock mapper to map the db entities to output DTO
        _mockMapper.Setup(m => m.Map<List<TransactionsGetAllOutput.Transactions>>(It.IsAny<List<Transactions>>()))
                   .Returns(dbTransactions.Select(t => new TransactionsGetAllOutput.Transactions
                   {
                       TransactionId = t.TransactionId,
                       BuyerId = t.BuyerId,
                       SellerId = t.SellerId,
                       Price = t.Price,
                       TransactionDate = t.TransactionDate,
                       Status = t.Status
                   }).ToList());

        // Act: Call the handler method
        var result = _handler.Handle(new TransactionsGetAllQuery());

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(2, result.TransactionsList.Count);
        Assert.Equal(1, result.TransactionsList[0].TransactionId);
        Assert.Equal(100.00m, result.TransactionsList[0].Price);
        Assert.Equal("completed", result.TransactionsList[0].Status);
    }

    [Fact]
    public void Handle_ShouldReturnEmptyList_WhenNoTransactionsExist()
    {
        // Arrange: Empty list of transactions
        var dbTransactions = new List<Transactions>();

        // Mock repository to return an empty list
        _mockTransactionsRepository.Setup(repo => repo.GetAll()).Returns(dbTransactions);

        // Act: Call the handler method
        var result = _handler.Handle(new TransactionsGetAllQuery());

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Empty(result.TransactionsList); // Should return an empty list
    }
}
