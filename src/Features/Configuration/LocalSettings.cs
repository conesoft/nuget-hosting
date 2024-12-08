using System.Threading.Tasks;

namespace Conesoft.Hosting;

public class LocalSettings(Files.File file)
{
    public async Task Save<T>(T obj) where T : class => await file.WriteAsJson(obj);
}
