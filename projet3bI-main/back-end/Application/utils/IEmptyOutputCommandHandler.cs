namespace Application.utils;

public interface IEmptyOutputCommandHandler<TIdUser>
{
    void Handle(in TIdUser id);
}