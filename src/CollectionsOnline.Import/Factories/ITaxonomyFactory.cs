using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface ITaxonomyFactory
    {
        Taxonomy Make(Map map);

        string MakeScientificName(QualifierRankType qualifierRank, string qualifier, Taxonomy taxonomy);
    }
}