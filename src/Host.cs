﻿using Conesoft.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conesoft.Hosting;

public static class Host
{
    const string HostingDirectoryName = "Hosting";

    public static Directory Root { get; private set; }

    public static File GlobalSettings => Root / "Settings" / Filename.From("settings", "json");
    public static File LocalSettings => HostingType switch
    {
        "Websites" => Root / "Settings" / HostingType / Domain / Subdomain / Filename.From("settings", "json"),
        "Services" => Root / "Settings" / HostingType / Name / Filename.From("settings", "json"),
        _ => throw new Exception($"wrong hosting type {HostingType}")
    };

    public static File GlobalConfiguration => Root / "Settings" / Filename.From("hosting", "json");

    public static Directory GlobalStorage => Root / "Storage";
    public static Directory LocalStorage => HostingType switch
    {
        "Websites" => Root / "Storage" / HostingType / Domain / Subdomain,
        "Services" => Root / "Storage" / HostingType / Name,
        _ => throw new Exception($"wrong hosting type {HostingType}")
    };
    public static string Name { get; private set; } = "";
    public static string Domain { get; private set; } = "";
    public static string Subdomain { get; private set; } = "";
    public static string FullDomain => (Subdomain.Equals("main", StringComparison.InvariantCultureIgnoreCase) ? Domain : $"{Subdomain}.{Domain}").ToLowerInvariant();
    public static string HostingType { get; private set; } = "Websites";

    static Host()
    {
        var directory = Directory.Common.Current;

        if (directory.FilteredFiles("Deploy.pubxml", allDirectories: true).FirstOrDefault() is File file)
        {
            //file.ReadText();
            var content = System.IO.File.ReadAllText(file.Path); // no async allowed here :( oldschool

            static string? ExtractTagContents(string content, string tagNameWithoutBrackets)
            {
                var tags = (beginning: $"<{tagNameWithoutBrackets}>", end: $"</{tagNameWithoutBrackets}>");
                var indicees = (beginning: content.IndexOf(tags.beginning), end: content.IndexOf(tags.end));

                return indicees.beginning >= 0 && indicees.end >= 0
                    ? content[(indicees.beginning + tags.beginning.Length)..indicees.end]
                    : null;
            }

            if (ExtractTagContents(content, "Hosting") is string hosting)
            {
                if (ExtractTagContents(content, "Domain") is string domain)
                {
                    var segments = GetDomainSegments(domain);

                    Subdomain = segments.subdomain;
                    Domain = segments.domain;

                    Name = segments.domain;
                }
                else if (ExtractTagContents(content, "Name") is string name)
                {
                    Subdomain = name;
                    Domain = "Services";
                    HostingType = "Services";

                    Name = name;
                }

                Root = Directory.From(hosting);

                while (Root.Name != HostingDirectoryName && Root.Info.Parent != null)
                {
                    Root = Root.Parent;
                }
            }
            else
            {
                throw new Exception("Running in Debug? Publish Profile not properly configured");
            }
        }
        else
        {
            var subs = new Stack<string>();
            while (directory.Name != HostingDirectoryName && directory.Info.Parent != null)
            {
                subs.Push(directory.Name);
                directory = directory.Parent;
            }

            var currentSubdirectories = subs.ToArray();

            Name = currentSubdirectories.Last();

            if(currentSubdirectories.SkipLast(1).Last() == "Services")
            {
                HostingType = "Services";
            }

            Subdomain = currentSubdirectories.Last();
            Domain = currentSubdirectories.SkipLast(1).Last();

            Root = directory;
        }
    }

    static (string domain, string subdomain) GetDomainSegments(string name)
    {
        var domainSegments = name.Split('.').ToArray();
        var domain = $"{domainSegments.SkipLast(1).Last()}.{domainSegments.Last()}";
        var subdomain = domainSegments.Length == 3 ? domainSegments.First() : "main";

        return (domain, subdomain);
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hosting, configuration) =>
            {
                configuration.AddJsonFile(LocalSettings.Path, optional: true, reloadOnChange: true);
                configuration.AddJsonFile(GlobalSettings.Path, optional: true, reloadOnChange: true);
            });
    }
}