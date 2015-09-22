using AutoMapper;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
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
                cfg.CreateMap<Article, ArticleApiViewModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("article"));
                cfg.CreateMap<Item, ItemApiViewModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("item"));
                cfg.CreateMap<Species, SpeciesApiViewModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("species"));
                cfg.CreateMap<Specimen, SpecimenApiViewModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("specimen"));
                cfg.CreateMap<Comment, CommentApiViewModel>();
            });
        }
    }
}