namespace Application.exceptions;

public class RatingNotFoundException: Exception
{
    public RatingNotFoundException(int id) : base($"Rating with id {id} not found."){}
}