namespace CollectionsOnline.Core.Models
{
    public abstract class EmuAggregateRoot : AggregateRoot
    {
        public bool IsHidden { get; set; }
    }
}