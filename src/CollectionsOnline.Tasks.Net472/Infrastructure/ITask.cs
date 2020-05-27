namespace CollectionsOnline.Tasks.Net472.Infrastructure
{
    public interface ITask
    {
        void Run();

        int Order { get; }
    }
}
