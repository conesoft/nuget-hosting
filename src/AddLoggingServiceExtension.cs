using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Conesoft.Hosting;

public static partial class AddLoggingServiceExtension
{
    public static WebApplicationBuilder AddLoggingService(this WebApplicationBuilder builder)
    {
        AttachConsole(-1);
        builder.Services.AddSingleton<LoggingService>();
        builder.Services.AddHostedService(s => s.GetRequiredService<LoggingService>()).AddSerilog();
        return builder;
    }

    public static IApplicationBuilder UseLoggingServiceOnRequests(this IApplicationBuilder app)
    {
        app.ApplicationServices.GetRequiredService<LoggingService>();
        app.UseSerilogRequestLogging();
        return app;
    }

    [System.Runtime.InteropServices.LibraryImport("kernel32.dll")]
    [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
    static private partial bool AttachConsole(int dwProcessId);
}