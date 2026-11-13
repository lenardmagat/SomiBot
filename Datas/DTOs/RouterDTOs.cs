namespace SomiBot.DTOs;
public record UploadDTO(
    IFormFile File,
    string FileName
);
public record FileDetailsRespponse(
    string filename,
    int fileid,
    bool IsQuizSource,
    DateTime DateCreated,
    DateTime? DateUpdatedAt
);