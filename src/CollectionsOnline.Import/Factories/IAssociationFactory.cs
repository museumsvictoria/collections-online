using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface IAssociationFactory
    {
        Association Make(Map map);

        IList<Association> Make(IEnumerable<Map> maps);
    }
}