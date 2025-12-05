using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Transactions
{
    [Key]
    public int TransactionId { get; set; }

    private int _buyerId;
    public int BuyerId
    {
        get => _buyerId;
        set
        {
            if (value == SellerId)
            {
                throw new ArgumentException("BuyerId cannot be the same as SellerId.");
            }
            _buyerId = value;
        }
    }

    private int _sellerId;
    public int SellerId
    {
        get => _sellerId;
        set
        {
            if (value == BuyerId)
            {
                throw new ArgumentException("SellerId cannot be the same as BuyerId.");
            }
            _sellerId = value;
        }
    }

    public int ArticleId { get; set; }

    private string _transactionType;
    public string TransactionType
    {
        get => _transactionType;
        set
        {
            if (value != "purchase" && value != "exchange")
            {
                throw new ArgumentException("The transaction type must be 'purchase' or 'exchange'.");
            }

            _transactionType = value;
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

    private decimal _commission;
    public decimal Commission
    {
        get => _commission;
        set
        {
            if (value < 0)
            {
                throw new ArgumentException("The commission cannot be negative or greater than the price.");
            }

            _commission = value;
        }
    }

    private DateTime _transactionDate;
    public DateTime TransactionDate
    {
        get => _transactionDate;
        set
        {
            if (value > DateTime.Now)
            {
                throw new ArgumentException("The transaction date cannot be in the future.");
            }

            _transactionDate = value;
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            if (value != "in progress" && value != "finished" && value != "cancelled")
            {
                throw new ArgumentException("The status must be 'in progress', 'finished', or 'cancelled'.");
            }

            _status = value;
        }
    }
}

