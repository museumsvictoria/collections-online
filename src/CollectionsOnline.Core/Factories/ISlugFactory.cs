namespace CollectionsOnline.Core.Factories
{
    public interface ISlugFactory
    {
        string MakeSlug(string value, int maxLength = 0);
    }
}