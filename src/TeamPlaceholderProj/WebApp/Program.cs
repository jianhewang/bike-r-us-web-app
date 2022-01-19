using AppSecurity.BLL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Data;

namespace WebApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            await ApplicationUserSeeding(host);
            host.Run();
        }

        private static async Task ApplicationUserSeeding(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger<Program>();
                var env = services.GetRequiredService<IWebHostEnvironment>();
                if (env is not null && env.IsDevelopment())
                {
                    try
                    {
                        var configuration = services.GetRequiredService<IConfiguration>();
                        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                        if (!userManager.Users.Any())
                        {
                            var capstoneService = services.GetRequiredService<SecurityService>();
                            var users = capstoneService.ListEmployees();
                            string password = configuration.GetValue<string>("Setup:InitialPassword");
                            foreach (var person in users)
                            {
                                var user = new ApplicationUser
                                {
                                    UserName = person.UserName,
                                    Email = person.Email,
                                    EmployeeId = person.EmployeeId,
                                    EmailConfirmed = true
                                };
                                var result = await userManager.CreateAsync(user, password);
                                if (!result.Succeeded)
                                {
                                    logger.LogInformation("User was not created");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "An error occurred seeing the website users");
                    }
                }
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }

}
