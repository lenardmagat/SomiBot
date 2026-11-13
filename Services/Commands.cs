using System.Security;
using MediatR;
using SomiBot.DTOs;
using SomiBot.ErrorHandling;
using SomiBot.Models;

namespace SomiBot.Services;
public record UploadFileCommand(UploadDTO DTO) : IRequest<Result>;
public record GetFileDetailsCommand(): IRequest< Result<List<FileDetailsRespponse>>>;
public record SetQuizSourceCommand(int id): IRequest<Result>;