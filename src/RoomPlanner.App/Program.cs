using Microsoft.EntityFrameworkCore;
using RoomPlanner.Infrastructure.Context;

namespace RoomPlanner.App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            // Auto-migration of database
            using (var scope = host.Services.CreateScope())
            {
                ApplicationDbContext db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                ILogger<Program> log = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

                if (db.Database.GetPendingMigrations().Any())
                {
                    log.LogInformation("Starting Migrations");
                    db.Database.Migrate();
                    log.LogInformation("Migrations completed");
                }
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}