using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Domain;
public class Users
{
    [Key]
    public int UserId { get; set; }

    private string _username;
    public string Username
    {
        get => _username;
        set
        {
            if (string.IsNullOrEmpty(value) || value.Length < 3)
            {
                throw new ArgumentException("The Username must be at least 3 characters long.");
            }

            if (!Regex.IsMatch(value, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException("The username contains invalid characters. chose between letters and numbers and underscores.");
            }

            _username = value;
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Email mustn't be null or empty.");
            }

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(value))
            {
                throw new ArgumentException("Email is not a valid email address.");
            }

            _email = value;
        }
    }
    
    // the hashing is done elsewhere so for the moment no verification possible unless we decide to hash here
    public string Password { get; set; }


    private string _role;
    public string Role
    {
        get => _role;
        set
        {
            if (value != "admin" && value != "connected_user")
            {
                throw new ArgumentException("The role must be  'admin' or 'connected_user'.");
            }

            _role = value;
        }
    }

    public string ProfilePicture { get; set; }

    private string _membershipLevel;
    public string MembershipLevel
    {
        get => _membershipLevel;
        set
        {
            if (value != "Bronze" && value != "Silver" && value != "Gold")
            {
                throw new ArgumentException("The membership must be 'Bronze', 'Silver' or 'Gold'.");
            }

            _membershipLevel = value;
        }
    }

    private float _rating;
    public float Rating
    {
        get => _rating;
        set
        {
            if (value < 0 || value > 5)
            {
                throw new ArgumentException("the rating must be between 0 and 5.");
            }

            _rating = value;
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            if (value != "active" && value != "suspended" && value != "deleted")
            {
                throw new ArgumentException("the status me be 'active' 'suspended' or 'deleted'.");
            }

            _status = value;
        }
    }

    private decimal _balance;
    public decimal Balance
    {
        get => _balance;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("the balance mustn't be negative.");
            }

            _balance = value;
        }
    }
}
