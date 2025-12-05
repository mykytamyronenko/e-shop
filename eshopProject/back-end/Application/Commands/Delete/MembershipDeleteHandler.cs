using Application.utils;
using Infrastructure;

namespace Application.Commands.Delete;

public class MembershipDeleteHandler : IMembershipDeleteCommandHandler
{
    private readonly IMembershipsRepository _membershipsRepository;
    private readonly TradeShopContext _context;

    public MembershipDeleteHandler(IMembershipsRepository membershipsRepository, TradeShopContext context)
    {
        _membershipsRepository = membershipsRepository;
        _context = context;
    }


    public void Handle(in int id)
    {
        if (_membershipsRepository.GetById(id) is not null)
        {
            _membershipsRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new InvalidOperationException("Membership not found");
        }
    }

}