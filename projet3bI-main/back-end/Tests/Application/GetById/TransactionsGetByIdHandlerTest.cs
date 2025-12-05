using Application.exceptions;
using Application.Queries.getById;

namespace Tests.Application.GetById;

using Moq;
using Xunit;
using AutoMapper;
using System;

public class TransactionsGetByIdHandlerTest
{
    private readonly Mock<ITransactionsRepository> _mockTransactionsRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TransactionsGetByIdHandler _handler;

    public TransactionsGetByIdHandlerTest()
    {
        // Mock the dependencies
        _mockTransactionsRepository = new Mock<ITransactionsRepository>();
        _mockMapper = new Mock<IMapper>();

        // Instantiate the handler
        _handler = new TransactionsGetByIdHandler(_mockTransactionsRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public void Handle_ShouldReturnTransaction_WhenTransactionExists()
    {
        // Arrange: Define a mock transaction entity
        var transactionId = 1;
        var dbTransaction = new Transactions { TransactionId = transactionId, BuyerId = 123, SellerId = 456, Price = 100.50m, TransactionDate = DateTime.Now, Status = "finished" };
        var outputTransaction = new TransactionsGetByIdOutput { TransactionId = transactionId, BuyerId = 123, SellerId = 456, Price = 100.50m, TransactionDate = DateTime.Now, Status = "finished" };

        // Mock the repository to return the transaction
        _mockTransactionsRepository.Setup(repo => repo.GetById(transactionId)).Returns(dbTransaction);

        // Mock the mapper to map the entity to the output DTO
        _mockMapper.Setup(m => m.Map<TransactionsGetByIdOutput>(dbTransaction)).Returns(outputTransaction);

        // Act: Call the handler's handle method
        var result = _handler.Handle(transactionId);

        // Assert: Validate the result
        Assert.NotNull(result);
        Assert.Equal(transactionId, result.TransactionId);
        Assert.Equal(123, result.BuyerId);
        Assert.Equal(456, result.SellerId);
        Assert.Equal(100.50m, result.Price);
        Assert.Equal("finished", result.Status);
    }

    [Fact]
    public void Handle_ShouldThrowTransactionNotFoundException_WhenTransactionDoesNotExist()
    {
        // Arrange: Define a non-existent transaction ID
        var transactionId = 99;

        // Mock the repository to return null (transaction not found)
        _mockTransactionsRepository.Setup(repo => repo.GetById(transactionId)).Returns((Transactions)null);

        // Act & Assert: Ensure the handler throws an exception
        Assert.Throws<TransactionNotFoundException>(() => _handler.Handle(transactionId));
    }
}
