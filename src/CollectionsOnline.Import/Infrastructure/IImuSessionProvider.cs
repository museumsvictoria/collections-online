namespace CollectionsOnline.Import.Infrastructure
{
    public interface IImuSessionProvider
    {
        ImuSession CreateInstance(string moduleName);
    }
}