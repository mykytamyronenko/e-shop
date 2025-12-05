using System.Windows.Input;

namespace Application.Commands.Create;

public class MembershipCreateCommand
{
    public string Name { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public string Description { get; set; }
}