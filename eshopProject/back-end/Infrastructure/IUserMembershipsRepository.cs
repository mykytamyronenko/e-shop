using Domain;

namespace Infrastructure;

public interface IUserMembershipsRepository
{
    List<UserMemberships> GetAll();
    UserMemberships? GetById(int id);
    UserMemberships Create(UserMemberships userMemberships);
    bool Update(UserMemberships userMemberships);
    bool Delete(int id);
    IEnumerable<UserMemberships> GetExpiredMemberships(DateTime now);
    
}