namespace CollectionsOnline.Tasks.NetCoreApp31.Infrastructure
{
    public interface ITask
    {
        void Run();

        int Order { get; }
    }
}
