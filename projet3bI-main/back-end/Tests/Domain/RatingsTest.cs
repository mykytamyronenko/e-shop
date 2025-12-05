namespace Tests.Domain;

public class RatingsTest
{
    [Fact]
    public void ReviewerId_ShouldThrowArgumentException_WhenReviewerIsSameAsUser()
    {
        // Arrange
        var rating = new Ratings
        {
            UserId = 1
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => rating.ReviewerId = 1); // Same as UserId
    }

    [Fact]
    public void Score_ShouldThrowArgumentException_WhenScoreIsOutOfRange()
    {
        // Arrange
        var rating = new Ratings();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => rating.Score = 0); // Score < 1
        Assert.Throws<ArgumentException>(() => rating.Score = 6); // Score > 5
    }

    [Fact]
    public void Score_ShouldSetValue_WhenScoreIsBetween1And5()
    {
        // Arrange
        var rating = new Ratings();

        // Act
        rating.Score = 1; // Minimum valid score
        rating.Score = 3; // Valid score in range
        rating.Score = 5; // Maximum valid score

        // Assert
        Assert.Equal(1, rating.Score);
        Assert.Equal(3, rating.Score);
        Assert.Equal(5, rating.Score);
    }

    [Fact]
    public void Comment_ShouldThrowArgumentException_WhenCommentIsNullOrEmpty()
    {
        // Arrange
        var rating = new Ratings();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => rating.Comment = null);
        Assert.Throws<ArgumentException>(() => rating.Comment = string.Empty);
        Assert.Throws<ArgumentException>(() => rating.Comment = " "); // Whitespace
    }

    [Fact]
    public void Comment_ShouldThrowArgumentException_WhenCommentExceeds500Characters()
    {
        // Arrange
        var rating = new Ratings();
        var longComment = new string('A', 501); // Comment with more than 500 characters

        // Act & Assert
        Assert.Throws<ArgumentException>(() => rating.Comment = longComment);
    }

    [Fact]
    public void Comment_ShouldSetValue_WhenValidComment()
    {
        // Arrange
        var rating = new Ratings();
        var validComment = "This is a valid comment.";

        // Act
        rating.Comment = validComment;

        // Assert
        Assert.Equal(validComment, rating.Comment);
    }
    [Fact]
    public void CreatedAt_ShouldThrowArgumentException_WhenDateIsInTheFuture()
    {
        // Arrange
        var rating = new Ratings();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => rating.CreatedAt = DateTime.Now.AddMinutes(1)); // Future date
    }

    [Fact]
    public void CreatedAt_ShouldSetValue_WhenValidDate()
    {
        // Arrange
        var rating = new Ratings();
        var validDate = DateTime.Now.AddMinutes(-1); // Valid past date

        // Act
        rating.CreatedAt = validDate;

        // Assert
        Assert.Equal(validDate, rating.CreatedAt);
    }


}