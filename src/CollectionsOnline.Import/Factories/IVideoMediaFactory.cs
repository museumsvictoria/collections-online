using CollectionsOnline.Core.Models;

namespace CollectionsOnline.Import.Factories
{
    public interface IVideoMediaFactory
    {
        bool Make(ref VideoMedia videoMedia);
    }
}
