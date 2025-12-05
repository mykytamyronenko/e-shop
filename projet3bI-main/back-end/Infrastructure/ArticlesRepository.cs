using Domain;

namespace Infrastructure;

public class ArticlesRepository : IArticlesRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public ArticlesRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }

    public List<Articles> GetAll()
    {
        return _tradeShopContext.Articles.ToList();
    }

    public Articles? GetById(int id)
    {
        return _tradeShopContext.Articles.FirstOrDefault(article => article.ArticleId == id);
    }

    public Articles Create(Articles article)
    {
        _tradeShopContext.Articles.Add(article);
        _tradeShopContext.SaveChanges();
        return new Articles
        {
            ArticleId = article.ArticleId,
            Title = article.Title,
            Description = article.Description,
            Price = article.Price,
            Category = article.Category,
            State = article.State,
            UserId = article.UserId,
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt,
            Status = article.Status,
            MainImageUrl = article.MainImageUrl,
            Quantity = article.Quantity
        };
    }
    
    public bool Update(Articles article)
    {
        var entity = _tradeShopContext.Articles.FirstOrDefault(a => a.ArticleId == article.ArticleId);
        
        if (entity == null)
        {
            return false;
        }

        entity.Title = article.Title;
        entity.Description = article.Description;
        entity.Price = article.Price;
        entity.Category = article.Category;
        entity.State = article.State;
        entity.CreatedAt = article.CreatedAt;
        entity.UpdatedAt = article.UpdatedAt;
        entity.Status = article.Status;
        entity.MainImageUrl = article.MainImageUrl;
        entity.Quantity = article.Quantity;
        
        _tradeShopContext.SaveChanges();
        return true;

    }



    public bool Delete(int id)
    {
        var entity = _tradeShopContext.Articles.FirstOrDefault(a => a.ArticleId == id);

        if (entity == null)
        {
            return false;
        }
        
        _tradeShopContext.Articles.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }
    
}