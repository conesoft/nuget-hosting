using Conesoft.Files;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

public abstract class BackgroundEntryWatcher<T>() : IHostedService where T : Entry
{
    readonly CancellationTokenSource cts = new();

    protected abstract Task<T> GetEntry();
    protected virtual bool AllDirectories { get; } = false;

    public abstract Task OnChange(T entry);

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        var entry = await GetEntry();
        entry.Live(() => OnChange(entry), AllDirectories, cts);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        cts.Cancel();
        return Task.CompletedTask;
    }
}