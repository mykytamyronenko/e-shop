using System.Windows.Input;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Create;

public class BasicUserCreateCommand
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public IFormFile ProfilePicture { get; set; }
    public string MembershipLevel { get; set; }
}