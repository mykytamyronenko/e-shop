using Domain;

namespace Infrastructure;

public interface IArticlesRepository
{
    List<Articles> GetAll();
    Articles? GetById(int id);
    Articles Create(Articles article);
    bool Update(Articles article);
    bool Delete(int id);
}