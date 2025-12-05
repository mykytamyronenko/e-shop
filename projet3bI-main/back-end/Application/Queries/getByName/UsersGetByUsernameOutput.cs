namespace Application.Queries.getByName;

public class UsersGetByUsernameOutput
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Role { get; set; }
    public string ProfilePicture { get; set; }
    public string MembershipLevel { get; set; }
    public float Rating { get; set; }
    public string Status { get; set; }
}