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

    public static HostedLoggingExtensionWrapper AddLogging(string logFilePath, string appName)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                logFilePath,
                buffered: false,
                shared: true,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            )
            .CreateLogger();

        Log.Information($"app '{appName.ToTitleCase()}' starting up...");

        return new(appName);
    }

    public static IServiceCollection AddLoggingToHost(this IServiceCollection services)
    {
        var name = Host.HostingType == "Websites" ?
            $"{Host.HostingType} - {Host.FullDomain.ToLowerInvariant()}" :
            $"{Host.HostingType} - {Host.Name.ToTitleCase()}";
        var log = Host.Root / "Logs" / Filename.From($"{name} - ", "txt");

        AddLogging(log.Path, Host.Name);

        services.AddHostedService<HostedLoggingExtension>();

        return services;
    }

    class HostedLoggingExtension() : IHostedService
    {
        private readonly string? appName = null;
        public HostedLoggingExtension(string appName) : this()
        {
            this.appName = appName.ToTitleCase();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information($"app '{appName ?? Host.Name.ToTitleCase()}' started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"app '{appName ?? Host.Name.ToTitleCase()}' stopped");
            return Task.CompletedTask;
        }
    }

    public class HostedLoggingExtensionWrapper() : IDisposable
    {
        private bool started = false;
        private readonly HostedLoggingExtension wrapped = new();

        public HostedLoggingExtensionWrapper(string appName) : this()
        {
            wrapped = new(appName);
        }

        public void Start()
        {
            wrapped.StartAsync(default);
            started = true;
        }

        void IDisposable.Dispose()
        {
            if (started)
            {
                wrapped.StopAsync(default);
            }
        }
    }
}
