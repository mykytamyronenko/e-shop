using System.Security.Cryptography;
using System.Text;
using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.update;

public class MembershipUpdateHandler : IEmptyOutputCommandHandler<MembershipUpdateCommand>
{
    private readonly IMembershipsRepository _membershipsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;


    public MembershipUpdateHandler(IMembershipsRepository membershipsRepository, IMapper mapper, TradeShopContext context)
    {
        _membershipsRepository = membershipsRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in MembershipUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _membershipsRepository.GetById(input.MembershipId)
                     ?? throw new MembershipNotFoundException(input.MembershipId);
        
        entity.Name = input.Name;
        entity.Price = input.Price;
        entity.DiscountPercentage = input.DiscountPercentage;
        entity.Description = input.Description;
        
        _membershipsRepository.Update(entity);
        _context.SaveChanges();

        transaction.Commit();
    }
    
}