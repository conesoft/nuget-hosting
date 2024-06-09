using Conesoft.Files;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

public static class LoggingExtensions
{
    private static string ToTitleCase(this string text) => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());

    public static IServiceCollection AddLoggingToHost(this IServiceCollection services)
    {
        var log = Host.Root / "Logs" / Filename.From($"{Host.HostingType} - {Host.Name.ToTitleCase()} - ", "txt");

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

        Log.Information($"app '{Host.Name.ToTitleCase()}' starting up...");

        services.AddHostedService<HostedLoggingExtension>();

        return services;
    }

    class HostedLoggingExtension : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information($"app '{Host.Name.ToTitleCase()}' started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"app '{Host.Name.ToTitleCase()}' stopped");
            return Task.CompletedTask;
        }
    }
}
