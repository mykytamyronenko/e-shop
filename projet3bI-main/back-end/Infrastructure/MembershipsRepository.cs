using Domain;

namespace Infrastructure;

public class MembershipsRepository : IMembershipsRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public MembershipsRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }

    public List<Memberships> GetAll()
    {
        return _tradeShopContext.Memberships.ToList();
    }

    public Memberships? GetById(int id)
    {
        return _tradeShopContext.Memberships.FirstOrDefault(membership => membership.MembershipId == id);
    }

    public Memberships Create(Memberships membership)
    {
        _tradeShopContext.Memberships.Add(membership);
        _tradeShopContext.SaveChanges();
        return new Memberships
        {
            MembershipId = membership.MembershipId,
            Name = membership.Name,
            Price = membership.Price,
            DiscountPercentage = membership.DiscountPercentage,
            Description = membership.Description
        };
    }
    
    public bool Update(Memberships membership)
    {
        var entity = _tradeShopContext.Memberships.FirstOrDefault(m => m.MembershipId == membership.MembershipId);
        
        if (entity == null)
        {
            return false;
        }

        entity.MembershipId = membership.MembershipId;
        entity.Name = membership.Name;
        entity.Price = membership.Price;
        entity.DiscountPercentage = membership.DiscountPercentage;
        entity.Description = membership.Description;
        
        _tradeShopContext.SaveChanges();
        return true;

    }



    public bool Delete(int id)
    {
        var entity = _tradeShopContext.Memberships.FirstOrDefault(m => m.MembershipId == id);

        if (entity == null)
        {
            return false;
        }
        
        _tradeShopContext.Memberships.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }
}