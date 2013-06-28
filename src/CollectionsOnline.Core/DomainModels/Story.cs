using System;
using CollectionsOnline.Core.Config;

namespace CollectionsOnline.Core.DomainModels
{
    public class Story : DomainModel
    {
        public bool Title { get; private set; }

        public Story()
        {
        }
    }
}