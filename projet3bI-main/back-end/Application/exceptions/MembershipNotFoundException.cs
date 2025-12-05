namespace Application.exceptions;

public class MembershipNotFoundException : Exception
{
    public MembershipNotFoundException(int id) 
        : base($"Membership with ID {id} was not found.")
    {
    }
    
}