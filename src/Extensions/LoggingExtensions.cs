using Conesoft.Files;
using Humanizer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

public static class LoggingExtensions
{
    public static HostedLoggingExtensionWrapper AddLogging(Directory logFilePath, string appName)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(
                (logFilePath / "as Text" / Filename.From($"{appName} - ", "txt")).Path,
                buffered: false,
                shared: true,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            )
            .WriteTo.File(
                new CompactJsonFormatter(),
                (logFilePath / Filename.From($"{appName} - ", "log")).Path,
                buffered: false,
                shared: true,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: null,
                flushToDiskInterval: TimeSpan.FromSeconds(1)
            )
            .CreateLogger();

        Log.Information($"app '{appName.Titleize()}' starting up...");

        return new(appName);
    }


    public static IServiceCollection AddLoggingToHost(this IServiceCollection services)
    {
        var name = Host.HostingType == "Websites" ?
            $"{Host.HostingType} - {Host.FullDomain.ToLowerInvariant()}" :
            $"{Host.HostingType} - {Host.Name.Titleize()}";
        var log = Host.Root / "Logs" / name;

        return services.AddLoggingToHost(log, name);
    }

    public static IServiceCollection AddLoggingToHost(this IServiceCollection services, Directory logFilePath, string appName)
    {
        AddLogging(logFilePath, appName);

        services.AddHostedService<HostedLoggingExtension>();

        services.AddSerilog();

        AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Unhandled Exception");
            }
            else
            {
                Log.Fatal("Unhandled Exception: {ex}", e.ExceptionObject);
            }
        };

        return services;
    }

    public static IApplicationBuilder UseLoggingToHost(this IApplicationBuilder app) => app.UseSerilogRequestLogging();

    class HostedLoggingExtension() : IHostedService
    {
        private readonly string? appName = null;
        public HostedLoggingExtension(string appName) : this()
        {
            this.appName = appName.Titleize();
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information($"app '{appName ?? Host.Name.Titleize()}' started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information($"app '{appName ?? Host.Name.Titleize()}' stopped");
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
