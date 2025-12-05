using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class MembershipGetAllHandler : IQueryHandler<MembershipGetAllQuery,MembershipGetAllOutput>
{
    private readonly IMembershipsRepository _membershipsRepository;
    private readonly IMapper _mapper;

    public MembershipGetAllHandler(IMembershipsRepository membershipsRepository, IMapper mapper)
    {
        _membershipsRepository = membershipsRepository;
        _mapper = mapper;
    }


    public MembershipGetAllOutput Handle(MembershipGetAllQuery query)
    {
        var dbMemberships = _membershipsRepository.GetAll();
        var output = new MembershipGetAllOutput()
        {
            MembershipList = _mapper.Map<List<MembershipGetAllOutput.Memberships>>(dbMemberships)
        };

        return output;
    }
}