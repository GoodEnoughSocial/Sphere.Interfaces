using Consul;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using Sphere.Shared;

namespace Sphere.Interfaces;

public sealed class ClusterClientHostedService : IHostedService, IAsyncDisposable, IDisposable
{
    private readonly IServiceProvider _services;
    private readonly ILogger<ClusterClientHostedService> _logger;
    private AgentServiceRegistration? _registration;

    public ClusterClientHostedService(ILoggerFactory loggerFactory, IServiceProvider services)
    {
        _services = services;

        _logger = loggerFactory.CreateLogger<ClusterClientHostedService>();
        Client = new ClientBuilder()
            .UseLocalhostClustering()
            .ConfigureServices(services =>
            {
                // Add logging from the host's container.
                services.AddSingleton(loggerFactory);
                services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            })
            .Build();
    }

    public IClusterClient Client { get; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var attempt = 0;
        var maxAttempts = 100;
        var delay = TimeSpan.FromSeconds(1);

        await Client.Connect(async error =>
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return false;
            }

            if (++attempt < maxAttempts)
            {
                _logger.LogWarning(error, "Failed to connect to Orleans cluster on attempt {Attempt} of {MaxAttempts}.", attempt, maxAttempts);

                try
                {
                    await Task.Delay(delay, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    return false;
                }

                return true;
            }
            else
            {
                _logger.LogError(error, "Failed to connect to Orleans cluster on attempt {Attempt} of {MaxAttempts}.", attempt, maxAttempts);
                return false;
            }
        });

        var discovery = Client.GetGrain<IServiceDiscovery>(Services.Accounts);
        var sd = await discovery.GetServiceDefinition();

        _registration = sd.GetServiceRegistration();
        await Services.RegisterService(_registration, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Services.UnregisterService(_registration, cancellationToken);
            await Client.Close();
        }
        catch (OrleansException error)
        {
            _logger.LogWarning(error, "Error while gracefully disconnecting from Orleans cluster. Will ignore and continue to shutdown.");
        }
    }

    public void Dispose() => Client?.Dispose();

    public ValueTask DisposeAsync() => Client?.DisposeAsync() ?? default;
}
