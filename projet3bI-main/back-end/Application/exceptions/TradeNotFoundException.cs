namespace Application.exceptions;

public class TradeNotFoundException: Exception
{
    public TradeNotFoundException(int id) : base($"Trade with id: {id} was not found"){}
}