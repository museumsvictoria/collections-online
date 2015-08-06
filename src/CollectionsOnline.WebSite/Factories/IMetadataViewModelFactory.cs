using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using Nancy;

namespace CollectionsOnline.WebSite.Factories
{
    public interface IMetadataViewModelFactory
    {
        MetadataViewModel Make(Article article);

        MetadataViewModel Make(Collection collection);

        MetadataViewModel MakeHomeIndex();

        MetadataViewModel MakeCollectionIndex();
    }
}