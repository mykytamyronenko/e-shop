using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;

public class MembershipCreateHandler: ICommandsHandler<MembershipCreateCommand,MembershipCreateOutput>
{
    private readonly IMembershipsRepository _membershipsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;


    public MembershipCreateHandler(IMembershipsRepository membershipsRepository, IMapper mapper, TradeShopContext context)
    {
        _membershipsRepository = membershipsRepository;
        _mapper = mapper;
        _context = context;
    }

    public MembershipCreateOutput Handle(MembershipCreateCommand input) {
        var membership = new Memberships
        {
            Name = input.Name,
            Price = input.Price,
            DiscountPercentage = input.DiscountPercentage,
            Description = input.Description
        };
 
        
        var existingMembership = _context.Memberships.FirstOrDefault(a => a.MembershipId == membership.MembershipId);

        if (existingMembership != null)
        {
            throw new Exception("Membership already exists.");
        }
        
        
        _membershipsRepository.Create(membership);
        _context.SaveChanges();
        return _mapper.Map<MembershipCreateOutput>(membership);
    }
}