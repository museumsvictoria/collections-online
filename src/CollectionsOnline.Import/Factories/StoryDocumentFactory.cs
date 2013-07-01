using System.Linq;
using CollectionsOnline.Core.DomainModels;
using CollectionsOnline.Core.Factories;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class StoryDocumentFactory : IDocumentFactory<Story>
    {
        private readonly ISlugFactory _slugFactory;

        public StoryDocumentFactory(
            ISlugFactory slugFactory)
        {
            _slugFactory = slugFactory;
        }

        public string MakeModuleName()
        {
            return "enarratives";
        }

        public string[] MakeColumns()
        {
            return new[]
                   {
                       "irn",
                       "NarTitle",                       
                       "DesSubjects_tab",
                       "NarNarrative",
                   };
        }

        public Terms MakeTerms()
        {
            var terms = new Terms();

            terms.Add("DetPurpose_tab", "Website - History & Technology Collections");
            terms.Add("AdmPublishWebNoPassword", "Yes");

            return terms;
        }

        public Story MakeDocument(Map map)
        {
            var irn = map.GetString("irn");
            var title = map.GetString("NarTitle");
            var tags = map.GetStrings("DesSubjects_tab").Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => _slugFactory.MakeSlug(x)).ToArray();

            var story = map.GetString("NarNarrative");

            return new Story(
                irn,
                title,
                tags);
        }
    }
}
