using Application.exceptions;
using Application.Queries.getById;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getByName;

public class UsersGetByUsernameHandler : IQueryHandler<string,UsersGetByUsernameOutput>
{
    private readonly IUsersRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersGetByUsernameHandler(IUsersRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public UsersGetByUsernameOutput Handle(string name)
    {
        var dbUser = _userRepository.GetUserByUsername(name) ?? throw new UserNotFoundException(name);

        return _mapper.Map<UsersGetByUsernameOutput>(dbUser);
    }


    
}