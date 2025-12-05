using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Trades
{
    [Key]
    public int TradeId { get; set; }

    private int _traderId;
    public int TraderId
    {
        get => _traderId;
        set
        {
            if (value == ReceiverId)
            {
                throw new ArgumentException("The buyer and the seller cannot be the same.");
            }
            _traderId = value;
        }
    }

    private int _receiverId;
    public int ReceiverId
    {
        get => _receiverId;
        set
        {
            if (value == TraderId)
            {
                throw new ArgumentException("The buyer and seller cannot be the same.");
            }
            _receiverId = value;
        }
    }
    
    private string _traderArticlesIds;
    public string TraderArticlesIds
    {
        get => _traderArticlesIds;
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var ids = value.Split(',').Select(id => int.TryParse(id, out var parsedId) ? parsedId : (int?)null).ToList();
                if (ids.Contains(null))
                {
                    throw new ArgumentException("All trader article IDs must be valid integers.");
                }
            }

            _traderArticlesIds = value;
        }
    }
    
    
    private int _receiverArticleId;
    public int ReceiverArticleId
    {
        get => _receiverArticleId;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Receiver article ID must be a positive integer.");
            }
            
            _receiverArticleId = value;
        }
    }


    private DateTime _tradeDate;
    public DateTime TradeDate
    {
        get => _tradeDate;
        set
        {
            if (value > DateTime.Now)
            {
                throw new ArgumentException("The trade date cannot be in the future.");
            }
            _tradeDate = value;
        }
    }

    private string _status;
    public string Status
    {
        get => _status;
        set
        {
            if (value != "in progress" && value != "accepted" && value != "denied")
            {
                throw new ArgumentException("The status must be 'in progress', 'accepted', or 'denied'.");
            }
            _status = value;
        }
    }
}


