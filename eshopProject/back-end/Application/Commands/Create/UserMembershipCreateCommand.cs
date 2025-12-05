using System.Windows.Input;

namespace Application.Commands.Create;

public class UserMembershipCreateCommand
{
    public int UserId { get; set; }
    public int MembershipId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}