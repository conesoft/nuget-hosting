using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AddHostEnvironmentInfoExtension
{
    public static Builder AddHostEnvironmentInfo<Builder>(this Builder builder) where Builder : IHostApplicationBuilder
    {
        builder.Services.AddSingleton<HostEnvironment>();
        return builder;
    }
}