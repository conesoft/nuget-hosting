﻿using System;
using System.Threading.Tasks;

namespace Conesoft.Hosting;

public abstract class PeriodicCache<T>(TimeSpan period) : PeriodicTask(period) where T : class
{
    private T? content = null;

    public async Task<T> GetContent()
    {
        while (content == null)
        {
            await Task.Delay(25);
        }
        return content;
    }

    protected virtual Task OnContentChanged(T content) => Task.CompletedTask;

    protected override async Task Process()
    {
        content = await Generate();
        await OnContentChanged(content);
    }

    protected abstract Task<T> Generate();
}
