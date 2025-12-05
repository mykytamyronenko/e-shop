namespace Application.Commands.Create;

public class RatingCreateOutput
{
    public int UserId { get; set; }
    public int ReviewerId { get; set; }
    public int Score { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}