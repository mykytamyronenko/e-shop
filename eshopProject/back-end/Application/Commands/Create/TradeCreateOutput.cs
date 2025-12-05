namespace Application.Commands.Create;

public class TradeCreateOutput
{
    public int TraderId { get; set; }
    public int ReceiverId { get; set; }
    public string TraderArticlesIds { get; set; }
    public int ReceiverArticleId { get; set; }
    public DateTime TradeDate { get; set; }
    public string Status { get; set; }
}