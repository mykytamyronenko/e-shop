using Domain;

namespace Infrastructure;

public class UserMembershipsRepository : IUserMembershipsRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public UserMembershipsRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }


    public List<UserMemberships> GetAll()
    {
        return _tradeShopContext.UserMemberships.ToList();
    }

    public UserMemberships? GetById(int id)
    {
        return _tradeShopContext.UserMemberships.FirstOrDefault(userMembership => userMembership.UserMembershipId == id);
    }

    public UserMemberships Create(UserMemberships userMemberships)
    {
        _tradeShopContext.UserMemberships.Add(userMemberships);
        _tradeShopContext.SaveChanges();
        return new UserMemberships
        {
            UserMembershipId = userMemberships.UserMembershipId,
            UserId = userMemberships.UserId,
            MembershipId = userMemberships.MembershipId,
            StartDate = userMemberships.StartDate,
            EndDate = userMemberships.EndDate,
            Status = userMemberships.Status
        };
    }
    
    public bool Update(UserMemberships userMemberships)
    {
        var entity = _tradeShopContext.UserMemberships.FirstOrDefault(um => um.UserMembershipId == userMemberships.UserMembershipId);

        if (entity == null)
        {
            return false;
        }

        entity.UserId = userMemberships.UserId;
        entity.MembershipId = userMemberships.MembershipId;
        entity.StartDate = userMemberships.StartDate;
        entity.EndDate = userMemberships.EndDate;
        entity.Status = userMemberships.Status;
        

        
        _tradeShopContext.SaveChanges();
        return true;

    }



    public bool Delete(int id)
    {
        var entity = _tradeShopContext.UserMemberships.FirstOrDefault(um => um.UserMembershipId == id);

        if (entity == null)
        {
            Console.WriteLine($"user membership with ID {id} not found");
            return false;
        }
        _tradeShopContext.UserMemberships.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }
    
    public IEnumerable<UserMemberships> GetExpiredMemberships(DateTime now)
    {
        return _tradeShopContext.UserMemberships.Where(m => m.EndDate <= now).ToList();
    }
}