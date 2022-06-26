using Orleans;
using Sphere.Shared;

namespace Sphere.Interfaces;

public interface IServiceDiscovery : IGrainWithStringKey
{
    Task<ServiceDefinition> GetServiceDefinition();
    Task<ServiceDefinition> SetServiceDefinition(ServiceDefinition serviceDefinition);
}
