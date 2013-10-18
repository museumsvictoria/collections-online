namespace CollectionsOnline.WebApi.Modules
{
    public class IndexModule : BaseModule
    {
        public IndexModule()
        {
            Get["/"] = parameters => View["index"];
        }
    }
}