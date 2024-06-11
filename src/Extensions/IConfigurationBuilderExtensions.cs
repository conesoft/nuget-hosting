using Microsoft.Extensions.Configuration;

namespace Conesoft.Hosting;

public static class IConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddHostConfiguration(this IConfigurationBuilder configuration)
    {
        configuration.AddJsonFile(Host.LocalSettings.Path, optional: true, reloadOnChange: true);
        configuration.AddJsonFile(Host.GlobalSettings.Path, optional: true, reloadOnChange: true);
        return configuration;
    }
}
