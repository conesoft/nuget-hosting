using Conesoft.Users;
using Microsoft.AspNetCore.Builder;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AddUsersWithStorageExtensions
{
    public static WebApplicationBuilder AddUsersWithStorage(this WebApplicationBuilder builder)
    {
        builder.AddUsers<HostEnvironment>((options, environment) =>
        {
            options.CookieName = "Conesoft.Users";
            options.Directory = (environment.Global.Storage / "Users").Path;
        });
        return builder;
    }

    public static WebApplication MapUsersWithStorage(this WebApplication app) => app.MapUsers();
}
