using System.Security.Cryptography;
using System.Text;
using Application.utils;
using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;

public class UserCreateHandler : ICommandsHandler<BasicUserCreateCommand,UserCreateOutput>,ICommandsAdminHandler<UserCreateCommand,UserCreateOutput>
{
    private readonly IUsersRepository _usersRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;
    
    public UserCreateHandler(IUsersRepository usersRepository, IMapper mapper, TradeShopContext context)
    {
        _usersRepository = usersRepository;
        _mapper = mapper;
        _context = context;
    }

    
    
    public UserCreateOutput Handle(BasicUserCreateCommand input) {
        
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "profilePic/uploads");
        Directory.CreateDirectory(uploadsFolder);

        //Generate a uniq name for the image but still use the original extension
        //still need to limit some extension
        var fileName = Guid.NewGuid() + Path.GetExtension(input.ProfilePicture.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        //to be sure the flux will be closed once finished
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            input.ProfilePicture.CopyTo(stream);
        }

        var relativePath = Path.Combine("profilePic/uploads", fileName);
        
        
        var user = new Users
        {
            Username = input.Username,
            Password = HashPassword(input.Password),
            Email = input.Email,
            Role = "connected_user",
            ProfilePicture = relativePath,
            MembershipLevel = input.MembershipLevel,
            Rating = 0,
            Status = "active"
        };
        
        var existingUser = _context.Users
            .FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);

        if (existingUser != null)
        {
            throw new Exception("User with this email or username already exists.");
        }
        
        
        _usersRepository.Create(user);
        _context.SaveChanges();
        return _mapper.Map<UserCreateOutput>(user);
    }
    
    public UserCreateOutput HandleAdmin(UserCreateCommand input) {
        
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "profilePic/uploads");
        Directory.CreateDirectory(uploadsFolder);

        //Generate a uniq name for the image but still use the original extension
        //still need to limit some extension
        var fileName = Guid.NewGuid() + Path.GetExtension(input.ProfilePicture.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        //to be sure the flux will be closed once finished
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            input.ProfilePicture.CopyTo(stream);
        }

        var relativePath = Path.Combine("profilePic/uploads", fileName);
        
        var allowedRoles = new[] { "connected_user", "admin" };
        if (!allowedRoles.Contains(input.Role))
        {
            throw new ArgumentException("Invalid role. Allowed values are 'connected_user' and 'admin'.");
        }
        
        var allowedStatuses = new[] { "active", "suspended", "deleted" };
        if (!allowedStatuses.Contains(input.Status))
        {
            throw new ArgumentException("Invalid status. Allowed values are 'active', 'suspended', and 'deleted'.");
        }
        
        var user = new Users
        {
            Username = input.Username,
            Password = HashPassword(input.Password),
            Email = input.Email,
            Role = string.IsNullOrEmpty(input.Role) ? "connected_user" : input.Role,
            ProfilePicture = relativePath,
            MembershipLevel = input.MembershipLevel,
            Rating = input.Rating,
            Status = string.IsNullOrEmpty(input.Status) ? "active" : input.Status
        };
        
        var existingUser = _context.Users
            .FirstOrDefault(u => u.Username == user.Username || u.Email == user.Email);

        if (existingUser != null)
        {
            throw new Exception("User with this email or username already exists.");
        }
        
        
        _usersRepository.Create(user);
        _context.SaveChanges();
        return _mapper.Map<UserCreateOutput>(user);
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
