using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend_Receiving.BLL;
using Backend_Receiving.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Backend_Receiving
{
    public static class StartupExtensions
    {
        public static void AddReceivingBackendDependencies(this IServiceCollection service, Action<DbContextOptionsBuilder> options)
        {
            service.AddDbContext<Ebike_DMIT2018Context>(options);
            service.AddTransient<ReceivingService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<Ebike_DMIT2018Context>();
                return new ReceivingService(context);
            });
        }
    }
}
