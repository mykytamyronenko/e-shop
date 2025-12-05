using Application.Commands.Create;
using Application.Commands.Delete;
using Application.Commands.update;
using Application.utils;
using Application.Utils;
using Domain;

namespace Application.Commands;

public class ArticleCommandsProcessor
{
    private readonly ICommandsHandler<ArticleCreateCommand,ArticleCreateOutput> _createArticleOutputHandler;
    private readonly IArticleDeleteCommandHandler _deleteArticleCommandHandler;
    private readonly IEmptyOutputCommandHandler<ArticleUpdateCommand> _updateArticleCommandHandler;


    public ArticleCommandsProcessor(ICommandsHandler<ArticleCreateCommand, ArticleCreateOutput> createArticleOutputHandler, IArticleDeleteCommandHandler deleteArticleCommandHandler, IEmptyOutputCommandHandler<ArticleUpdateCommand> updateArticleCommandHandler)
    {
        _createArticleOutputHandler = createArticleOutputHandler;
        _deleteArticleCommandHandler = deleteArticleCommandHandler;
        _updateArticleCommandHandler = updateArticleCommandHandler;
    }

    public ArticleCreateOutput CreateArticle(ArticleCreateCommand command)
    {
        return _createArticleOutputHandler.Handle(command);
    }

    public void DeleteArticle(int id)
    {
        _deleteArticleCommandHandler.Handle(id);
    }

    
    public void UpdateArticle(ArticleUpdateCommand command)
    {
        _updateArticleCommandHandler.Handle(command);
    }
}