using System.ComponentModel.DataAnnotations;

namespace Domain;
public class Memberships
{
    [Key]
    public int MembershipId { get; set; }

    private string _name;
    public string Name
    {
        get => _name;
        set
        {
            if (value != "Bronze" && value != "Silver" && value != "Gold")
            {
                throw new ArgumentException("The membership name must be 'Bronze', 'Silver' or 'Gold'.");
            }

            _name = value;
        }
    }

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value < 1)
            {
                throw new ArgumentException("The price cannot be less than 1.");
            }

            _price = value;
        }
    }

    private decimal _discountPercentage;
    public decimal DiscountPercentage
    {
        get => _discountPercentage;
        set
        {
            if (value < 0 || value > 1)
            {
                throw new ArgumentException("The discount percentage must be between 0 and 1 (e.g., 0.25 for 25%).");
            }

            _discountPercentage = value;
        }
    }


    private string _description;
    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The description cannot be null or empty.");
            }

            _description = value;
        }
    }
}
