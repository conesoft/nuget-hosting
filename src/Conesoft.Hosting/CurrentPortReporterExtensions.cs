using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Conesoft.Hosting
{
    static class CurrentPortReporterExtensions
    {
        public static IServiceCollection AddPortReporter(this IServiceCollection services) => services.AddSingleton<CurrentPortReporter>();

        public static IApplicationBuilder UsePortReporter(this IApplicationBuilder app, Action<int> handlePort)
        {
            var reporter = app.ApplicationServices.GetService<CurrentPortReporter>();
            reporter!.HandlePort(handler: handlePort);
            return app;
        }

        public static IApplicationBuilder UsePortReporter(this IApplicationBuilder app, Func<int, Task> handlePort)
        {
            var reporter = app.ApplicationServices.GetService<CurrentPortReporter>();
            reporter!.HandlePort(handler: handlePort);
            return app;
        }
    }
}
