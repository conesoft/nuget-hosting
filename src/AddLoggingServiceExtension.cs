using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class AddLoggingServiceExtension
{
    public static Builder AddLoggingService<Builder>(this Builder builder) where Builder : IHostApplicationBuilder
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