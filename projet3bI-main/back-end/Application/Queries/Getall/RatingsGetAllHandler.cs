using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class RatingsGetAllHandler : IQueryHandler<RatingsGetAllQuery, RatingsGetAllOutput>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IMapper _mapper;

    public RatingsGetAllHandler(IRatingsRepository ratingsRepository, IMapper mapper)
    {
        _ratingsRepository = ratingsRepository;
        _mapper = mapper;
    }


    public RatingsGetAllOutput Handle(RatingsGetAllQuery query)
    {
        var dbRatings = _ratingsRepository.GetAll();
        var output = new RatingsGetAllOutput()
        {
            RatingsList  = _mapper.Map<List<RatingsGetAllOutput.Ratings>>(dbRatings)
        };

        return output;
    } 
}