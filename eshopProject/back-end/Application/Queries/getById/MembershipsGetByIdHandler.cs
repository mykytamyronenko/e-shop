using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class MembershipsGetByIdHandler : IQueryHandler<int,MembershipsGetByIdOutput>
{
    private readonly IMembershipsRepository _membershipsRepository;
    private readonly IMapper _mapper;


    public MembershipsGetByIdHandler(IMembershipsRepository membershipsRepository, IMapper mapper)
    {
        _membershipsRepository = membershipsRepository;
        _mapper = mapper;
    }

    public MembershipsGetByIdOutput Handle(int id)
    {
        var dbMembership = _membershipsRepository.GetById(id) ?? throw new MembershipNotFoundException(id);

        return _mapper.Map<MembershipsGetByIdOutput>(dbMembership);
    }


    
}