namespace Application.Queries.Getall;

public class TradesGetAllOutput
{
    public List<Trades> TradesList { get; set; } = new List<Trades>();

    public class Trades
    {
        public int TradeId { get; set; }
        public int TraderId { get; set; }
        public int ReceiverId { get; set; }
        public string TraderArticlesIds { get; set; }
        public int ReceiverArticleId { get; set; }
        public DateTime TradeDate { get; set; }
        public string Status { get; set; }
    }
}


