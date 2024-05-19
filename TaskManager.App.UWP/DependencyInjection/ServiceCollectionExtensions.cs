using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.App.UWP.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNamedSingleton<TService, TImplementation>(this IServiceCollection services, string name, Func<IServiceProvider, TImplementation> implementationFactory)
            where TService : class
            where TImplementation : class, TService
        {
            services.AddSingleton(implementationFactory);
            services.AddSingleton<INamedServiceProvider<TService>>(sp =>
            {
                var provider = sp.GetRequiredService<IServiceProvider>();
                var _services = new Dictionary<string, TService>
                    {
                        { name, provider.GetRequiredService<TService>() }
                    };
                return new NamedServiceProvider<TService>(provider, _services);
            });

            return services;
        }
    }
}
