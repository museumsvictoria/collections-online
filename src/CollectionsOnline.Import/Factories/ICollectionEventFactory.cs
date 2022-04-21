﻿using CollectionsOnline.Core.Models;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public interface ICollectionEventFactory
    {
        CollectionEvent Make(Map map, string type, string registrationPrefix, string scientificGroup);
    }
}