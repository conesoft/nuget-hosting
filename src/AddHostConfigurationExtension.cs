using Conesoft.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Conesoft.Hosting;

[Browsable(false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public static class AddHostConfigurationExtension
{
    public static Builder AddHostConfigurationFiles<Builder>(this Builder builder) where Builder : IHostApplicationBuilder
    {
        builder.AddHostConfigurationToConfiguration(developmentMode: builder.Environment.IsDevelopment());

        builder.Services.ConfigureOptionsSection<HostingOptions>(section: "hosting");

        return builder;
    }

    public static Builder AddHostConfigurationFiles<Builder>(this Builder builder, Action<Configurator> configureTypes)
        where Builder : IHostApplicationBuilder
    {
        builder.AddHostConfigurationFiles();

        configureTypes(new(builder.Services, builder.Configuration));

        return builder;
    }

    private static Builder AddHostConfigurationToConfiguration<Builder>(this Builder builder, bool developmentMode)
        where Builder : IHostApplicationBuilder
    {
        var deployFile = Directory.Common.Current.FilteredFiles("Deploy.pubxml", allDirectories: true).FirstOrDefault();
        var configuration = builder.Configuration;

        var appName = FindAppName(configuration, deployFile);
        var root = FindRoot(configuration, deployFile);

        configuration.AddAppNameToConfiguration(appName);
        configuration.AddRootToConfiguration(root);

        var settingsRoot = Directory.From(root) / "Settings";
        configuration.AddJsonFile((settingsRoot / Filename.From("settings", "json")).Path);
        configuration.AddJsonFile((settingsRoot / Filename.From(appName, "json")).Path, optional: true, reloadOnChange: true);

        builder.Services.AddSingleton(new LocalSettings(settingsRoot / Filename.From(appName, "json")));

        if (developmentMode == false)
        {
            configuration.AddJsonFile((settingsRoot / Filename.From("hosting", "json")).Path);
        }

        return builder;
    }

    private static string FindAppName(IConfigurationManager _, File? deployFile)
    {
        var appNameFromDeployFile = Safe.Try(() => XDocument.Load(deployFile!.Path).XPathSelectElement("//Name|//Domain")?.Value);

        var appNameFromEntryAssemblyPath = Safe.Try(() => File.From(Assembly.GetEntryAssembly()!.Location).Parent.Name);

        return appNameFromDeployFile
            ?? appNameFromEntryAssemblyPath
            ?? throw new Exception("Could not find hosting:appname from Deploy.pubxml or Entry Assembly Location");
    }

    private static void AddAppNameToConfiguration(this IConfigurationManager configuration, string appName)
    {
        configuration.AddInMemoryCollection([new("hosting:appname", appName)]);
    }

    private static string FindRoot(IConfigurationManager configuration, File? deployFile)
    {
        var rootFromConfiguration = configuration["hosting:root"];

        var rootFromDeployHostingValue = Safe.Try(() => Directory.From(XDocument.Load(deployFile!.Path).XPathSelectElement("//Hosting")!.Value).Parent.Parent.Path);

        var rootFromAssemblyParentPath = Safe.Try(() => File.From(Assembly.GetEntryAssembly()!.Location).Parent.Parent.Parent.Parent.Path);

        return rootFromConfiguration
            ?? rootFromDeployHostingValue
            ?? rootFromAssemblyParentPath
            ?? throw new Exception("Could not find hosting:root from appseettings.json, Deploy.pubxml or Entry Assembly Location");
    }

    private static void AddRootToConfiguration(this IConfigurationManager configuration, string root)
    {
        configuration.AddInMemoryCollection([new("hosting:root", root)]);
    }
}
