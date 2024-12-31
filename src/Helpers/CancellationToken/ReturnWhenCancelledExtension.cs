using System;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

public static class ReturnWhenCancelledExtension
{
    public static async Task ReturnWhenCancelled(this ValueTask task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
        }
    }

    public static async Task ReturnWhenCancelled(this Task task)
    {
        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
        }
    }
}