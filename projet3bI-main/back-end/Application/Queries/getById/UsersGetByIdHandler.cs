using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;

namespace Application.Queries.getById;

public class UsersGetByIdHandler : IQueryHandler<int,UsersGetByIdOutput>
{
    private readonly IUsersRepository _userRepository;
    private readonly IMapper _mapper;

    public UsersGetByIdHandler(IUsersRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }
    
    public UsersGetByIdOutput Handle(int id)
    {
        var dbUser = _userRepository.GetById(id) 
                     ?? throw new UserNotFoundException(id);

        return _mapper.Map<UsersGetByIdOutput>(dbUser);
    }


    
}