using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Conesoft.Hosting.Tasks;


public static class TasksExtensions
{
    public static IServiceCollection AddTasks(this IServiceCollection services)
    {
        var itask = typeof(ITask);
        foreach (var task in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => !t.IsInterface && itask.IsAssignableFrom(t)).ToArray())
        {
            services.AddSingleton(itask, task);
        }
        return services;
    }
}