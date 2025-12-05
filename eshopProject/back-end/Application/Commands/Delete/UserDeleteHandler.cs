using Application.utils;
using Infrastructure;

namespace Application.Commands.Delete;

public class UserDeleteHandler : IUserDeleteCommandHandler
{
    private readonly IUsersRepository _usersRepository;
    private readonly TradeShopContext _context;

    public UserDeleteHandler(IUsersRepository usersRepository, TradeShopContext context)
    {
        _usersRepository = usersRepository;
        _context = context;
    }


    public void Handle(in int id)
    {
        if (_usersRepository.GetById(id) is not null)
        {
            _usersRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new InvalidOperationException("User not found");
        }
    }

}