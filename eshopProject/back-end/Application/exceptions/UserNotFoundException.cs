namespace Application.exceptions;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(int id) 
        : base($"User with ID {id} was not found.")
    {
    }
    
    public UserNotFoundException(string name) 
        : base($"User with the name : {name} was not found.")
    {
    }
    
}