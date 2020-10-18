using System;
using EducNotes.API.Data;
using EducNotes.API.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EducNotes.API
{
    public class Program
    {
      public static void Main(string[] args)
      {
        var host = CreateWebHostBuilder(args).Build();
        using(var scope = host.Services.CreateScope())
        {
          var services = scope.ServiceProvider;
          try
          {
            var context = services.GetRequiredService<DataContext>();
            var userManager = services.GetRequiredService<UserManager<User>>();
            var roleManager = services.GetRequiredService<RoleManager<Role>>();
            context.Database.Migrate();
            Seed.SeedUsers(context, userManager, roleManager);
          }
          catch(Exception ex)
          {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "an error occured during migration");
          }
        }

        host.Run();
      }

      public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
          WebHost.CreateDefaultBuilder(args)
              .UseStartup<Startup>();
    }
}
