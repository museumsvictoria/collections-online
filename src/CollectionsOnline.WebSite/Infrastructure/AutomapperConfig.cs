using System.Configuration;
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

                // Prevent double handling of adding site base to uri
                cfg.CreateMap<MediaFile, MediaFileApiViewModel>()
                    .BeforeMap((src, dest) => src.Uri = src.Uri.Contains(ConfigurationManager.AppSettings["CanonicalSiteBase"]) ? src.Uri : string.Format("{0}{1}", ConfigurationManager.AppSettings["CanonicalSiteBase"], src.Uri))
                    .Include<ImageMediaFile, ImageMediaFileApiViewModel>();
                cfg.CreateMap<ImageMediaFile, ImageMediaFileApiViewModel>();

                cfg.CreateMap<Media, MediaApiViewModel>()
                    .Include<ImageMedia, ImageMediaApiViewModel>()
                    .Include<VideoMedia, VideoMediaApiViewModel>()
                    .Include<AudioMedia, AudioMediaApiViewModel>()
                    .Include<FileMedia, FileMediaApiViewModel>()
                    .Include<UriMedia, UriMediaApiViewModel>();
                cfg.CreateMap<ImageMedia, ImageMediaApiViewModel>();
                cfg.CreateMap<VideoMedia, VideoMediaApiViewModel>();
                cfg.CreateMap<AudioMedia, AudioMediaApiViewModel>();
                cfg.CreateMap<FileMedia, FileMediaApiViewModel>();
                cfg.CreateMap<UriMedia, UriMediaApiViewModel>();

                cfg.CreateMap<Taxonomy, TaxonomyApiViewModel>();
                cfg.CreateMap<CollectionEvent, CollectionEventApiViewModel>();
                cfg.CreateMap<CollectionSite, CollectionSiteApiViewModel>();

                cfg.CreateMap<Author, AuthorApiViewModel>();
            });
        }
    }
}