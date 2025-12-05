using Application.Commands.Create;
using Application.Queries.Getall;
using Application.Queries.getById;
using Application.Queries.getByName;
using Domain;

namespace Application.utils;

public class Profile : AutoMapper.Profile
{
    public Profile()
    {
        CreateMap<Users, UsersGetAllOutput.Users>();
        CreateMap<Users, UserCreateOutput>();
        CreateMap<Users, UsersGetByIdOutput>();
        CreateMap<Users, UsersGetByUsernameOutput>();

        
        CreateMap<Articles, ArticlesGetAllOutput.Articles>();
        CreateMap<Articles, ArticleCreateOutput>();
        CreateMap<Articles, ArticlesGetByIdOutput>();
        
        CreateMap<Memberships, MembershipGetAllOutput.Memberships>();
        CreateMap<Memberships, MembershipCreateOutput>();
        CreateMap<Memberships, MembershipsGetByIdOutput>();
        
        CreateMap<UserMemberships, UsersMembershipGetAllOutput.UserMemberships>();
        CreateMap<UserMemberships, UserMembershipCreateOutput>();
        CreateMap<UserMemberships, UserMembershipsGetByIdOutput>();
        
        CreateMap<Transactions, TransactionsGetAllOutput.Transactions>();
        CreateMap<Transactions, TransactionCreateOutput>();
        CreateMap<Transactions, TransactionsGetByIdOutput>();

        CreateMap<Trades, TradesGetAllOutput.Trades>();
        CreateMap<Trades, TradeCreateOutput>();
        CreateMap<Trades, TradesGetByIdOutput>();

        CreateMap<Ratings, RatingsGetAllOutput.Ratings>();
        CreateMap<Ratings, RatingCreateOutput>();
        CreateMap<Ratings, RatingsGetByIdOutput>();

    }
}