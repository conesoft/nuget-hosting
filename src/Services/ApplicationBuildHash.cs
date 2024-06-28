using Serilog;
using System;
using System.IO;

namespace Conesoft.Hosting.Services;

public class ApplicationBuildHash
{
    private int? compiledHash;
    public int CompiledHash => compiledHash ??= RetrieveLinkerHash();
    public string CompiledHashString => CompiledHash.ToString("X");

    // http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html
    private static int RetrieveLinkerHash()
    {
        const int peHeaderOffset = 60;
        const int linkerCompileHashOffset = 8;
        var b = new byte[2048];
        FileStream? s = null;
        try
        {
            s = new FileStream(System.Reflection.Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read);
            s.Read(b, 0, 2048);
        }
        finally
        {
            s?.Close();
        }
        var hash = BitConverter.ToInt32(b, BitConverter.ToInt32(b, peHeaderOffset) + linkerCompileHashOffset);
        Log.Information($"current app hash: {hash}");
        return hash;
    }
}
