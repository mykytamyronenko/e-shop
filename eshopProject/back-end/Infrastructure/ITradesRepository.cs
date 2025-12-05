using Domain;

namespace Infrastructure;

public interface ITradesRepository
{
    List<Trades> GetAll();
    Trades GetById(int id);
    Trades Create(Trades trade);
    bool Update(Trades trade);
    bool Delete(int id);
}