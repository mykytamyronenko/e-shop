using Domain;

namespace Application.Queries.Getall;

public class RatingsGetAllOutput
{
    public List<Ratings> RatingsList { get; set; } = new List<Ratings>();

    public class Ratings
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }
        public int ReviewerId { get; set; }
        public int Score { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}