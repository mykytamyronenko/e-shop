namespace Application.Utils;

public interface ICommandsHandler<TQuery, TResult>
{
    TResult Handle(TQuery query);
}