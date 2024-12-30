using Conesoft.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Conesoft.Hosting;

public static class AddHostConfigurationExtension
{
    public static IHostApplicationBuilder AddHostConfigurationFiles(this IHostApplicationBuilder builder, bool legacyMode)
    {
        builder.AddHostConfigurationToConfiguration(developmentMode: builder.Environment.IsDevelopment(), legacyMode);

        builder.Services.ConfigureOptionsSection<HostingOptions>(section: "hosting");

        return builder;
    }

    public static IHostApplicationBuilder AddHostConfigurationFiles<OptionsType>(this IHostApplicationBuilder builder, string section, bool legacyMode) where OptionsType : class
    {
        builder.AddHostConfigurationFiles(legacyMode);

        builder.Services.ConfigureOptionsSection<OptionsType>(section);

        return builder;
    }

    public static IHostApplicationBuilder AddHostConfigurationFiles<OptionsType>(this IHostApplicationBuilder builder, bool legacyMode) where OptionsType : class
    {
        builder.AddHostConfigurationFiles(legacyMode);

        builder.Services.Configure<OptionsType>(builder.Configuration);

        return builder;
    }

    private static IHostApplicationBuilder AddHostConfigurationToConfiguration(this IHostApplicationBuilder builder, bool developmentMode, bool legacyMode)
    {
        var deployFile = Directory.Common.Current.FilteredFiles("Deploy.pubxml", allDirectories: true).FirstOrDefault();
        var configuration = builder.Configuration;

        var appName = FindAppName(configuration, deployFile, legacyMode);
        var root = FindRoot(configuration, deployFile, legacyMode);

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

    private static string FindAppName(IConfigurationManager _, File? deployFile, bool legacyMode)
    {
        var appNameFromDeployFile = Safe.Try(() => XDocument.Load(deployFile!.Path).XPathSelectElement("//Name|//Domain")?.Value);

        var appNameFromExecutingAssemblyPath = Safe.Try(() => File.From(Assembly.GetExecutingAssembly().Location).Parent.Name);

        if (legacyMode)
        {
            if(Assembly.GetExecutingAssembly().FullName?.Contains("Website") ?? false)
            {
                appNameFromExecutingAssemblyPath = Safe.Try(() =>
                {
                    var file = File.From(Assembly.GetExecutingAssembly().Location);

                    return file.Parent.Name + "." + file.Parent.Parent.Name;
                });
            }
        }

        return appNameFromDeployFile
            ?? appNameFromExecutingAssemblyPath
            ?? throw new Exception("Could not find hosting:appname from Deploy.pubxml or Executing Assembly Location");
    }

    private static void AddAppNameToConfiguration(this IConfigurationManager configuration, string appName)
    {
        configuration.AddInMemoryCollection([new("hosting:appname", appName)]);
    }

    private static string FindRoot(IConfigurationManager configuration, File? deployFile, bool legacyMode)
    {
        var rootFromConfiguration = configuration["hosting:root"];

        var rootFromDeployHostingValue = Safe.Try(() => Directory.From(XDocument.Load(deployFile!.Path).XPathSelectElement("//Hosting")!.Value).Parent.Parent.Path);

        var rootFromAssemblyParentPath = Safe.Try(() => File.From(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Path);

        if (legacyMode)
        {
            rootFromAssemblyParentPath = Safe.Try(() => File.From(Assembly.GetExecutingAssembly().Location).Parent.Parent.Parent.Parent.Parent.Path);
        }

        return rootFromConfiguration
            ?? rootFromDeployHostingValue
            ?? rootFromAssemblyParentPath
            ?? throw new Exception("Could not find hosting:root from appseettings.json, Deploy.pubxml or Executing Assembly Location");
    }

    private static void AddRootToConfiguration(this IConfigurationManager configuration, string root)
    {
        configuration.AddInMemoryCollection([new("hosting:root", root)]);
    }
}
