using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
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