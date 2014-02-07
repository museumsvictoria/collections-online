namespace CollectionsOnline.WebSite.Features.Specimens
{
    public interface ISpecimenViewModelQuery
    {
        SpecimenViewModel BuildSpecimen(string specimenId);
    }
}