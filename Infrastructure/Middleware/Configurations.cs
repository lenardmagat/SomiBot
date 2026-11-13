using System.Text.Json.Serialization;
using DotNetEnv;
using Serilog;
using SomiBot.DependenciesInjection;
using SomiBot.GlobalException;

namespace SomiBot.Configurations;
class Configurations
{
    static public WebApplication application()
    {
         var builder = WebApplication.CreateBuilder();
        Env.Load();
        builder.Services.AddApplicationServices(builder.Configuration);
        builder.Configuration.AddEnvironmentVariables();
        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Error()
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File("Logs/chat_log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Host.UseSerilog();
        builder.Services.AddProblemDetails();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
        return builder.Build();
    }
}