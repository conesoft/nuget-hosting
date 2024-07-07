using Conesoft.Hosting.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Conesoft.Hosting;

public static class CacheBustingExtension
{
    public static IServiceCollection AddCompiledHashCacheBuster(this IServiceCollection services) => services.AddSingleton<ApplicationBuildHash>();

    public static IApplicationBuilder UseCompiledHashCacheBuster(this IApplicationBuilder app, string cookieName = "Conesoft.ApplicationHash") => app.Use(async (context, next) =>
    {
        var abh = (context.RequestServices.GetService(typeof(ApplicationBuildHash)) as ApplicationBuildHash)!;
        if (context.Request.Cookies.ContainsKey(cookieName) == true)
        {
            var hash = context.Request.Cookies[cookieName];
            if (hash != abh.CompiledHashString)
            {
                context.Response.Headers["Clear-Site-Data"] = "\"cache\"";
                Log.Information($"clearing cache on client");
            }
        }
        context.Response.Cookies.Append(cookieName, abh.CompiledHashString, new()
        {
            Secure = true,
            Expires = DateTimeOffset.MaxValue

        });
        await next();
    });
}
