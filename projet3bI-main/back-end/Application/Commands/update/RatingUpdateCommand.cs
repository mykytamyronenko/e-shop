namespace Application.Commands.update;

public class RatingUpdateCommand
{
    public int RatingId { get; set; }
    public int UserId { get; set; }
    public int ReviewerId { get; set; }
    public int Score { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}