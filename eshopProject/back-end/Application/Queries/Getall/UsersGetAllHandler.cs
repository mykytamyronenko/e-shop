using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.Getall;

public class UsersGetAllHandler : IQueryHandler<UsersGetAllQuery,UsersGetAllOutput>
{
    private readonly IUsersRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersGetAllHandler(IUsersRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }


    public UsersGetAllOutput Handle(UsersGetAllQuery query)
    {
        var dbUsers = _userRepository.GetAll();
        var output = new UsersGetAllOutput()
        {
            UsersList = _mapper.Map<List<UsersGetAllOutput.Users>>(dbUsers)
        };

        return output;
    }
}