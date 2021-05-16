using Microsoft.Extensions.DependencyInjection;

namespace Conesoft.Hosting
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddHostingDefaults(this IServiceCollection services) => services.AddPortReporter();
    }
}
