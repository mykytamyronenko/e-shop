using Domain;

namespace Infrastructure;

public interface IMembershipsRepository
{
    List<Memberships> GetAll();
    Memberships? GetById(int id);
    Memberships Create(Memberships membership);
    bool Update(Memberships memberships);
    bool Delete(int id);
}