using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class UsersMembershipGetAllHandler : IQueryHandler<UsersMembershipGetAllQuery,UsersMembershipGetAllOutput>
{
    private readonly IUserMembershipsRepository _userMembershipsRepository;
    private readonly IMapper _mapper;


    public UsersMembershipGetAllHandler(IUserMembershipsRepository userMembershipsRepository, IMapper mapper)
    {
        _userMembershipsRepository = userMembershipsRepository;
        _mapper = mapper;
    }

    public UsersMembershipGetAllOutput Handle(UsersMembershipGetAllQuery query)
    {
        var dbUserMembership = _userMembershipsRepository.GetAll();
        var output = new UsersMembershipGetAllOutput
        {
            UserMembershipsList = _mapper.Map<List<UsersMembershipGetAllOutput.UserMemberships>>(dbUserMembership)
        };

        return output;
    }
}