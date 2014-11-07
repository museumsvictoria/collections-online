using System.Linq;
using CollectionsOnline.Import.Extensions;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class PartiesNameFactory : IPartiesNameFactory
    {
        public string Make(Map map)
        {
            if (map != null)
            {
                switch (map.GetEncodedString("NamPartyType"))
                {
                    case "Collaboration":
                        return new[]
                        {
                            map.GetEncodedString("ColCollaborationName")
                        }.Concatenate(", ");
                    case "Cutter Number":
                        return new[]
                        {
                            map.GetEncodedString("NamBranch"),
                            map.GetEncodedString("NamDepartment"),
                            map.GetEncodedString("NamOrganisation"),
                            map.GetEncodedString("AddPhysStreet"),
                            map.GetEncodedString("AddPhysCity"),
                            map.GetEncodedString("AddPhysState"),
                            map.GetEncodedString("AddPhysCountry")
                        }.Concatenate(", ");
                    case "Organisation":
                        return new[]
                        {
                            map.GetEncodedString("NamBranch"),
                            map.GetEncodedString("NamDepartment"),
                            map.GetEncodedString("NamOrganisation")
                        }.Concatenate(", ");
                    case "Person":
                        return new[]
                        {
                            map.GetEncodedString("NamFullName"),
                            map.GetEncodedString("NamOrganisation")
                        }.Concatenate(" - ");
                    case "Position":
                        break;
                    case "Transport":
                        var name = string.Empty;
                        var organisationOtherName = map.GetEncodedStrings("NamOrganisationOtherNames_tab").FirstOrDefault();
                        var source = map.GetEncodedString("NamSource");

                        if (string.IsNullOrWhiteSpace(organisationOtherName) && !string.IsNullOrWhiteSpace(source))
                        {
                            name = source;
                        }
                        else if (!string.IsNullOrWhiteSpace(organisationOtherName) && string.IsNullOrWhiteSpace(source))
                        {
                            name = organisationOtherName;
                        }
                        else if (!string.IsNullOrWhiteSpace(organisationOtherName) && !string.IsNullOrWhiteSpace(source))
                        {
                            name = string.Format("{0} ({1})", organisationOtherName, source);
                        }

                        return new[]
                        {
                            name,
                            map.GetEncodedString("NamFullName"),
                            map.GetEncodedString("NamOrganisation")
                        }.Concatenate(", ");
                }
            }

            return null;
        }
    }
}