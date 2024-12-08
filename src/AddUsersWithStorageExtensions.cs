using Conesoft.Users;
using Microsoft.AspNetCore.Builder;

namespace Conesoft.Hosting;

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

    public static void MapUsersWithStorage(this WebApplication app) => app.MapUsers();
}
