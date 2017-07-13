using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using THNETII.Acme.Client;
using THNETII.Common;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AcmeServiceCollectionExtensions
    {
        private static IServiceCollection AddAcmeClientInternal(this IServiceCollection services)
        {
            services.TryAddSingleton<HttpClient>();
            services.AddSingleton<AcmeClient>();

            return services;
        }

        public static IServiceCollection AddAcmeClient(this IServiceCollection services)
            => AddAcmeClientInternal(services.ThrowIfNull(nameof(services)));

        public static IServiceCollection AddAcmeClient(this IServiceCollection services, IConfiguration configuration)
            => AddAcmeClient(services, _ => configuration);

        public static IServiceCollection AddAcmeClient(this IServiceCollection services, Func<IConfiguration> configurationFactory)
            => AddAcmeClient(services, _ => configurationFactory?.Invoke());

        public static IServiceCollection AddAcmeClient(this IServiceCollection services, Func<IServiceProvider, IConfiguration> configurationFactory)
            => AddAcmeClientInternal(services.ThrowIfNull(nameof(services))
                .AddSingleton(serviceProvider => new AcmeConfiguration(configurationFactory?.Invoke(serviceProvider)))
                );
    }
}
