using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static partial class AddLoggingServiceExtension
{
    public static Builder AddLoggingService<Builder>(this Builder builder) where Builder : IHostApplicationBuilder
    {
        var environment = CreateHostEnvironmentFromConfigurationManually(builder.Configuration);
        if(environment.IsInHostedEnvironment == false)
        {
            AttachConsole(-1);
        }
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

    private static HostEnvironment CreateHostEnvironmentFromConfigurationManually(IConfigurationManager configurationManager)
    {
        // can't use DI yet, but can access configuration already.
        var options = configurationManager.GetRequiredSection("hosting").Get<HostingOptions>();
        if (options == null)
        {
            throw new Exception("can't read the required hosting section");
        }
        return new HostEnvironment(Options.Create(options));
    }
}