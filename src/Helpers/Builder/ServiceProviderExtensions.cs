using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Conesoft.Hosting.Helpers.Builder;

public static class ResolverExtension
{
    // Just a helper method to shorten code registration code
    // based on
    // https://stackoverflow.com/a/53885374/1528847

    [Obsolete("not supported anymore")]
    public static IServiceCollection AddSingletonWith<T>(this IServiceCollection services, params object[] parameters) where T : class
    {
        return services.AddSingleton(c => ActivatorUtilities.CreateInstance<T>(c, parameters));
    }

    [Obsolete("not supported anymore")]
    public static IServiceCollection AddHostedServiceWith<T>(this IServiceCollection services, params object[] parameters) where T : class, IHostedService
    {
        return services.AddSingletonWith<T>(parameters).AsHostedService<T>();
    }

    [Obsolete("not supported anymore")]
    public static IServiceCollection AsHostedService<T>(this IServiceCollection services) where T : class, IHostedService
    {
        return services.AddHostedService(p => p.GetRequiredService<T>());
    }
}