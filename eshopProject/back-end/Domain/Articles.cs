using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Application.utils;

namespace Domain;
public class Articles
{
    [Key]
    public int ArticleId { get; set; }

    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The title cannot be null or empty.");
            }

            _title = value;
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

    private decimal _price;
    public decimal Price
    {
        get => _price;
        set
        {
            if (value <= 1)
            {
                throw new ArgumentException("The price must be greater than 1.");
            }

            _price = value;
        }
    }

    private ArticleCategory _category;
    [Column(TypeName = "nvarchar(50)")]

    public ArticleCategory Category
    {
        get => _category;
        set
        {
            if (string.IsNullOrWhiteSpace(value.ToString()))
            {
                throw new ArgumentException("The category cannot be null or empty.");
            }

            _category = value;
        }
    }

    private string _state;
    public string State
    {
        get => _state;
        set
        {
            if (value != "new" && value != "used")
            {
                throw new ArgumentException("The state must be 'new' or 'used'.");
            }

            _state = value;
        }
    }

    public int UserId { get; set; }

    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set
        {
            if (value > DateTime.Now)
            {
                throw new ArgumentException("The creation date cannot be in the future.");
            }

            _createdAt = value;
        }
    }

    private DateTime _updatedAt;
    public DateTime UpdatedAt
    {
        get => _updatedAt;
        set
        {
            if (value < CreatedAt)
            {
                throw new ArgumentException("The update date cannot be earlier than the creation date.");
            }

            _updatedAt = value;
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            if (value != "available" && value != "sold" && value != "removed")
            {
                throw new ArgumentException("The status must be 'available', 'sold' or 'removed'.");
            }

            _status = value;
        }
    }

    private string _mainImageUrl;
    public string MainImageUrl
    {
        get => _mainImageUrl;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The main image URL cannot be null or empty.");
            }

            _mainImageUrl = value;
        }
    }

    private int _quantity;
    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("The quantity cannot be less than 0.");
            }

            _quantity = value;
        }
    }
}
