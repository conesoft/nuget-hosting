using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Conesoft.Hosting.Services;

public class ApplicationBuildHash
{
    private string? compiledHash;
    public string CompiledHash => compiledHash ??= RetrieveLinkerHash();


    // http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html

    private static string RetrieveLinkerHash()
    {
        var assembly = Assembly.GetEntryAssembly()!;
        Log.Information("generating app hash for {app}", Path.GetFileNameWithoutExtension(assembly.Location));
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => Path.GetDirectoryName(a.Location) == Path.GetDirectoryName(assembly.Location));

        var hash = $"{assemblies.Select(RetrieveLinkerHash).Aggregate(0, (sum, i) => HashCode.Combine(sum, i)):X}";
        Log.Information("app hash for {app}: {hash}", Path.GetFileNameWithoutExtension(assembly.Location), hash);
        return hash;
    }

    private static int RetrieveLinkerHash(Assembly assembly)
    {
        const int peHeaderOffset = 60;
        const int linkerCompileHashOffset = 8;
        var b = new byte[2048];
        FileStream? s = null;
        try
        {
            s = new FileStream(assembly.Location, FileMode.Open, FileAccess.Read);
            s.Read(b, 0, 2048);
        }
        finally
        {
            s?.Close();
        }
        var hash = BitConverter.ToInt32(b, BitConverter.ToInt32(b, peHeaderOffset) + linkerCompileHashOffset);
        Log.Information("generating assembly hash for {assembly}: {hash}", Path.GetFileNameWithoutExtension(assembly.Location), $"{hash:X}");
        return hash;
    }
}
