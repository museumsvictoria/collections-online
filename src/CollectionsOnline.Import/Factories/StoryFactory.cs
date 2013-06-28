using CollectionsOnline.Core.DomainModels;
using IMu;

namespace CollectionsOnline.Import.Factories
{
    public class StoryFactory
    {
        public string MakeModuleName()
        {
            return "enarratives";
        }

        public string[] MakeColumns()
        {
            return new[]
                   {
                       "irn",
                       "NarTitle"
                   };
        }

        public Terms MakeTerms()
        {
            var terms = new Terms();

            terms.Add("DetPurpose_tab", "Website - History & Technology Collections");
            terms.Add("AdmPublishWebNoPassword", "Yes");

            return terms;
        }

        public Story MakeStory(Map map)
        {
            return new Story();
        }
    }
}
