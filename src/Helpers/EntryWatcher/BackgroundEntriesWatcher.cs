using Conesoft.Files;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

public abstract class BackgroundEntiesWatcher() : IHostedService
{
    readonly CancellationTokenSource cts = new();

    protected abstract Task<IEnumerable<Entry>> GetEntries();
    protected virtual bool AllDirectories { get; } = false;

    public abstract Task OnChange();

    async Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        var entries = await GetEntries();
        entries.Live(OnChange, AllDirectories, cts);
    }

    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        cts.Cancel();
        return Task.CompletedTask;
    }
}