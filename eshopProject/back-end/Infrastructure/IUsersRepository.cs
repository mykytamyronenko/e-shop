using Domain;

namespace Infrastructure;

public interface IUsersRepository
{
    List<Users> GetAll();
    Users? GetById(int id);
    Users Create(Users user);
    bool Update(Users user);
    bool Delete(int id);
    Users? GetUserByUsername(string username);
    Users? GetUserByEmail(string email);

    
}