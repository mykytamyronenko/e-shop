using System.ComponentModel.DataAnnotations;

namespace Domain;
public class UserMemberships
{
    [Key]
    public int UserMembershipId { get; set; }
    public int UserId { get; set; }
    public int MembershipId { get; set; }

    private DateTime _startDate;
    public DateTime StartDate
    {
        get => _startDate;
        set
        {
            if (value > DateTime.Now)
            {
                throw new ArgumentException("The start date cannot be in the future.");
            }

            _startDate = value;
        }
    }

    private DateTime _endDate;
    public DateTime EndDate
    {
        get => _endDate;
        set
        {
            if (value < StartDate)
            {
                throw new ArgumentException("The end date must be later than the start date.");
            }

            _endDate = value;
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            if (value != "active" && value != "expired" && value != "cancelled")
            {
                throw new ArgumentException("The status must be 'active', 'expired', or 'cancelled'.");
            }

            _status = value;
        }
    }
}
