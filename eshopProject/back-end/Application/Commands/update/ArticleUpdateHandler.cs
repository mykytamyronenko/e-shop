using Application.Commands.Create;
using Application.exceptions;
using Application.utils;
using AutoMapper;
using Infrastructure;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Commands.update;

public class ArticleUpdateHandler: IEmptyOutputCommandHandler<ArticleUpdateCommand>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;

    public ArticleUpdateHandler(IArticlesRepository articlesRepository, IMapper mapper, TradeShopContext context)
    {
        _articlesRepository = articlesRepository;
        _mapper = mapper;
        _context = context;
    }

    public void Handle(in ArticleUpdateCommand input)
    {
        using var transaction = _context.Database.BeginTransaction();
        var entity = _articlesRepository.GetById(input.ArticleId)
                     ?? throw new ArticleNotFoundException(input.ArticleId);
        
        if (!Enum.TryParse<ArticleCategory>(input.Category, true, out var category))
        {
            throw new ArgumentException($"Invalid category. Allowed categories are: {string.Join(", ", Enum.GetNames(typeof(ArticleCategory)))}");
        }
    
        entity.Title = input.Title;
        entity.Description = input.Description;
        entity.Price = input.Price;
        entity.Category = category;
        entity.State = input.State;
        entity.UserId = input.UserId;
        entity.UpdatedAt = DateTime.Now;
        entity.Status = input.Status;
        entity.MainImageUrl = input.MainImageUrl;
        entity.Quantity = input.Quantity;
        
        _articlesRepository.Update(entity);
        _context.SaveChanges();
        
        transaction.Commit();
    }
}