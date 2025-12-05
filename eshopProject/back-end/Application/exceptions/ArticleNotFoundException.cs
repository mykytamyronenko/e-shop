namespace Application.exceptions;

public class ArticleNotFoundException : Exception
{
    public ArticleNotFoundException(int id) 
        : base($"Article with ID {id} was not found.")
    {
    }
    
}