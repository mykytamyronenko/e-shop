using Domain;

namespace Infrastructure;

public class TransactionsRepository: ITransactionsRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public TransactionsRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }

    public List<Transactions> GetAll()
    {
        return _tradeShopContext.Transactions.ToList();
    }

    public Transactions? GetById(int id)
    {
        return _tradeShopContext.Transactions.FirstOrDefault(transaction => transaction.TransactionId == id);
    }

    public Transactions Create(Transactions transaction)
    {
        _tradeShopContext.Transactions.Add(transaction);
        _tradeShopContext.SaveChanges();
        return new Transactions
        {
            TransactionId = transaction.TransactionId,
            BuyerId = transaction.BuyerId,
            SellerId = transaction.SellerId,
            ArticleId = transaction.ArticleId,
            TransactionType = transaction.TransactionType,
            Price = transaction.Price,
            Commission = transaction.Commission,
            TransactionDate = transaction.TransactionDate,
            Status = transaction.Status
        };
    }
    
    public bool Update(Transactions transaction)
    {
        var entity = _tradeShopContext.Transactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
        
        if (entity == null)
        {
            return false;
        }

        entity.TransactionId = transaction.TransactionId;
        entity.BuyerId = transaction.BuyerId;
        entity.SellerId = transaction.SellerId;
        entity.ArticleId = transaction.ArticleId;
        entity.TransactionType = transaction.TransactionType;
        entity.Price = transaction.Price;
        entity.Commission = transaction.Commission;
        entity.TransactionDate = transaction.TransactionDate;
        entity.Status = transaction.Status;
        
        _tradeShopContext.SaveChanges();
        return true;

    }



    public bool Delete(int id)
    {
        var entity = _tradeShopContext.Transactions.FirstOrDefault(t => t.TransactionId == id);

        if (entity == null)
        {
            return false;
        }
        
        _tradeShopContext.Transactions.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }
}