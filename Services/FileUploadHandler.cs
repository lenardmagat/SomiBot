using MediatR;
using Microsoft.EntityFrameworkCore;
using SomiBot.Database;
using SomiBot.ErrorHandling;
using SomiBot.Models;

namespace SomiBot.Services;
public class FileUploadHandler : IRequestHandler<UploadFileCommand, Result>
{
    private readonly DbManager _db;
    private readonly IFileStorageService _fileStorage;
    private readonly ILogger<FileUploadHandler> _logger;
    public FileUploadHandler(DbManager db, IFileStorageService fileStorage, ILogger<FileUploadHandler> logger)
    {
        _db = db;
        _fileStorage = fileStorage;
        _logger = logger;
    }
    public async Task<Result> Handle(UploadFileCommand command, CancellationToken cancellation)
    {
        var isExisting = await _db.fileDetails
            .AsNoTracking()
            .Where(f => f.FileName == command.DTO.FileName)
            .FirstOrDefaultAsync(cancellation);
        if(isExisting != null) return Result.Failure($"{command.DTO.FileName} is already exisiting", StatusCodes.Status400BadRequest);
        string key = $"{Guid.NewGuid()}.docx";
        using var transaction = await _db.Database.BeginTransactionAsync(cancellation);
        try
        {
            FileDetails fileDetails = new FileDetails
            {
                FileName = command.DTO.FileName,
                FileKey = key
            };
            await _db.fileDetails.AddAsync(fileDetails, cancellation);
            await _db.SaveChangesAsync(cancellation);
            var stream = command.DTO.File.OpenReadStream();
            var response = await _fileStorage.UploadAsync(stream, fileDetails.FileKey);
            if (!response.IsSuccess)
            {
                await transaction.RollbackAsync(cancellation);
                return response;
            }
            await transaction.CommitAsync(cancellation);
            return response;
        }
        catch(Exception e)
        {
            await transaction.RollbackAsync(cancellation);
            _logger.LogError(e, "An unexpected error occured while trying to upload docx.");
            return Result.Failure("An internal server error occured.", StatusCodes.Status500InternalServerError);
        }
    }

}