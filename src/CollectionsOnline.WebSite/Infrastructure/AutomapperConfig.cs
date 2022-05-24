using AutoMapper;
using CollectionsOnline.Core.Extensions;
using CollectionsOnline.Core.Models;
using CollectionsOnline.Core.Utilities;
using CollectionsOnline.WebSite.Models;
using CollectionsOnline.WebSite.Models.Api;
using System.Configuration;
using System.Linq;

namespace CollectionsOnline.WebSite.Infrastructure
{
    public static class AutomapperConfig
    {
        public static void Initialize()
        {
            // Automapper configuration
            Mapper.Initialize(cfg =>
            {
                // Api view models
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

                // Api media view models
                cfg.CreateMap<Media, MediaApiViewModel>()
                    .Include<ImageMedia, ImageMediaApiViewModel>()
                    .Include<VideoMedia, VideoMediaApiViewModel>()
                    .Include<AudioMedia, AudioMediaApiViewModel>()
                    .Include<FileMedia, FileMediaApiViewModel>()
                    .Include<UriMedia, UriMediaApiViewModel>()
                    .AfterMap((src, dest) =>
                    {
                        dest.Id = $"media/{src.Irn}";
                    });
                cfg.CreateMap<ImageMedia, ImageMediaApiViewModel>();
                cfg.CreateMap<VideoMedia, VideoMediaApiViewModel>();
                cfg.CreateMap<AudioMedia, AudioMediaApiViewModel>();
                cfg.CreateMap<FileMedia, FileMediaApiViewModel>();
                cfg.CreateMap<UriMedia, UriMediaApiViewModel>();

                // Api Sub objects 
                cfg.CreateMap<Taxonomy, TaxonomyApiViewModel>();
                cfg.CreateMap<CollectionEvent, CollectionEventApiViewModel>();
                cfg.CreateMap<CollectionSite, CollectionSiteApiViewModel>();
                cfg.CreateMap<Licence, LicenceApiViewModel>();
                cfg.CreateMap<Author, AuthorApiViewModel>();

                // Csv Download                
                cfg.CreateMap<Article, EmuAggregateRootCsvModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("article"))
                    .AfterMap((src, dest) =>
                    {
                        dest.Licence = string.Format("{0} {1}", src.Licence.Name, !string.IsNullOrWhiteSpace(src.Licence.Uri) ? string.Format("({0})", src.Licence.Uri) : string.Empty).Trim();
                        dest.Authors = src.Authors.Select(x => x.FullName).Concatenate(", ");
                        dest.Contributors = src.Contributors.Select(x => x.FullName).Concatenate(", ");                        
                        dest.ArticleTypes = src.Types.Concatenate(", ");
                    });
                cfg.CreateMap<Item, EmuAggregateRootCsvModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("item"))
                    .AfterMap((src, dest) =>
                    {
                        dest.Licence = string.Format("{0} {1}", src.Licence.Name, !string.IsNullOrWhiteSpace(src.Licence.Uri) ? string.Format("({0})", src.Licence.Uri) : string.Empty).Trim();
                        dest.Associations = src.Associations.Select(a => string.Format("{0}: {1}", a.Type, new[] { a.Name, a.StreetAddress, a.Locality, a.Region, a.State, a.Country, a.Date }.Concatenate(", "))).Concatenate("; ");
                        dest.TradeLiteraturePrimaryRoleAndName = (!string.IsNullOrWhiteSpace(src.TradeLiteraturePrimaryRole)) ? string.Format("{0} : {1}", src.TradeLiteraturePrimaryRole, src.TradeLiteraturePrimaryName) : null;
                        dest.Dimensions = src.Dimensions.Select(d => string.Format("{0}: {1} {2}", (!string.IsNullOrWhiteSpace(d.Configuration)) ? d.Configuration : "Dimensions", d.Dimensions, d.Comments).Trim()).Concatenate("; ");
                        dest.IndigenousCulturesLocalities = src.IndigenousCulturesLocalities.Concatenate(", ");
                        dest.IndigenousCulturesCulturalGroups = src.IndigenousCulturesCulturalGroups.Concatenate(", ");
                    });
                cfg.CreateMap<Species, EmuAggregateRootCsvModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("species"))
                    .AfterMap((src, dest) =>
                    {
                        dest.DisplayTitle = HtmlConverter.HtmlToText(src.DisplayTitle);
                        dest.Licence = string.Format("{0} {1}", src.Licence.Name, !string.IsNullOrWhiteSpace(src.Licence.Uri) ? string.Format("({0})", src.Licence.Uri) : string.Empty).Trim();
                        dest.Authors = src.Authors.Select(x => x.FullName).Concatenate(", ");
                    });
                cfg.CreateMap<Specimen, EmuAggregateRootCsvModel>()
                    .ForMember(dest => dest.RecordType, opt => opt.UseValue("specimen"))
                    .AfterMap((src, dest) =>
                    {
                        dest.DisplayTitle = HtmlConverter.HtmlToText(src.DisplayTitle);
                        dest.Licence = string.Format("{0} {1}", src.Licence.Name, !string.IsNullOrWhiteSpace(src.Licence.Uri) ? string.Format("({0})", src.Licence.Uri) : string.Empty).Trim();
                        dest.Associations = src.Associations.Select(a => string.Format("{0}: {1}", a.Type, new[] { a.Name, a.StreetAddress, a.Locality, a.Region, a.State, a.Country, a.Date }.Concatenate(", "))).Concatenate("; ");
                        dest.Storage = src.Storages.Select(s => new[] { s.Nature, s.Form, s.FixativeTreatment, s.Medium}.Concatenate(", ")).Concatenate("; ");
                    });
            });
        }
    }
}