using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

class ChangeBroadcaster()
{
    readonly HashSet<TaskCompletionSource> targets = [];

    public void Notify()
    {
        foreach (var target in targets)
        {
            target.SetResult();
        }
    }

    public async Task WaitForChange()
    {
        var target = new TaskCompletionSource();
        targets.Add(target);
        await target.Task;
        targets.Remove(target);
    }
}