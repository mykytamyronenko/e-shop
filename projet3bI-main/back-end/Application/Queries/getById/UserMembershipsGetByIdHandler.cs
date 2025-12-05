using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class UserMembershipsGetByIdHandler : IQueryHandler<int,UserMembershipsGetByIdOutput>
{
    private readonly IUserMembershipsRepository _userMembershipsRepository;
    private readonly IMapper _mapper;
    
    public UserMembershipsGetByIdHandler(IUserMembershipsRepository userMembershipsRepository, IMapper mapper)
    {
        _userMembershipsRepository = userMembershipsRepository;
        _mapper = mapper;
    }

    public UserMembershipsGetByIdOutput Handle(int id)
    {
        var dbUserMembership = _userMembershipsRepository.GetById(id) 
                     ?? throw new UserMembershipNotFoundException(id);

        return _mapper.Map<UserMembershipsGetByIdOutput>(dbUserMembership);
    }


    
}