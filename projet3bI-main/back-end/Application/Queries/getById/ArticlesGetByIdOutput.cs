namespace Application.Queries.getById;

public class ArticlesGetByIdOutput
{
    public int ArticleId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public string State { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Status { get; set; }
    public string MainImageUrl { get; set; }
    public int Quantity { get; set; }
}