using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class StartConsoleExtension
{
    public static async Task<LifetimeWrapper> StartConsoleAsync(this IHost host)
    {
        var cancellationTokenSource = new CancellationTokenSource();
        Console.CancelKeyPress += (_, _) => cancellationTokenSource.Cancel();
        await host.StartAsync();
        return new LifetimeWrapper(host, cancellationTokenSource.Token);
    }

    public class LifetimeWrapper(IHost host, CancellationToken token) : IDisposable
    {
        public CancellationToken CancellationToken => token;
        void IDisposable.Dispose() => host.StopAsync();
    }
}