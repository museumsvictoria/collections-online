using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            // Automapper configuration
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Article, ArticleApiViewModel>();
                cfg.CreateMap<Item, ItemApiViewModel>();
                cfg.CreateMap<Species, SpeciesApiViewModel>();
                cfg.CreateMap<Specimen, SpecimenApiViewModel>();
                cfg.CreateMap<Comment, CommentApiViewModel>();
            });
        }
    }
}