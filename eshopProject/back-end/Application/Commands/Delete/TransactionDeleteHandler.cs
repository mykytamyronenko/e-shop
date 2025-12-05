using Infrastructure;

namespace Application.Commands.Delete;

public class TransactionDeleteHandler: ITransactionDeleteCommandHandler
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly TradeShopContext _context;


    public TransactionDeleteHandler(ITransactionsRepository transactionsRepository, TradeShopContext context)
    {
        _transactionsRepository = transactionsRepository;
        _context = context;
    }

    public void Handle(in int id)
    {
        if (_transactionsRepository.GetById(id) is not null)
        {
            _transactionsRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("Transaction not found");
        }

    }
}