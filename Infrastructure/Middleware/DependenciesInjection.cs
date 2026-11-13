using Amazon.Runtime;
using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using SomiBot.Database;
using SomiBot.Security.KeyConfigurations;
namespace SomiBot.DependenciesInjection;
public static class DependeciesInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var _DbKey = configuration.GetConnectionString("DefaultConnection") 
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? configuration["DB_CONNECTION_STRING"]
            ?? Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        if(string.IsNullOrEmpty(_DbKey)) throw new InvalidConfigurationException("Data Base connection string is misisng");
        services.AddDbContext<DbManager>(options => options.UseNpgsql(_DbKey));
        services.AddOptions<R2Options>()
            .Bind(configuration.GetSection("R2"))
            .ValidateDataAnnotations()
            .ValidateOnStart(); 
        services.AddSingleton<IAmazonS3>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<R2Options>>().Value;
                var config = new AmazonS3Config
                {
                    ServiceURL = $"https://{options.AccountId}.r2.cloudflarestorage.com",
                    ForcePathStyle = true,
                    RequestChecksumCalculation = RequestChecksumCalculation.WHEN_REQUIRED,
                    ResponseChecksumValidation = ResponseChecksumValidation.WHEN_REQUIRED
                };
                return new AmazonS3Client(options.AccessKey, options.SecretKey, config);
            });
        services.AddScoped<IFileStorageService, R2FileStorageService>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        services.AddCors(options =>
            {
            options.AddPolicy("DevFrontend", policy =>
                policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                    .AllowAnyMethod()
                    .AllowAnyHeader());
                });
        return services;
        }

}