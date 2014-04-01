using System.Linq;
using IMu;
using CollectionsOnline.Core.Extensions;

namespace CollectionsOnline.Import.Factories
{
    public class PartiesNameFactory : IPartiesNameFactory
    {
        public string MakePartiesName(Map map)
        {
            if (map != null)
            {
                switch (map.GetString("NamPartyType"))
                {
                    case "Collaboration":
                        break;
                    case "Cutter Number":
                        return new[]
                        {
                            map.GetString("NamBranch"),
                            map.GetString("NamDepartment"),
                            map.GetString("NamOrganisation"),
                            map.GetString("AddPhysStreet"),
                            map.GetString("AddPhysCity"),
                            map.GetString("AddPhysState"),
                            map.GetString("AddPhysCountry")
                        }.Concatenate(", ");
                    case "Organisation":
                        return new[]
                        {
                            map.GetString("NamBranch"),
                            map.GetString("NamDepartment"),
                            map.GetString("NamOrganisation")
                        }.Concatenate(", ");
                    case "Person":
                        return new[]
                        {
                            map.GetString("NamFullName"),
                            map.GetString("NamOrganisation")
                        }.Concatenate(" - ");
                    case "Position":
                        break;
                    case "Transport":
                        var name = string.Empty;
                        var organisationOtherName =
                            map.GetStrings("NamOrganisationOtherNames_tab")
                                .FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                        var source = map.GetString("NamSource");

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
                            map.GetString("NamFullName"),
                            map.GetString("NamOrganisation")
                        }.Concatenate(", ");
                }
            }

            return null;
        }
    }
}