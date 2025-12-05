using Application.exceptions;
using Application.Utils;
using AutoMapper;
using Domain;
using Infrastructure;

namespace Application.Commands.Create;
public class TransactionCreateHandler : ICommandsHandler<TransactionCreateCommand, TransactionCreateOutput>
{
    private readonly ITransactionsRepository _transactionsRepository;
    private readonly IMapper _mapper;
    private readonly TradeShopContext _context;

    public TransactionCreateHandler(ITransactionsRepository transactionsRepository, IMapper mapper, TradeShopContext context)
    {
        _transactionsRepository = transactionsRepository;
        _mapper = mapper;
        _context = context;
    }

    public TransactionCreateOutput Handle(TransactionCreateCommand input)
    {
        var allowedTransactionTypes = new[] { "purchase", "exchange" };
        if (!allowedTransactionTypes.Contains(input.TransactionType))
        {
            throw new ArgumentException("Invalid transaction type. Allowed values are 'purchase' or 'exchange'.");
        }

        var allowedStatuses = new[] { "in progress", "finished", "cancelled" };
        if (!allowedStatuses.Contains(input.Status))
        {
            throw new ArgumentException("Invalid status. Allowed values are 'in progress', 'finished', and 'cancelled'.");
        }

        using var transactionScope = _context.Database.BeginTransaction();
        
        var article = _context.Articles.FirstOrDefault(a => a.ArticleId == input.ArticleId)
                      ?? throw new ArticleNotFoundException(input.ArticleId);
        
        if (article.Status != "available")
        {
            throw new InvalidOperationException("The article is not available for transaction.");
        }
        
        if (article.Quantity < 1)
        {
            throw new InvalidOperationException("The article is out of stock.");
        }
        
        var seller = _context.Users.FirstOrDefault(u => u.UserId == input.SellerId)
                     ?? throw new UserNotFoundException(input.SellerId);
        
        var buyer = _context.Users.FirstOrDefault(u => u.UserId == input.BuyerId)
                    ?? throw new UserNotFoundException(input.BuyerId);
        
        const decimal fixedCommission = 2m;
        
        var userMembership = _context.UserMemberships
            .FirstOrDefault(um => um.UserId == input.BuyerId);
        
        decimal discountPercentage = 0m;
        
        if (userMembership != null)
        {
            var membership = _context.Memberships.FirstOrDefault(m => m.MembershipId == userMembership.MembershipId);
            if (membership != null)
            {
                discountPercentage = membership.DiscountPercentage;
            }
        }
        
        decimal discountedCommission = fixedCommission * (1 - discountPercentage);
        
        
        decimal totalPrice = input.Price + discountedCommission;
        if (buyer.Balance < totalPrice)
        {
            throw new InvalidOperationException("Buyer does not have enough balance to complete the transaction.");
        }
        
        var transaction = new Transactions
        {
            BuyerId = input.BuyerId,
            SellerId = input.SellerId,
            ArticleId = input.ArticleId,
            TransactionType = input.TransactionType,
            Price = totalPrice,
            Commission = discountedCommission,
            TransactionDate = DateTime.Now,
            Status = string.IsNullOrEmpty(input.Status) ? "finished" : input.Status,
        };
        
        var existingTransaction = _context.Transactions.FirstOrDefault(t => t.TransactionId == transaction.TransactionId);
        if (existingTransaction != null)
        {
            throw new Exception("Transaction already exists.");
        }
        
        buyer.Balance -= totalPrice;
        seller.Balance += input.Price;
        
        article.Quantity -= 1;
        
        article.Status = article.Quantity > 0 ? "available" : "sold";
        
        _context.Articles.Update(article);
        
        _transactionsRepository.Create(transaction);
        _context.Users.Update(buyer);
        _context.Users.Update(seller);
        _context.SaveChanges();
        
        transactionScope.Commit();

        return _mapper.Map<TransactionCreateOutput>(transaction);
    }
}

