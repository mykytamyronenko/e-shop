using Infrastructure;

namespace Application.Commands.Delete;

public class TradeDeleteHandler: ITradeDeleteCommandHandler
{
    private readonly ITradesRepository _tradesRepository;
    private readonly TradeShopContext _context;


    public TradeDeleteHandler(ITradesRepository tradesRepository, TradeShopContext context)
    {
        _tradesRepository = tradesRepository;
        _context = context;
    }

    public void Handle(in int id)
    {
        if (_tradesRepository.GetById(id) is not null)
        {
            _tradesRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("Trade not found");
        }

    }
}