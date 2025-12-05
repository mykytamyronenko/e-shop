using Domain;

namespace Infrastructure;

public class RatingsRepository: IRatingsRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public RatingsRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }

    public List<Ratings> GetAll()
    {
        return _tradeShopContext.Ratings.ToList();
    }

    public Ratings? GetById(int id)
    {
        return _tradeShopContext.Ratings.FirstOrDefault(rating => rating.RatingId == id);
    }

    public Ratings Create(Ratings rating)
    {
        _tradeShopContext.Ratings.Add(rating);
        _tradeShopContext.SaveChanges();
        return new Ratings
        {
            RatingId = rating.RatingId,
            UserId = rating.UserId,
            ReviewerId = rating.ReviewerId,
            Score = rating.Score,
            Comment = rating.Comment,
            CreatedAt = rating.CreatedAt
        };
    }
    
    public bool Update(Ratings rating)
    {
        var entity = _tradeShopContext.Ratings.FirstOrDefault(t => t.RatingId == rating.RatingId);
        
        if (entity == null)
        {
            return false;
        }

        entity.RatingId = rating.RatingId;
        entity.UserId = rating.UserId;
        entity.ReviewerId = rating.ReviewerId;
        entity.Score = rating.Score;
        entity.Comment = rating.Comment;
        entity.CreatedAt = rating.CreatedAt;
        
        _tradeShopContext.SaveChanges();
        return true;

    }



    public bool Delete(int id)
    {
        var entity = _tradeShopContext.Ratings.FirstOrDefault(r => r.RatingId == id);

        if (entity == null)
        {
            return false;
        }
        
        _tradeShopContext.Ratings.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }
}