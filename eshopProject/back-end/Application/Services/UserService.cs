using System.Security.Cryptography;
using System.Text;
using Application.Dtos;
using AutJwt;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class UserService
{
    private readonly IUsersRepository _userRepository;
    private readonly JwtService _jwtService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;

    public UserService(IUsersRepository userRepository, JwtService jwtService, IConfiguration configuration, ILogger<UserService> logger)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _configuration = configuration;
        _logger = logger;
    }
    
    public string Login(DtoInputUser dtoUsers)
    {
        var user = _userRepository.GetUserByUsername(dtoUsers.Username) ?? _userRepository.GetUserByEmail(dtoUsers.Username);
        if (user == null)
        {
            _logger.LogInformation("No user found with this email or username : {EmailOrUsername}", dtoUsers.Username);
            return null; // User does not exist
        }
        
        if (user.Status == "deleted")
        {
            _logger.LogInformation("You account has been deleted");
            return "AccountDeleted";
        }
        
        _logger.LogInformation("pass dto : {pass}", dtoUsers.Password);
        _logger.LogInformation("username dto : {username}", dtoUsers.Username);
        
        _logger.LogInformation("pass user : {username}", user.Password);
        _logger.LogInformation("username user : {username}", user.Username);

        _logger.LogInformation("pass dto : {username}", HashPassword(dtoUsers.Password));

        
        if (user == null || user.Password != HashPassword(dtoUsers.Password))
        {
            return null;
        }
        
        
        _logger.LogInformation("User found : {EmailOrUsername}", dtoUsers.Username);
        var dtoUser = new DtoServerInput()
        {
            Username = user.Username,
            UserId = user.UserId,
            Role = user.Role
        };

        // If user is valid, generate JWT token
        var token = _jwtService.BuildToken(
            _configuration["Jwt:Key"],
            _configuration["Jwt:Issuer"],
            dtoUser
        );        
        return token;
    }
    
    private string HashPassword(string password)
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
