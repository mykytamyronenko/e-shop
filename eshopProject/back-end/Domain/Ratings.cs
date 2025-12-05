using System.ComponentModel.DataAnnotations;

namespace Domain;

public class Ratings
{
    [Key]
    public int RatingId { get; set; }
    public int UserId { get; set; }
    private int _reviewerId;
    public int ReviewerId
    {
        get => _reviewerId;
        set
        {
            if (value == UserId)
            {
                throw new ArgumentException("An user cannot rate themselves.");
            }

            _reviewerId = value;
        }
    }

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            if (value < 1 || value > 5)
            {
                throw new ArgumentException("The score must be between 1 and 5.");
            }

            _score = value;
        }
    }

    private string _comment;
    public string Comment
    {
        get => _comment;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("The comment cannot be null or empty.");
            }

            if (value.Length > 500)
            {
                throw new ArgumentException("The comment cannot exceed 500 characters.");
            }

            _comment = value;
        }
    }

    private DateTime _createdAt;
    public DateTime CreatedAt
    {
        get => _createdAt;
        set
        {
            if (value > DateTime.Now)
            {
                throw new ArgumentException("The creation date cannot be in the future.");
            }

            _createdAt = value;
        }
    }
}
