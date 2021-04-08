using Conesoft.Files;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Conesoft.Hosting
{

    public static class Host
    {
        const string HostingDirectoryName = "Hosting";

        public static Directory Root { get; private set; }

        public static File GlobalSettings => Root / "Settings" / HostingType / Filename.From("settings", "json");
        public static File LocalSettings => Root / "Settings" / HostingType / Domain / Subdomain / Filename.From("settings", "json");

        public static Directory GlobalStorage => Root / "Storage" / HostingType;
        public static Directory LocalStorage => Root / "Storage" / HostingType / Domain / Subdomain;

        static readonly string[] currentSubdirectories;

        public static string Domain { get; private set; }
        public static string Subdomain { get; private set; }
        public static string HostingType { get; private set; }

        static Host()
        {
            var directory = Directory.Common.Current;

            if (directory.Filtered("Deploy*.pubxml", allDirectories: true).FirstOrDefault() is File file)
            {
                //file.ReadText();
                var content = System.IO.File.ReadAllText(file.Path); // no async allowed here :( oldschool

                var tags = (beginning: "<PublishUrl>", end: "</PublishUrl>");
                var indicees = (beginning: content.IndexOf(tags.beginning) + tags.beginning.Length, end: content.IndexOf(tags.end));

                var path = content[indicees.beginning..indicees.end];

                directory = Directory.From(path);
            }

            var subs = new Stack<string>();
            while (directory.Name != HostingDirectoryName && directory.Info.Parent != null)
            {
                subs.Push(directory.Name);
                directory = directory.Parent;
            }

            currentSubdirectories = subs.ToArray();

            Subdomain = currentSubdirectories.Last();
            Domain = currentSubdirectories.SkipLast(1).Last();
            HostingType = currentSubdirectories.SkipLast(2).Last();

            Root = directory;

            if (Root.Name != HostingDirectoryName)
            {
                throw new Exception("Running in Debug? Publish Profile not properly configured");
            }
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
}