using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;

public class TradeCreateHandler: ICommandsHandler<TradeCreateCommand, TradeCreateOutput>
{
    private readonly ITradesRepository _tradesRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;
    
    public TradeCreateHandler(ITradesRepository tradesRepository, IMapper mapper, TradeShopContext context)
    {
        _tradesRepository = tradesRepository;
        _mapper = mapper;
        _context = context;
    }

    public TradeCreateOutput Handle(TradeCreateCommand input) {
        var allowedStatuses = new[] { "in progress", "accepted", "denied" };
        if (!allowedStatuses.Contains(input.Status))
        {
            throw new ArgumentException("Invalid status. Allowed values are 'in progress', 'accepted', and 'denied'.");
        }
        var trade = new Trades
        {
            TraderId = input.TraderId,
            ReceiverId = input.ReceiverId,
            TraderArticlesIds = input.TraderArticlesIds,
            ReceiverArticleId = input.ReceiverArticleId,
            TradeDate = input.TradeDate,
            Status = string.IsNullOrEmpty(input.Status) ? "in progress" : input.Status,
        };
 
        
        var existingTrade = _context.Trades.FirstOrDefault(t => t.TradeId == trade.TradeId);

        if (existingTrade != null)
        {
            throw new Exception("Trade already exists.");
        }
        
        
        _tradesRepository.Create(trade);
        _context.SaveChanges();
        return _mapper.Map<TradeCreateOutput>(trade);
    }
}