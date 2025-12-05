namespace Application.exceptions;

public class UserMembershipNotFoundException : Exception
{
    public UserMembershipNotFoundException(int id) 
        : base($"User Membership with ID {id} was not found.")
    {
    }
    
}