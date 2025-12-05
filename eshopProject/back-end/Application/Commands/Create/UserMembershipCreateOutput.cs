namespace Application.Commands.Create;

public class UserMembershipCreateOutput
{
    public int UserId { get; set; }
    public int MembershipId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}