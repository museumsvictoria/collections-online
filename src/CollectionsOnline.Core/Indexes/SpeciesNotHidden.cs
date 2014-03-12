using System.Linq;
using CollectionsOnline.Core.Models;
using Raven.Client.Indexes;

namespace CollectionsOnline.Core.Indexes
{
    public class SpeciesNotHidden : AbstractIndexCreationTask<Species>
    {
        public SpeciesNotHidden()
        {
            Map = speciesDocs => 
                from species in speciesDocs
                where species.IsHidden == false
                select new {};
        }
    }
}
