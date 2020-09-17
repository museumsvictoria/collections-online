using System.Threading;
using System.Threading.Tasks;

namespace CollectionsOnline.Tasks.NetCoreApp31.Infrastructure
{
    public interface ITask
    {
        Task Run(CancellationToken stoppingToken);

        int Order { get; }
    }
}
