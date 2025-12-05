using Domain;

namespace Infrastructure;

public interface ITransactionsRepository
{
    List<Transactions> GetAll();
    Transactions GetById(int id);
    Transactions Create(Transactions transaction);
    bool Update(Transactions transaction);
    bool Delete(int id);
}