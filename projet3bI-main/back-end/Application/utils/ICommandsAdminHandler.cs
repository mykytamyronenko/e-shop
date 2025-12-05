namespace Application.utils;

public interface ICommandsAdminHandler<TQuery, TResult>
{
    TResult HandleAdmin(TQuery query);

}