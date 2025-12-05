namespace Application.Queries.Getall;

public class UsersGetAllOutput
{
    public List<Users> UsersList { get; set; } = new List<Users>();

    public class Users
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public string ProfilePicture { get; set; }
        public string MembershipLevel { get; set; }
        public float Rating { get; set; }
        public string Status { get; set; }
        public decimal Balance { get; set; }
        
    }
}