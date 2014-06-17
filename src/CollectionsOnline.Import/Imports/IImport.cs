namespace CollectionsOnline.Import.Imports
{
    public interface IImport
    {
        void Run();

        int Order { get; }
    }
}