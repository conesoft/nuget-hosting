using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AddCompiledHashCacheBusterExtension
{
    public static IServiceCollection AddCompiledHashCacheBuster(this IServiceCollection services) => services.AddSingleton<ApplicationBuildHash>();

    public static IApplicationBuilder UseCompiledHashCacheBuster(this IApplicationBuilder app, string cookieName = "Conesoft.ApplicationHash")
    {
        return app.Use(async (context, next) =>
        {
            var abh = context.RequestServices.GetRequiredService<ApplicationBuildHash>();
            if (context.Request.Cookies.ContainsKey(cookieName) == true)
            {
                var hash = context.Request.Cookies[cookieName];
                if (hash != abh.CompiledHash)
                {
                    context.Response.Headers["Clear-Site-Data"] = "\"cache\"";
                    Log.Information($"clearing cache on client");
                }
            }
            context.Response.Cookies.Append(cookieName, abh.CompiledHash, new()
            {
                Secure = true,
                Expires = DateTimeOffset.MaxValue

            });
            await next();
        });
    }
}
