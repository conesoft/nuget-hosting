using System.Threading.Tasks;

namespace Conesoft.Hosting.Tasks;

public interface ITask
{
    public Task Run();
}