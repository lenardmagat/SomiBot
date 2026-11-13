using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SomiBot.Models;

namespace SomiBot.Database;
public class DbManager : DbContext
{
    public DbSet<FileDetails> fileDetails {get; set;} = null!;
    public DbManager(DbContextOptions<DbManager> options) : base(options){}
}