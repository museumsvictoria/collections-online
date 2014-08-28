using System.Collections.Generic;
using System.Linq;
using CollectionsOnline.Core.Factories;
using CollectionsOnline.Core.Models;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class AssociationFactory : IAssociationFactory
    {
        private readonly ISlugFactory _slugFactory;
        private readonly IPartiesNameFactory _partiesNameFactory;

        public AssociationFactory(
            IPartiesNameFactory partiesNameFactory,
            ISlugFactory slugFactory)
        {
            _partiesNameFactory = partiesNameFactory;
            _slugFactory = slugFactory;
        }

        public Association Make(Map map)
        {
            var association = new Association
            {
                Type = map.GetString("AssAssociationType_tab"),
                Name = _partiesNameFactory.Make(map.GetMap("party")),
                Date = map.GetString("AssAssociationDate_tab"),
                Comments = map.GetString("AssAssociationComments0")
            };

            var place = new[]
            {
                map.GetString("AssAssociationStreetAddress_tab"),
                map.GetString("AssAssociationLocality_tab"),
                map.GetString("AssAssociationRegion_tab").Remove(new[] { "greater", "district" }),
                map.GetString("AssAssociationState_tab"),
                map.GetString("AssAssociationCountry_tab")
            }.Distinct();

            association.Place = place.Concatenate(", ");
            association.PlaceKey = _slugFactory.MakeSlug(association.Place);

            if (string.IsNullOrWhiteSpace(association.PlaceKey))
                association.GeocodeStatus = GeocodeStatus.Failure;

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