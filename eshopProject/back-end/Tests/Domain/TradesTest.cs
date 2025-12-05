namespace Tests.Domain;

public class TradesTest
{
    [Fact]
    public void TraderId_ShouldThrowArgumentException_WhenTraderAndReceiverAreSame()
    {
        // Arrange
        var trade = new Trades();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => trade.TraderId = 1); // Same as ReceiverId
        Assert.Throws<ArgumentException>(() => trade.ReceiverId = 1); // Same as TraderId
    }

    [Fact]
    public void TraderArticlesIds_ShouldThrowArgumentException_WhenIdsContainInvalidInteger()
    {
        // Arrange
        var trade = new Trades();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => trade.TraderArticlesIds = "1,abc,3"); // Invalid integer ("abc")
        Assert.Throws<ArgumentException>(() => trade.TraderArticlesIds = "1,,3"); // Empty value
    }

    [Fact]
    public void ReceiverArticleId_ShouldThrowArgumentException_WhenIdIsZeroOrNegative()
    {
        // Arrange
        var trade = new Trades();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => trade.ReceiverArticleId = 0); // Zero
        Assert.Throws<ArgumentException>(() => trade.ReceiverArticleId = -1); // Negative value
    }
    
    [Fact]
    public void TradeDate_ShouldThrowArgumentException_WhenDateIsInTheFuture()
    {
        // Arrange
        var trade = new Trades();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => trade.TradeDate = DateTime.Now.AddMinutes(1)); // Future date
    }
    
    [Fact]
    public void Status_ShouldThrowArgumentException_WhenStatusIsInvalid()
    {
        // Arrange
        var trade = new Trades();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => trade.Status = "completed"); // Invalid status
        Assert.Throws<ArgumentException>(() => trade.Status = "rejected"); // Invalid status
    }

    [Fact]
    public void Status_ShouldSetValue_WhenStatusIsValid()
    {
        // Arrange
        var trade = new Trades();

        // Act
        trade.Status = "in progress";
        trade.Status = "accepted";
        trade.Status = "denied";

        // Assert
        Assert.Equal("in progress", trade.Status);
        Assert.Equal("accepted", trade.Status);
        Assert.Equal("denied", trade.Status);
    }


}