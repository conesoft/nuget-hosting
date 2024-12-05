using Microsoft.Extensions.Configuration;

namespace Conesoft.Hosting.Features.Configuration.Options;

public class HostingOptions()
{
    [ConfigurationKeyName("root")]
    public string Root { get; init; } = "";

    [ConfigurationKeyName("certificate-password")]
    public string CertificatePassword { get; init; } = "";

    [ConfigurationKeyName("appname")]
    public string AppName { get; init; } = "";
}
