using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ReturnFalseWhenCancelledExtension
{
    public static async Task<bool> ReturnFalseWhenCancelled(this ValueTask<bool> task)
    {
        try
        {
            return await task;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }

    public static async Task<bool> ReturnFalseWhenCancelled(this Task<bool> task)
    {
        try
        {
            return await task;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
}