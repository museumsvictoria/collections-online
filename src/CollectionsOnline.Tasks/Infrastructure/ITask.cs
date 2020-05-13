namespace CollectionsOnline.Tasks.Infrastructure
{
    public interface ITask
    {
        void Run();

        int Order { get; }
    }
}
