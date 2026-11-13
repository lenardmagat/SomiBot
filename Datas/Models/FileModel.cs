using System.ComponentModel.DataAnnotations;

namespace SomiBot.Models;
public class FileDetails
{
    [Key]
    public int FileId {get; set;}
    public string FileName {get; set;} = null!;
    public string FileKey {get; set;} = null!;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    public DateTime? UpdatedAt {get; set;} = null;
    public bool IsQuizSource {get; set;}
    public bool IsDeleted {get; set;} = false;
}