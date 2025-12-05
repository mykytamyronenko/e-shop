
namespace Application.Commands.update;

public class MembershipUpdateCommand
{
    public int MembershipId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public string Description { get; set; }
}