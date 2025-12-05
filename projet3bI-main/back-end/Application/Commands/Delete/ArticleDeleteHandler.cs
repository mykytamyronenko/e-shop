using Application.utils;
using Infrastructure;

namespace Application.Commands.Delete;

public class ArticleDeleteHandler: IArticleDeleteCommandHandler
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly TradeShopContext _context;


    public ArticleDeleteHandler(IArticlesRepository articlesRepository, TradeShopContext context)
    {
        _articlesRepository = articlesRepository;
        _context = context;
    }

    public void Handle(in int id)
    {
        if (_articlesRepository.GetById(id) is not null)
        {
            _articlesRepository.Delete(id);
            _context.SaveChanges();
        }
        else
        {
            throw new Exception("Article not found");
        }

    }
}