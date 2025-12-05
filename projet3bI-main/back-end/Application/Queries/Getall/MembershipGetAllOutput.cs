using Domain;

namespace Application.Queries.Getall;

public class MembershipGetAllOutput
{
    public List<Memberships> MembershipList { get; set; } = new List<Memberships>();

    public class Memberships
    {
        public int MembershipId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string Description { get; set; }
        
    }
}