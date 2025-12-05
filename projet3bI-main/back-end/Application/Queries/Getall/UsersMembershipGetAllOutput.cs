using Domain;

namespace Application.Queries.Getall;

public class UsersMembershipGetAllOutput
{
    public List<UserMemberships> UserMembershipsList { get; set; } = new List<UserMemberships>();

    public class UserMemberships
    {
        public int UserMembershipId { get; set; }
        public int UserId { get; set; }
        public int MembershipId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        
    }
}