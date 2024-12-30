using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Conesoft.Hosting;

public static class AddHostEnvironmentInfoExtension
{
    public static Builder AddHostEnvironmentInfo<Builder>(this Builder builder) where Builder : IHostApplicationBuilder
    {
        builder.Services.AddSingleton<HostEnvironment>();
        return builder;
    }
}