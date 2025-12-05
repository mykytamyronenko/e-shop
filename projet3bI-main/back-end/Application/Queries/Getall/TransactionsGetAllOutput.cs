using Domain;

namespace Application.Queries.Getall;

public class TransactionsGetAllOutput
{
    public List<Transactions> TransactionsList { get; set; } = new List<Transactions>();

    public class Transactions
    {
        public int TransactionId { get; set; }
        public int BuyerId { get; set; }
        public int SellerId { get; set; }
        public int ArticleId { get; set; }
        public string TransactionType { get; set; }
        public decimal Price { get; set; }
        public decimal Commission { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Status { get; set; }
    }
}