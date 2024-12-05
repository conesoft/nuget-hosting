using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

public abstract class PeriodicTask(TimeSpan period) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var timer = new PeriodicTimer(period);
        do
        {
            await Process();
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    protected abstract Task Process();
}
