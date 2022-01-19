using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Backend_Purchasing.BLL;
using Backend_Purchasing.DAL;
#endregion

namespace Backend_Purchasing
{
    public static class StartupExtensions
    {
        public static void AddBackendPurchaseDependencies(this IServiceCollection services,
              Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<PurchasingContext>(options);

            services.AddTransient<PurchasingServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<PurchasingContext>();
                return new PurchasingServices(context);
            });
        }
    }
}
