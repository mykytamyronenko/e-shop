namespace Application.Queries.getById;

public class MembershipsGetByIdOutput
{
    public int MembershipId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public string Description { get; set; }
}