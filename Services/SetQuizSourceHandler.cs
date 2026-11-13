using MediatR;
using Microsoft.EntityFrameworkCore;
using SomiBot.Database;
using SomiBot.ErrorHandling;

namespace SomiBot.Services;
public class SetQuizHandler : IRequestHandler<SetQuizSourceCommand, Result>
{
    private readonly DbManager _db;
    private readonly ILogger<SetQuizHandler> _logger;
    public SetQuizHandler(DbManager db, ILogger<SetQuizHandler> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task<Result> Handle(SetQuizSourceCommand command, CancellationToken cancellationToken)
    {
        var IsIdExisting = await _db.fileDetails.AsNoTracking().Where(f => f.FileId == command.id).FirstOrDefaultAsync(cancellationToken);
        if(IsIdExisting == null)
            return Result.Failure("Invalid Credential.", StatusCodes.Status400BadRequest);
        using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await _db.fileDetails.Where(f => f.IsQuizSource == true).ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.IsQuizSource, false)
                );
            await _db.fileDetails.Where(f => f.FileId == command.id).ExecuteUpdateAsync(setter => setter
                .SetProperty(p => p.IsQuizSource, true)
            );
            await transaction.CommitAsync(cancellationToken);
            return Result.Success();
        }catch(Exception e)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(e, "An unexpected error while shifting the quizSource");
            return Result.Failure("An internal server error occured.", StatusCodes.Status500InternalServerError);
        }
    }
}