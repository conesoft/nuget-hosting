using Conesoft.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Conesoft.Hosting;

public static class UsersWithStorageExtensions
{
    public static IServiceCollection AddUsersWithStorage(this IServiceCollection services)
    {
        var directory = Host.GlobalStorage / "Users";
        services.AddUsers("Conesoft.Host.User", directory.Path);
        return services;
    }

    public static void MapUsersWithStorage(this WebApplication app) => app.MapUsers();
}
