using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Commands.update;

public class RatingUpdateHandler: IEmptyOutputCommandHandler<RatingUpdateCommand>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;

    public RatingUpdateHandler(IRatingsRepository ratingsRepository, IMapper mapper, TradeShopContext context)
    {
        _ratingsRepository = ratingsRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in RatingUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _ratingsRepository.GetById(input.RatingId)
                     ?? throw new RatingNotFoundException(input.RatingId);
    
        entity.UserId = input.UserId;
        entity.ReviewerId = input.ReviewerId;
        entity.Score = input.Score;
        entity.Comment = input.Comment;
        
        _ratingsRepository.Update(entity);
        _context.SaveChanges();
        
        transaction.Commit();
    }
}