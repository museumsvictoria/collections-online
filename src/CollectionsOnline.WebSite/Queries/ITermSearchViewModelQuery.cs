using System.Collections.Generic;
using CollectionsOnline.WebSite.Models;

namespace CollectionsOnline.WebSite.Queries
{
    public interface ITermSearchViewModelQuery
    {
        IList<TermSearchViewModel> BuildLocalityTermSearch(TermSearchInputModel termSearchInputModel);

        IList<TermSearchViewModel> BuildKeywordTermSearch(TermSearchInputModel termSearchInputModel);

        IList<TermSearchViewModel> BuildNameTermSearch(TermSearchInputModel termSearchInputModel);
    }
}