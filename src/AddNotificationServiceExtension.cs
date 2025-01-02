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
        builder.AddNotificationService((HostEnvironment environment) => environment.Global.Storage);
        return builder;
    }
}