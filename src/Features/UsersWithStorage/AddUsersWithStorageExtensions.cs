using Conesoft.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

public static class AddUsersWithStorageExtensions
{
    [Obsolete("not supported anymore")]
    public static IServiceCollection AddUsersWithStorage(this IServiceCollection services)
    {
        //var directory = Host.GlobalStorage / "Users";
        //services.AddUsers("Conesoft.Host.User", directory.Path);
        return services;
    }

    [Obsolete("not supported anymore")]
    public static void MapUsersWithStorage(this WebApplication app) => app.MapUsers();
}
