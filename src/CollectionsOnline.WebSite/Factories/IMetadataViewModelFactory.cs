using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Factories
{
    public interface IMetadataViewModelFactory
    {
        MetadataViewModel Make(Article article);

        MetadataViewModel Make(Collection collection);

        MetadataViewModel Make(Item item);

        MetadataViewModel Make(Species species);

        MetadataViewModel Make(Specimen specimen);

        MetadataViewModel MakeHomeIndex();

        MetadataViewModel MakeAboutIndex();

        MetadataViewModel MakeCollectionIndex();

        MetadataViewModel MakeDevelopersIndex();

        MetadataViewModel MakeSearchIndex();
    }
}