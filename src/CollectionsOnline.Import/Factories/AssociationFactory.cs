using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Import.Extensions;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class AssociationFactory : IAssociationFactory
    {
        private readonly IPartiesNameFactory _partiesNameFactory;

        public AssociationFactory(
            IPartiesNameFactory partiesNameFactory)
        {
            _partiesNameFactory = partiesNameFactory;
        }

        public Association Make(Map map)
        {
            var association = new Association
            {
                Type = map.GetEncodedString("AssAssociationType_tab"),
                Name = _partiesNameFactory.Make(map.GetMap("party")),
                Date = map.GetEncodedString("AssAssociationDate_tab"),
                Comments = map.GetEncodedString("AssAssociationComments0"),
                StreetAddress = map.GetEncodedString("AssAssociationStreetAddress_tab"),
                Locality = map.GetEncodedString("AssAssociationLocality_tab"),
                Region = map.GetEncodedString("AssAssociationRegion_tab"),
                State = map.GetEncodedString("AssAssociationState_tab"),
                Country = map.GetEncodedString("AssAssociationCountry_tab")
            };

            return association;
        }

        public IList<Association> Make(IEnumerable<Map> maps)
        {
            var associations = new List<Association>();

            associations.AddRange(maps.Select(Make).Where(x => x != null));

            return associations;
        }
    }
}