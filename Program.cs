using Microsoft.EntityFrameworkCore;
using Serilog;
using SomiBot.Configurations;
using SomiBot.Database;

var app = Configurations.application();
app.UseExceptionHandler();
app.UseSerilogRequestLogging();
app.UseRouting();
app.UseCors("DevFrontend");
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DbManager>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database tables.");
    }
}
app.Run();