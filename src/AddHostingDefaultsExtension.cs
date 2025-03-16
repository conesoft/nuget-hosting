using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Conesoft.Hosting;

public static class AddHostingDefaultsExtension
{
    public static Builder AddHostingDefaults<Builder>(this Builder builder, bool addHttpClient = true, bool addBlazor = true) where Builder : IHostApplicationBuilder
    {
        if (addHttpClient)
        {
            builder.Services.AddHttpClient();
        }
        if (addBlazor)
        {
            builder.Services.AddRazorComponents().AddInteractiveServerComponents().AddCircuitOptions(options =>
            {
                options.DetailedErrors = true;
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromSeconds(0);
                options.DisconnectedCircuitMaxRetained = 0;
            });
        }
        return builder;
    }
}
