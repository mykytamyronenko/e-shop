namespace Tests.Domain;

public class TransactionsTest
{
    [Fact]
    public void BuyerId_ShouldThrowArgumentException_WhenBuyerAndSellerAreSame()
    {
        // Arrange
        var transaction = new Transactions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => transaction.BuyerId = 1); // Same as SellerId
        Assert.Throws<ArgumentException>(() => transaction.SellerId = 1); // Same as BuyerId
    }

    [Fact]
    public void TransactionType_ShouldThrowArgumentException_WhenTypeIsInvalid()
    {
        // Arrange
        var transaction = new Transactions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => transaction.TransactionType = "rent"); // Invalid type
        Assert.Throws<ArgumentException>(() => transaction.TransactionType = "sale"); // Invalid type
    }

    [Fact]
    public void Price_ShouldThrowArgumentException_WhenPriceIsLessThanOne()
    {
        // Arrange
        var transaction = new Transactions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => transaction.Price = 0); // Price is less than 1
        Assert.Throws<ArgumentException>(() => transaction.Price = -1); // Price is negative
    }

    [Fact]
    public void Commission_ShouldThrowArgumentException_WhenCommissionIsNegativeOrGreaterThanPrice()
    {
        // Arrange
        var transaction = new Transactions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => transaction.Commission = -1); // Commission is negative
        Assert.Throws<ArgumentException>(() => transaction.Commission = 100); // Commission is greater than price
    }
    
    [Fact]
    public void TransactionDate_ShouldThrowArgumentException_WhenDateIsInTheFuture()
    {
        // Arrange
        var transaction = new Transactions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => transaction.TransactionDate = DateTime.Now.AddMinutes(1)); // Future date
    }

    [Fact]
    public void Status_ShouldThrowArgumentException_WhenStatusIsInvalid()
    {
        // Arrange
        var transaction = new Transactions();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => transaction.Status = "completed"); // Invalid status
        Assert.Throws<ArgumentException>(() => transaction.Status = "rejected"); // Invalid status
    }

    [Fact]
    public void Status_ShouldSetValue_WhenStatusIsValid()
    {
        // Arrange
        var transaction = new Transactions();

        // Act
        transaction.Status = "in progress";
        transaction.Status = "finished";
        transaction.Status = "cancelled";

        // Assert
        Assert.Equal("in progress", transaction.Status);
        Assert.Equal("finished", transaction.Status);
        Assert.Equal("cancelled", transaction.Status);
    }


}