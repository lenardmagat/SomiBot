using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using SomiBot.ErrorHandling;
using SomiBot.Security.KeyConfigurations;

namespace SomiBot.Database;
public interface IFileStorageService
{
    Task<Result> UploadAsync(Stream fileStream, string key);
    Task<byte[]> DownloadAsync(string key);
    Task<Result> DeleteAsync(string key);
}

public class R2FileStorageService : IFileStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public R2FileStorageService(IAmazonS3 s3Client, IOptions<R2Options> options)
    {
        _s3Client = s3Client;
        _bucketName = options.Value.BucketName;
    }

    public async Task<Result> UploadAsync(Stream fileStream, string key)
    {
        var response = await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            DisablePayloadSigning = true
        });
        if(response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            return Result.Failure("Failed to upload file", StatusCodes.Status400BadRequest);
        }
        return Result.Success();
    }
    public async Task<Result> DeleteAsync(string key)
    {
        var response = await _s3Client.DeleteObjectAsync(_bucketName, key);
        if(response.HttpStatusCode != System.Net.HttpStatusCode.OK)
        {
            return Result.Failure("Failed to delete file", StatusCodes.Status400BadRequest);
        }
        return Result.Success();
    }
    public async Task<byte[]> DownloadAsync(string key)
    {
        var response = await _s3Client.GetObjectAsync(_bucketName, key);
        using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}