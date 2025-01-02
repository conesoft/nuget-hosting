using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public class Configurator(IServiceCollection services, IConfigurationManager configuration)
{
    public void Add<OptionsType>(string? section = null) where OptionsType : class
    {
        if(section != null)
        {
            services.ConfigureOptionsSection<OptionsType>(section);
        }
        else
        {
            services.Configure<OptionsType>(configuration);
        }
    }
}