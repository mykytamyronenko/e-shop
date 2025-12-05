using Microsoft.AspNetCore.Http;

namespace Application.Commands.Create;

public class UserCreateCommand
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public IFormFile ProfilePicture { get; set; }
    public string MembershipLevel { get; set; }
    public float Rating { get; set; }
    public string Status { get; set; }
}