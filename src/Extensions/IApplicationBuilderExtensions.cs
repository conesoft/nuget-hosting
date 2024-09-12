using Microsoft.AspNetCore.Builder;
using System.Linq;

namespace Conesoft.Hosting;

public static class IApplicationBuilderExtensions
{
    static readonly string[] contentTypes = ["text", "json", "xml"];
    public static IApplicationBuilder UseHostingDefaults(this IApplicationBuilder app, bool useDefaultFiles, bool useStaticFiles)
    {
        app.Use(async (context, next) =>
        {
            context.Response.Headers.ContentType = "text/html; charset=utf-8";
            context.Response.Headers.XContentTypeOptions = "nosniff";
            await next.Invoke();
        });

        if (useDefaultFiles)
        {
            app.UseDefaultFiles();
        }

        if (useStaticFiles)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    if (context.Context.Response.Headers.ContentType.Count > 0)
                    {
                        var contentType = context.Context.Response.Headers.ContentType[0] ?? "";
                        if (contentTypes.Any(type => contentType.Contains(type)))
                        {
                            context.Context.Response.Headers.ContentType = contentType + "; charset=utf-8";
                        }
                    }
                    if (context.Context.Response.Headers.CacheControl.Count == 0)
                    {
                        context.Context.Response.Headers.CacheControl = "max-age=31536000, immutable";
                    }
                }
            });
        }

        app.Use(async (context, next) =>
        {
            if (context.Response.Headers.CacheControl.Count == 0)
            {
                context.Response.Headers.Pragma = "no-cache";
                context.Response.Headers.CacheControl = "no-cache, no-store";
            }
            await next.Invoke();
        });
        return app;
    }
}
