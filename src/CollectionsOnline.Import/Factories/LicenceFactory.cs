using System.Linq;
using CollectionsOnline.Core.Config;
using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class LicenceFactory : ILicenceFactory
    {
        public Licence MakeMediaLicence(string licence)
        {
            if (licence.Contains("All Rights Reserved") || licence == "Third Party Copyright")
                return Constants.Licences[LicenceType.AllRightsReserved];
            if (licence == "No Known Restriction" || licence == "Public Domain Mark")
                return Constants.Licences[LicenceType.PublicDomainMark];
            
            return Constants.Licences.Select(x => x.Value).SingleOrDefault(x => x.ShortName == licence);
        }

        public Licence MakeItemSpecimenLicence(Map map)
        {
            if ((map.ContainsKey("ClaObjectSummary") && !string.IsNullOrWhiteSpace(map.GetString("ClaObjectSummary"))) ||
                (map.ContainsKey("SubHistoryTechSignificance") && !string.IsNullOrWhiteSpace(map.GetString("SubHistoryTechSignificance"))))
                return Constants.Licences[LicenceType.CcBy];

            return Constants.Licences[LicenceType.Cc0];
        }
    }
}