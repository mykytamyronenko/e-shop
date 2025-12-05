using System.Text;
using API.BackgroundServices;
using Application.Commands;
using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.Queries;
using Application.Queries.Getall;
using Application.Queries.getById;
using Application.Queries.getByName;
using Application.Services;
using Application.utils;
using Application.Utils;
using AutJwt;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TradeShopContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("tradeshop")));
builder.Services.AddAutoMapper(typeof(Profile));

//Users
builder.Services.AddScoped<IUsersRepository,UsersRepository>();
builder.Services.AddScoped<IQueryHandler<UsersGetAllQuery, UsersGetAllOutput>, UsersGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, UsersGetByIdOutput>, UsersGetByIdHandler>();
builder.Services.AddScoped<IQueryHandler<string, UsersGetByUsernameOutput>, UsersGetByUsernameHandler>();
builder.Services.AddScoped<UsersQueryProcessor>();
builder.Services.AddScoped<ICommandsHandler<BasicUserCreateCommand,UserCreateOutput>,UserCreateHandler>();
builder.Services.AddScoped<ICommandsAdminHandler<UserCreateCommand,UserCreateOutput>,UserCreateHandler>();
builder.Services.AddScoped<UserCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>, UserDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<UserUpdateCommand>, UserUpdateHandler>();
builder.Services.AddScoped<IUserDeleteCommandHandler,UserDeleteHandler>();


//Articles
builder.Services.AddScoped<IArticlesRepository, ArticlesRepository>();
//query
builder.Services.AddScoped<IQueryHandler<ArticlesGetAllQuery, ArticlesGetAllOutput>, ArticlesGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, ArticlesGetByIdOutput>, ArticlesGetByIdHandler>();
builder.Services.AddScoped<ArticlesQueryProcessor>();
//commands
builder.Services.AddScoped<ICommandsHandler<ArticleCreateCommand,ArticleCreateOutput>,ArticleCreateHandler>();
builder.Services.AddScoped<ArticleCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>,ArticleDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<ArticleUpdateCommand>, ArticleUpdateHandler>();
builder.Services.AddScoped<IArticleDeleteCommandHandler, ArticleDeleteHandler>();

//Membership
builder.Services.AddScoped<IMembershipsRepository, MembershipsRepository>();
//query
builder.Services.AddScoped<IQueryHandler<MembershipGetAllQuery, MembershipGetAllOutput>, MembershipGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, MembershipsGetByIdOutput>, MembershipsGetByIdHandler>();
builder.Services.AddScoped<MembershipsQueryProcessor>();
//commands
builder.Services.AddScoped<ICommandsHandler<MembershipCreateCommand,MembershipCreateOutput>,MembershipCreateHandler>();
builder.Services.AddScoped<MembershipCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>, MembershipDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<MembershipUpdateCommand>, MembershipUpdateHandler>();
builder.Services.AddScoped<IMembershipDeleteCommandHandler,MembershipDeleteHandler>();

//User membership
//query
builder.Services.AddScoped<IUserMembershipsRepository, UserMembershipsRepository>();
builder.Services.AddScoped<IQueryHandler<UsersMembershipGetAllQuery, UsersMembershipGetAllOutput>, UsersMembershipGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, UserMembershipsGetByIdOutput>, UserMembershipsGetByIdHandler>();
builder.Services.AddScoped<UserMembershipsQueryProcessor>();
//commands
builder.Services.AddScoped<ICommandsHandler<UserMembershipCreateCommand,UserMembershipCreateOutput>,UserMembershipCreateHandler>();
builder.Services.AddScoped<UserMembershipCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>, UserMembershipDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<UserMembershipUpdateCommand>, UserMembershipUpdateHandler>();
builder.Services.AddScoped<IUserMembershipDeleteCommandHandler,UserMembershipDeleteHandler>();

//Transation
builder.Services.AddScoped<ITransactionsRepository, TransactionsRepository>();
//query
builder.Services.AddScoped<IQueryHandler<TransactionsGetAllQuery, TransactionsGetAllOutput>, TransactionsGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, TransactionsGetByIdOutput>, TransactionsGetByIdHandler>();
builder.Services.AddScoped<TransactionsQueryProcessor>();
//commands
builder.Services.AddScoped<ICommandsHandler<TransactionCreateCommand, TransactionCreateOutput>, TransactionCreateHandler>();
builder.Services.AddScoped<TransactionCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>,TransactionDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<TransactionUpdateCommand>, TransactionUpdateHandler>();
builder.Services.AddScoped<ITransactionDeleteCommandHandler, TransactionDeleteHandler>();

//Trades
builder.Services.AddScoped<ITradesRepository, TradesRepository>();
//query
builder.Services.AddScoped<IQueryHandler<TradesGetAllQuery, TradesGetAllOutput>, TradesGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, TradesGetByIdOutput>, TradesGetByIdHandler>();
builder.Services.AddScoped<TradesQueryProcessor>();
//commands
builder.Services.AddScoped<ICommandsHandler<TradeCreateCommand, TradeCreateOutput>, TradeCreateHandler>();
builder.Services.AddScoped<TradeCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>,TradeDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<TradeUpdateCommand>, TradeUpdateHandler>();
builder.Services.AddScoped<ITradeDeleteCommandHandler, TradeDeleteHandler>();

//Ratings
builder.Services.AddScoped<IRatingsRepository, RatingsRepository>();
//query
builder.Services.AddScoped<IQueryHandler<RatingsGetAllQuery, RatingsGetAllOutput>, RatingsGetAllHandler>();
builder.Services.AddScoped<IQueryHandler<int, RatingsGetByIdOutput>, RatingsGetByIdHandler>();
builder.Services.AddScoped<RatingsQueryProcessor>();
//Command
builder.Services.AddScoped<ICommandsHandler<RatingCreateCommand, RatingCreateOutput>, RatingCreateHandler>();
builder.Services.AddScoped<RatingCommandsProcessor>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<int>,RatingDeleteHandler>();
builder.Services.AddScoped<IEmptyOutputCommandHandler<RatingUpdateCommand>, RatingUpdateHandler>();
builder.Services.AddScoped<IRatingDeleteCommandHandler, RatingDeleteHandler>();

builder.Services.AddScoped<UserMembershipCleanupService>();
builder.Services.AddHostedService<MembershipCleanupBackgroundService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new
            SymmetricSecurityKey
            (Encoding.UTF8.GetBytes
                (builder.Configuration["Jwt:Key"]))
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["cookie"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<UserService>();

builder.Logging.AddConsole();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "artImages/uploads")),
    RequestPath = "/artImages/uploads"
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "profilePic/uploads")),
    RequestPath = "/profilePic/uploads"
});

app.UseCors("Development");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
