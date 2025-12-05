using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class RatingsGetByIdHandler: IQueryHandler<int, RatingsGetByIdOutput>
{
    private readonly IRatingsRepository _ratingsRepository;
    private readonly IMapper _mapper;


    public RatingsGetByIdHandler(IRatingsRepository ratingsRepository, IMapper mapper)
    {
        _ratingsRepository = ratingsRepository;
        _mapper = mapper;
    }

    public RatingsGetByIdOutput Handle(int id)
    {
        var dbRating = _ratingsRepository.GetById(id) ?? throw new RatingNotFoundException(id);

        return _mapper.Map<RatingsGetByIdOutput>(dbRating);
    }
}