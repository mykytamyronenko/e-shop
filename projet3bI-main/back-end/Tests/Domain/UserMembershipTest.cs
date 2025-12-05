namespace Tests.Domain;

public class UserMembershipTest
{
    [Fact]
    public void StartDate_ShouldThrowArgumentException_WhenDateIsInTheFuture()
    {
        // Arrange
        var userMembership = new UserMemberships();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => userMembership.StartDate = DateTime.Now.AddDays(1)); // Future date
    }

    [Fact]
    public void EndDate_ShouldThrowArgumentException_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var userMembership = new UserMemberships
        {
            StartDate = DateTime.Now.AddDays(-5)
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => userMembership.EndDate = DateTime.Now.AddDays(-6)); // End date before start date
    }

    [Fact]
    public void Status_ShouldThrowArgumentException_WhenStatusIsInvalid()
    {
        // Arrange
        var userMembership = new UserMemberships();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => userMembership.Status = "inactive"); // Invalid status
        Assert.Throws<ArgumentException>(() => userMembership.Status = "pending");  // Invalid status
    }

    [Fact]
    public void Status_ShouldSetValue_WhenStatusIsValid()
    {
        // Arrange
        var userMembership = new UserMemberships();

        // Act
        userMembership.Status = "active";
        userMembership.Status = "expired";
        userMembership.Status = "cancelled";

        // Assert
        Assert.Equal("active", userMembership.Status);
        Assert.Equal("expired", userMembership.Status);
        Assert.Equal("cancelled", userMembership.Status);
    }
    
    [Fact]
    public void StartDateAndEndDate_ShouldNotThrowException_WhenDatesAreValid()
    {
        // Arrange
        var userMembership = new UserMemberships();

        // Act & Assert
        userMembership.StartDate = DateTime.Now.AddDays(-1); // Valid start date
        userMembership.EndDate = DateTime.Now.AddDays(10);  // Valid end date after start date
    }


}