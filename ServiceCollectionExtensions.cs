using Microsoft.Extensions.Hosting;
using Orleans;
using Sphere.Interfaces;
using Sphere.Shared;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInjectableOrleansClient(this IServiceCollection services)
    {
        if (string.IsNullOrWhiteSpace(Services.Current))
        {
            throw new ApplicationException($"Must set {nameof(Services.Current)} to the name of the service.");
        }

        services.AddSingleton<ClusterClientHostedService>();
        services.AddSingleton<IHostedService>(
            sp => sp.GetRequiredService<ClusterClientHostedService>());
        services.AddSingleton<IClusterClient>(
            sp => sp.GetRequiredService<ClusterClientHostedService>().Client);
        services.AddSingleton<IGrainFactory>(
            sp => sp.GetRequiredService<ClusterClientHostedService>().Client);

        return services;
    }
}
