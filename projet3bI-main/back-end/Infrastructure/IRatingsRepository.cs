using Domain;

namespace Infrastructure;

public interface IRatingsRepository
{
    List<Ratings> GetAll();
    Ratings GetById(int id);
    Ratings Create(Ratings rating);
    bool Update(Ratings rating);
    bool Delete(int id);
}