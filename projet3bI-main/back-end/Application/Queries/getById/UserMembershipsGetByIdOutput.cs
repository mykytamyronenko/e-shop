namespace Application.Queries.getById;

public class UserMembershipsGetByIdOutput
{
    public int UserMembershipId { get; set; }
    public int UserId { get; set; }
    public int MembershipId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Status { get; set; }
}