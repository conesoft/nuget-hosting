using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Conesoft.Hosting;

public static class AddHostEnvironmentInfoExtension
{
    public static WebApplicationBuilder AddHostEnvironmentInfo(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<HostEnvironment>();
        return builder;
    }
}