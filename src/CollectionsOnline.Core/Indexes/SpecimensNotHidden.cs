using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class SpecimensNotHidden : AbstractIndexCreationTask<Specimen>
    {
        public SpecimensNotHidden()
        {
            Map = specimens =>
                from specimen in specimens
                where specimen.IsHidden == false
                select new {};
        }
    }
}
