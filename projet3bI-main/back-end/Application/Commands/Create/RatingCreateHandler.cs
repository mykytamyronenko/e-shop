using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;

public class RatingCreateHandler: ICommandsHandler<RatingCreateCommand, RatingCreateOutput>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;
    
    public RatingCreateHandler(IRatingsRepository ratingsRepository, IMapper mapper, TradeShopContext context)
    {
        _ratingsRepository = ratingsRepository;
        _mapper = mapper;
        _context = context;
    }

    public RatingCreateOutput Handle(RatingCreateCommand input) {
        if (input.Score < 1 || input.Score > 5)
        {
            throw new ArgumentOutOfRangeException(nameof(input.Score), "Score must be between 1 and 5.");
        }
        
        var rating = new Ratings
        {
            UserId = input.UserId,
            ReviewerId = input.ReviewerId,
            Score = input.Score,
            Comment = input.Comment,
            CreatedAt = input.CreatedAt
        };
 
        //can't let the reviewer rate the same user
        var existingRating = _context.Ratings.FirstOrDefault(t => t.RatingId == rating.RatingId ||
                                                                  (t.ReviewerId == rating.ReviewerId && t.UserId == rating.UserId));

        if (existingRating != null)
        {
            throw new Exception("This rating already exists.");
        }
        
        
        _ratingsRepository.Create(rating);
        _context.SaveChanges();
        return _mapper.Map<RatingCreateOutput>(rating);
    }
}