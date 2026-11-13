using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using SomiBot.Database;
using SomiBot.DTOs;
using SomiBot.ErrorHandling;
using SomiBot.Models;

namespace SomiBot.Services;
public class GetFilesHandler : IRequestHandler<GetFileDetailsCommand,  Result<List<FileDetailsRespponse>>>
{
    private readonly DbManager _db;
    private readonly ILogger<GetFilesHandler> _logger;
    public GetFilesHandler(DbManager db, ILogger<GetFilesHandler> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task< Result<List<FileDetailsRespponse>>> Handle(GetFileDetailsCommand command, CancellationToken cancellation = default)
    {
        try{
            var filesDetails = await _db.fileDetails.AsNoTracking().Where(f => f.IsDeleted == false).ToListAsync(cancellation);
            List<FileDetailsRespponse> FileList = new();
            foreach(var file in filesDetails)
            {
                FileDetailsRespponse details = new FileDetailsRespponse(
                    file.FileName,
                    file.FileId,
                    file.IsQuizSource,
                    file.CreatedAt,
                    file.UpdatedAt
                );
                FileList.Add(details);
            }
            return Result<List<FileDetailsRespponse>>.Success(FileList);
        }catch(Exception e)
        {
            _logger.LogError(e, "An unexpected error occured while trying to fetch all active files.");
            return  Result<List<FileDetailsRespponse>>.Failure("An internal server occured!", StatusCodes.Status500InternalServerError);
        }
    }
}