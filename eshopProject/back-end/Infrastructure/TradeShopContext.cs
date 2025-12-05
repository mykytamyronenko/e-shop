using Application.utils;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class TradeShopContext : DbContext
{
    public DbSet<Users> Users { get; set; }
    public DbSet<UserMemberships> UserMemberships { get; set; }
    public DbSet<Transactions> Transactions { get; set; }
    public DbSet<Trades> Trades { get; set; }
    public DbSet<Ratings> Ratings { get; set; }
    public DbSet<Memberships> Memberships { get; set; }
    public DbSet<Articles> Articles { get; set; }
    
    public TradeShopContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Users>(builder =>
        {
            builder.ToTable("users");
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.UserId).IsRequired().HasColumnName("user_id");
            builder.Property(x => x.Username).IsRequired().HasColumnName("username");
            builder.Property(x => x.Email).IsRequired().HasColumnName("email");
            builder.Property(x => x.Password).IsRequired().HasColumnName("password");
            builder.Property(x => x.Role).IsRequired().HasColumnName("role");
            builder.Property(x => x.ProfilePicture).IsRequired().HasColumnName("profile_picture");
            builder.Property(x => x.MembershipLevel).IsRequired().HasColumnName("membership_level");
            builder.Property(u => u.Rating).IsRequired().HasColumnType("double precision").HasColumnName("rating");
            builder.Property(x => x.Status).IsRequired().HasColumnName("status");
            builder.Property(x => x.Balance).IsRequired().HasColumnType("decimal(10,2)").HasColumnName("balance");
        });
        
        modelBuilder.Entity<UserMemberships>(builder =>
        {
            builder.ToTable("user_memberships");
            builder.HasKey(x => x.UserMembershipId);
            builder.Property(x => x.UserMembershipId).IsRequired().HasColumnName("user_membership_id");
            builder.Property(x => x.UserId).IsRequired().HasColumnName("user_id");
            builder.Property(x => x.MembershipId).IsRequired().HasColumnName("membership_id");
            builder.Property(x => x.StartDate).IsRequired().HasColumnName("start_date");
            builder.Property(x => x.EndDate).IsRequired().HasColumnName("end_date");
            builder.Property(x => x.Status).IsRequired().HasColumnName("status");
        });
        
        modelBuilder.Entity<Transactions>(builder =>
        {
            builder.ToTable("transactions");
            builder.HasKey(x => x.TransactionId);
            builder.Property(x => x.TransactionId).IsRequired().HasColumnName("transaction_id");
            builder.Property(x => x.BuyerId).IsRequired().HasColumnName("buyer_id");
            builder.Property(x => x.SellerId).IsRequired().HasColumnName("seller_id");
            builder.Property(x => x.ArticleId).IsRequired().HasColumnName("article_id");
            builder.Property(x => x.TransactionType).IsRequired().HasColumnName("transaction_type");
            builder.Property(x => x.Price).IsRequired().HasColumnName("price");
            builder.Property(x => x.Commission).IsRequired().HasColumnName("commission");
            builder.Property(x => x.TransactionDate).IsRequired().HasColumnName("transaction_date");
            builder.Property(x => x.Status).IsRequired().HasColumnName("status");

        });

        modelBuilder.Entity<Trades>(builder =>
        {
            builder.ToTable("trades");
            builder.HasKey(x => x.TradeId);
            builder.Property(x => x.TradeId).IsRequired().HasColumnName("trade_id");
            builder.Property(x => x.TraderId).IsRequired().HasColumnName("trader_id");
            builder.Property(x => x.ReceiverId).IsRequired().HasColumnName("receiver_id");
            builder.Property(x => x.TraderArticlesIds).IsRequired().HasColumnName("trader_articles_ids");
            builder.Property(x => x.ReceiverArticleId).IsRequired().HasColumnName("receiver_article_id");
            builder.Property(x => x.TradeDate).IsRequired().HasColumnName("trade_date");
            builder.Property(x => x.Status).IsRequired().HasColumnName("status");
        });
        
        modelBuilder.Entity<Ratings>(builder =>
        {
            builder.ToTable("ratings");
            builder.HasKey(x => x.RatingId);
            builder.Property(x => x.RatingId).IsRequired().HasColumnName("rating_id");
            builder.Property(x => x.UserId).IsRequired().HasColumnName("user_id");
            builder.Property(x => x.ReviewerId).IsRequired().HasColumnName("reviewer_id");
            builder.Property(x => x.Score).IsRequired().HasColumnName("score");
            builder.Property(x => x.Comment).IsRequired().HasColumnName("comment");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
        });

        modelBuilder.Entity<Memberships>(builder =>
        {
            builder.ToTable("memberships");
            builder.HasKey(x => x.MembershipId);
            builder.Property(x => x.MembershipId).IsRequired().HasColumnName("membership_id");
            builder.Property(x => x.Name).IsRequired().HasColumnName("name");
            builder.Property(x => x.Price).IsRequired().HasColumnName("price");
            builder.Property(x => x.DiscountPercentage).IsRequired().HasColumnName("discount_percentage");
            builder.Property(x => x.Description).IsRequired().HasColumnName("description");
        });
        
        modelBuilder.Entity<Articles>(builder =>
        {
            builder.ToTable("articles");
            builder.HasKey(x => x.ArticleId);
            builder.Property(x => x.ArticleId).IsRequired().HasColumnName("article_id");
            builder.Property(x => x.Title).IsRequired().HasColumnName("title");
            builder.Property(x => x.Description).IsRequired().HasColumnName("description");
            builder.Property(x => x.Price).IsRequired().HasColumnName("price");
            builder.Property(x => x.Category)
                .IsRequired()
                .HasColumnName("category")
                .HasConversion(
                    v => v.ToString(),
                    v => ConvertToCategory(v)
                );
            builder.Property(x => x.State).IsRequired().HasColumnName("state");
            builder.Property(x => x.UserId).IsRequired().HasColumnName("user_id");
            builder.Property(x => x.CreatedAt).IsRequired().HasColumnName("created_at");
            builder.Property(x => x.UpdatedAt).IsRequired().HasColumnName("updated_at");
            builder.Property(x => x.Status).IsRequired().HasColumnName("status");
            builder.Property(x => x.MainImageUrl).IsRequired().HasColumnName("main_image_url");
            builder.Property(x => x.Quantity).IsRequired().HasColumnName("quantity");
        });
    }

    public static ArticleCategory ConvertToCategory(string value)
    {
        if (Enum.TryParse(value, out ArticleCategory category) && Enum.IsDefined(typeof(ArticleCategory), category))
        {
            return category;
        }
        return ArticleCategory.Other;
    }

}