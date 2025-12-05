namespace Application.Commands.Create;

public class TransactionCreateCommand
{
    public int BuyerId { get; set; }
    public int SellerId { get; set; }
    public int ArticleId { get; set; }
    public string TransactionType { get; set; }
    public decimal Price { get; set; }
    public decimal Commission { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Status { get; set; }

}