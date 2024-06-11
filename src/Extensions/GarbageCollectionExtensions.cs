using Conesoft.Hosting.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Conesoft.Hosting;

public static class GarbageCollectionExtensions
{
    public static IServiceCollection AddPeriodicGarbageCollection(this IServiceCollection services, TimeSpan period)
    {
        return services.AddHostedServiceWith<GarbageCollect>(period);
    }
}
