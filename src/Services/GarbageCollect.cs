using Humanizer;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Conesoft.Hosting.Services;

// based on https://stackoverflow.com/a/3634544

public class GarbageCollect(TimeSpan period) : PeriodicTask(period)
{
    protected override Task Process()
    {
        var memoryBefore = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;
        GC.Collect(GC.MaxGeneration, GCCollectionMode.Aggressive, blocking: true, compacting: true);
        var memoryAfter = System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;
        Log.Information("running garbage collection cycle: {releasedMemory} released, {usedMemory} in use", Math.Max(0, memoryBefore - memoryAfter).Bytes(), memoryAfter.Bytes());

        return Task.CompletedTask;
    }
}
