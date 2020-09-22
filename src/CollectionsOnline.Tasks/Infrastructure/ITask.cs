using System.Threading;
using System.Threading.Tasks;

namespace CollectionsOnline.Tasks.Infrastructure
{
    public interface ITask
    {
        Task Run(CancellationToken stoppingToken);

        int Order { get; }
    }
}
