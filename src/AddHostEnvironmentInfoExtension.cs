using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Conesoft.Hosting;

public static class AddHostEnvironmentInfoExtension
{
    public static IHostApplicationBuilder AddHostEnvironmentInfo(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<HostEnvironment>();
        return builder;
    }
}