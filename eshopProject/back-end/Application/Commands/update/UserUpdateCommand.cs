namespace Application.Commands.update;

public class UserUpdateCommand
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ProfilePicture { get; set; }
    public string MembershipLevel { get; set; }
    public float Rating { get; set; }
    public string Status { get; set; }
    public decimal Balance { get; set; }
}