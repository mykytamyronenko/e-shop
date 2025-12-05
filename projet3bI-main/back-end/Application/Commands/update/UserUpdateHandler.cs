using System.Security.Cryptography;
using System.Text;
using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Commands.update;

public class UserUpdateHandler : IEmptyOutputCommandHandler<UserUpdateCommand>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;


    public UserUpdateHandler(IUsersRepository usersRepository, IMapper mapper, TradeShopContext context)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in UserUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _usersRepository.GetById(input.UserId)
                     ?? throw new UserNotFoundException(input.UserId);
            
        entity.Username = input.Username;
        if (!string.IsNullOrEmpty(input.Password))
        {
            entity.Password = entity.Password;
        }
        entity.Email = input.Email;
        entity.ProfilePicture = input.ProfilePicture;
        entity.MembershipLevel = input.MembershipLevel;
        entity.Rating = input.Rating;
        entity.Status = input.Status;
        entity.Balance = input.Balance;
        
        _usersRepository.Update(entity);
        _context.SaveChanges();

        transaction.Commit();
    }
    
    public string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            
            StringBuilder hashString = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                hashString.Append(b.ToString("x2"));
            }
        
            return hashString.ToString();
        }
    }
}