using Backend_SalesReturns.BLL;
using Backend_SalesReturns.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_SalesReturns
{
    public static class SalesStartUpExtentions
    {
        public static void AddSalesBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<SalesReturnsContext>(options);
            services.AddTransient<SalesReturnService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesReturnsContext>();
                return new SalesReturnService(context);
            });
        }
    }
}
