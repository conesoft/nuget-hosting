using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class ConfigureOptionsSectionExtensions
{
    public static IServiceCollection ConfigureOptionsSection<T>(this IServiceCollection services, string section) where T : class
    {
        services.AddTransient<SectionName<T>>(_ => new(section));
        services.ConfigureOptions<Setup<T>>();
        return services;
    }

    class Setup<T>(IConfiguration configuration, SectionName<T> sectionName) : IConfigureOptions<T> where T : class
    {
        public void Configure(T options) => configuration.GetSection(sectionName.Section).Bind(options);

    }

    record SectionName<T>(string Section);
}