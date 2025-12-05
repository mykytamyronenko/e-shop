using Application.Commands.Create;
using AutoMapper;

namespace Tests.Application.Create;

public class TradesCreateHandlerTest
{
    private readonly Mock<ITradesRepository> _mockTradesRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<TradeShopContext> _mockContext;
    private readonly TradeCreateHandler _handler;

    public TradesCreateHandlerTest()
    {
        _mockTradesRepository = new Mock<ITradesRepository>();
        _mockMapper = new Mock<IMapper>();
        _mockContext = new Mock<TradeShopContext>();

        _handler = new TradeCreateHandler(_mockTradesRepository.Object, _mockMapper.Object, _mockContext.Object);
    }

    [Fact]
    public void Handle_ShouldCreateTrade_WhenTraderArticlesIdsIsValid()
    {
        // Arrange
        var command = new TradeCreateCommand
        {
            TraderId = 1,
            ReceiverId = 2,
            TraderArticlesIds = "5,6,7",  // String with comma-separated IDs
            ReceiverArticleId = 3,
            TradeDate = DateTime.UtcNow,
            Status = "in progress"
        };

        _mockMapper.Setup(m => m.Map<TradeCreateOutput>(It.IsAny<Trades>())).Returns(new TradeCreateOutput());
        _mockContext.Setup(c => c.Trades.FirstOrDefault(It.IsAny<Func<Trades, bool>>())).Returns((Trades)null); 

        var result = _handler.Handle(command);

        string expectedResult = "5,6,7"; 
        Assert.Equal(expectedResult, result.TraderArticlesIds); 
    }

}