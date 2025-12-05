namespace Application.utils;

public interface IQueryHandler<TQuery, TResult>
{
    TResult Handle(TQuery query);
}
