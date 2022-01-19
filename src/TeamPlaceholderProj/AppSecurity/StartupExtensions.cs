using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AppSecurity.BLL;
using AppSecurity.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AppSecurity
{
    public static class StartupExtensions
    {
        public static void AddBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<AppSecurityDbContext>(options);
            services.AddTransient<SecurityService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<AppSecurityDbContext>();
                return new SecurityService(context);
            });
        }
    }
}
