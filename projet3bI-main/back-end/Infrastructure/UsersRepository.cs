using Domain;

namespace Infrastructure;

public class UsersRepository : IUsersRepository
{
    private readonly TradeShopContext _tradeShopContext;

    public UsersRepository(TradeShopContext tradeShopContext)
    {
        _tradeShopContext = tradeShopContext;
    }

    public List<Users> GetAll()
    {
        return _tradeShopContext.Users.ToList();
    }

    public Users? GetById(int id)
    {
        return _tradeShopContext.Users.FirstOrDefault(user => user.UserId == id);
    }

    public Users Create(Users user)
    {
        _tradeShopContext.Users.Add(user);
        _tradeShopContext.SaveChanges();
        return new Users
        {
            UserId = user.UserId,
            Password = user.Password,
            Email = user.Email,
            Username = user.Username,
            Role = user.Role,
            ProfilePicture = user.ProfilePicture,
            MembershipLevel = user.MembershipLevel,
            Rating = user.Rating,
            Status = user.Status,
            Balance = user.Balance
        };
    }
    
    public bool Update(Users user)
    {
        var entity = _tradeShopContext.Users.FirstOrDefault(e => e.UserId == user.UserId);

        if (entity == null)
        {
            return false;
        }

        entity.Password = user.Password;
        entity.Email = user.Email;
        entity.Username = user.Username;
        entity.Role = user.Role;
        entity.ProfilePicture = user.ProfilePicture;
        entity.MembershipLevel = user.MembershipLevel;
        entity.Rating = user.Rating;
        entity.Status = user.Status;
        entity.Balance = user.Balance;


        
        _tradeShopContext.SaveChanges();
        return true;

    }



    public bool Delete(int id)
    {
        var entity = _tradeShopContext.Users.FirstOrDefault(e => e.UserId == id);

        if (entity == null)
        {
            Console.WriteLine($"User with ID {id} not found for deletion");
            return false;
        }
        Console.WriteLine($"Deleting user with ID {id}");
        _tradeShopContext.Users.Remove(entity);
        _tradeShopContext.SaveChanges();
        return true;
        
    }

    public Users? GetUserByUsername(string username)
    {
        return _tradeShopContext.Users.SingleOrDefault(u => u.Username == username);
    }

    public Users? GetUserByEmail(string email)
    {
        return _tradeShopContext.Users.SingleOrDefault(u => u.Email == email);
    }
}