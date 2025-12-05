using Application.Queries.Getall;
using Application.Queries.getById;
using Application.Queries.getByName;
using Application.utils;

namespace Application.Queries;

public class UsersQueryProcessor
{
    private readonly IQueryHandler<UsersGetAllQuery,UsersGetAllOutput> _userQueryHandler;
    private readonly IQueryHandler<int, UsersGetByIdOutput> _userQueryHandlerId;
    private readonly IQueryHandler<string, UsersGetByUsernameOutput> _userQueryHandlerUsername;


    public UsersQueryProcessor(IQueryHandler<UsersGetAllQuery, UsersGetAllOutput> userQueryHandler, IQueryHandler<int, UsersGetByIdOutput> userQueryHandlerId, IQueryHandler<string, UsersGetByUsernameOutput> userQueryHandlerUsername)
    {
        _userQueryHandler = userQueryHandler;
        _userQueryHandlerId = userQueryHandlerId;
        _userQueryHandlerUsername = userQueryHandlerUsername;
    }

    public UsersGetAllOutput GetAll(UsersGetAllQuery query)
    {
        return _userQueryHandler.Handle(query);
    }
    public UsersGetByIdOutput GetById(int id)
    {
        return _userQueryHandlerId.Handle(id);
    }
    
    public UsersGetByUsernameOutput GetByUsername(string name)
    {
        return _userQueryHandlerUsername.Handle(name);
    }

}