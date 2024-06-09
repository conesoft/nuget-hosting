using Conesoft.Files;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;

namespace Conesoft.Hosting;

public static class LoggingExtensions
{
    public static IServiceCollection AddLoggingToHost(this IServiceCollection services)
    {
        var log = Host.Root / "Logs" / Filename.From($"{Host.HostingType} - {Host.Name.ToLowerInvariant()} - ", "txt");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                log.Path,
                buffered: false,
                shared: true,
                rollingInterval: RollingInterval.Day,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            )
            .CreateLogger();

        Log.Information($"app '{Host.Name}' starting up...");

        return services;
    }
}
