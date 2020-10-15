using System.Collections.Generic;
using CollectionsOnline.Core.Models;
using IMu;
using Raven.Client;

namespace CollectionsOnline.Import.Factories
{
    public interface IEmuAggregateRootFactory<T> where T : EmuAggregateRoot
    {
        string ModuleName { get; }

        string[] Columns { get; }

        Terms Terms { get; }

        T MakeDocument(Map map);

        void UpdateDocument(T newDocument, T existingDocument,IList<string> missingDocumentIds,
            IDocumentSession documentSession);
    }
}