using Domain;

namespace Infrastructure;

public class TradesRepository: ITradesRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public TradesRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }

    public List<Trades> GetAll()
    {
        return _tradeShopContext.Trades.ToList();
    }

    public Trades? GetById(int id)
    {
        return _tradeShopContext.Trades.FirstOrDefault(trade => trade.TradeId == id);
    }

    public Trades Create(Trades trade)
    {
        _tradeShopContext.Trades.Add(trade);
        _tradeShopContext.SaveChanges();
        return new Trades
        {
            TradeId = trade.TradeId,
            TraderId = trade.TraderId,
            ReceiverId = trade.ReceiverId,
            TraderArticlesIds = trade.TraderArticlesIds,
            ReceiverArticleId = trade.ReceiverArticleId,
            TradeDate = trade.TradeDate,
            Status = trade.Status
        };
    }
    
    public bool Update(Trades trade)
    {
        var entity = _tradeShopContext.Trades.FirstOrDefault(t => t.TradeId == trade.TradeId);
        
        if (entity == null)
        {
            return false;
        }
        
        entity.TradeId = trade.TradeId;
        entity.TraderId = trade.TraderId;
        entity.ReceiverId = trade.ReceiverId;
        entity.TraderArticlesIds = trade.TraderArticlesIds;
        entity.ReceiverArticleId = trade.ReceiverArticleId;
        entity.TradeDate = trade.TradeDate;
        entity.Status = trade.Status;
        
        _tradeShopContext.SaveChanges();
        return true;

    }
    

    public bool Delete(int id)
    {
        var entity = _tradeShopContext.Trades.FirstOrDefault(t => t.TradeId == id);

        if (entity == null)
        {
            return false;
        }
        
        _tradeShopContext.Trades.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }
}