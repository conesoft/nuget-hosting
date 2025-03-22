using Conesoft.Notifications;
using Microsoft.Extensions.Hosting;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AddNotificationServiceExtension
{
    public static Builder AddNotificationService<Builder>(this Builder builder) where Builder : IHostApplicationBuilder
    {
        builder.AddNotificationService((HostEnvironment environment) => new(
            Root: environment.Global.Storage.Path,
            Name: environment.ApplicationName
        ));
        return builder;
    }
}