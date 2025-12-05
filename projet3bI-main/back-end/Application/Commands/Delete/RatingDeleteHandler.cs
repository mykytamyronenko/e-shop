using Application.utils;
using Infrastructure;

namespace Application.Commands.Delete;

public class RatingDeleteHandler: IRatingDeleteCommandHandler
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly TradeShopContext _context;


    public RatingDeleteHandler(IRatingsRepository ratingsRepository, TradeShopContext context)
    {
        _ratingsRepository = ratingsRepository;
        _context = context;
    }

    public void Handle(in int id)
    {
        if (_ratingsRepository.GetById(id) is not null)
        {
            _ratingsRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("Rating not found");
        }

    }
}