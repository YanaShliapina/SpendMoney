using SpendMoney.Core.Services;
using SpendMoney.Core.Services.Interfaces;

namespace SpendMoney.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountServicecs, AccountService>();
        }
    }
}
