namespace Application.Commands.update;

public class TradeUpdateCommand
{
    public int TradeId { get; set; }
    public int TraderId { get; set; }
    public int ReceiverId { get; set; }
    public string TraderArticlesIds { get; set; }
    public int ReceiverArticleId { get; set; }
    public DateTime TradeDate { get; set; }
    public string Status { get; set; }
}