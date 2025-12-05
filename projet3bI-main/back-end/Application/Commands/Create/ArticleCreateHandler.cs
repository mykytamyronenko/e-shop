using Application.utils;
using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;

public class ArticleCreateHandler: ICommandsHandler<ArticleCreateCommand,ArticleCreateOutput>
{
    private readonly IArticlesRepository _articlesRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;

    public ArticleCreateHandler(IArticlesRepository articlesRepository, IMapper mapper, TradeShopContext context)
    {
        _articlesRepository = articlesRepository;
        _mapper = mapper;
        _context = context;
    }


    public ArticleCreateOutput Handle(ArticleCreateCommand input) {
        var allowedState = new[] { "new", "used"};
        if (!allowedState.Contains(input.State))
        {
            throw new ArgumentException("Invalid state. Allowed values are 'new' and 'used'.");
        }
        var allowedStatuses = new[] { "available", "sold", "removed" };
        if (!allowedStatuses.Contains(input.Status))
        {
            throw new ArgumentException("Invalid status. Allowed values are 'available', 'sold', and 'removed'.");
        }
        
        if (input.Image == null || input.Image.Length == 0)
        {
            throw new ArgumentException("Image is required");
        }
        
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "artImages/uploads");
        Directory.CreateDirectory(uploadsFolder);

        //Generate a uniq name for the image but still use the original extension
        //still need to limit some extension
        var fileName = Guid.NewGuid() + Path.GetExtension(input.Image.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        //to be sure the flux will be closed once finished
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            input.Image.CopyTo(stream);
        }

        var relativePath = Path.Combine("artImages/uploads", fileName);

        if (!Enum.TryParse<ArticleCategory>(input.Category, true, out var category))
        {
            throw new ArgumentException($"Invalid category. Allowed categories are: {string.Join(", ", Enum.GetNames(typeof(ArticleCategory)))}");
        }
        
        var article = new Articles
        {
            Title = input.Title,
            Description = input.Description,
            Price = input.Price,
            Category = category,
            State = string.IsNullOrEmpty(input.State) ? "used" : input.State,
            UserId = input.UserId,
            CreatedAt = DateTime.UtcNow.Date,
            UpdatedAt = DateTime.UtcNow,
            Status = string.IsNullOrEmpty(input.Status) ? "available" : input.Status,
            MainImageUrl = relativePath,
            Quantity = input.Quantity
        };
 
        
        var existingArticle = _context.Articles
            .FirstOrDefault(a => a.ArticleId == article.ArticleId);

        if (existingArticle != null)
        {
            throw new Exception("Article already exists.");
        }
        
        
        _articlesRepository.Create(article);
        _context.SaveChanges();
        return _mapper.Map<ArticleCreateOutput>(article);
    }
}