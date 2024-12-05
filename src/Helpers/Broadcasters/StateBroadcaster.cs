using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

public class StateBroadcaster<T>(T startingState = default!)
{
    readonly HashSet<Channel<T>> targets = [];
    private T last = startingState;

    public Task NotifyAsync(T value)
    {
        last = value;
        return Task.WhenAll(targets.Select(t => t.Writer.WriteAsync(value).AsTask()));
    }

    public async IAsyncEnumerable<T> ListenAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var target = Channel.CreateBounded<T>(new BoundedChannelOptions(1)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });
        targets.Add(target);
        await foreach (var item in target.Reader.ReadAllAsync(cancellationToken).Prepend(last))
        {
            yield return item;
        }
        targets.Remove(target);
    }

}

public static class StateBroadcaster
{
    public static StateBroadcaster<T> CreateWith<T>(T startingState) => new(startingState);
}