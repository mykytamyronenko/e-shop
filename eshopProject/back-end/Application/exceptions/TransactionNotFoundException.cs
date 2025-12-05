namespace Application.exceptions;

public class TransactionNotFoundException: Exception
{
    public TransactionNotFoundException(int id) : base($"Transaction with id {id} not found."){}
}