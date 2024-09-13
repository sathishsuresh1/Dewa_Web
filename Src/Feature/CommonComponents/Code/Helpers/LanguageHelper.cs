using DEWAXP.Feature.CommonComponents.Models.LanguageSwitcher;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Data.Items;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SitecoreX = global::Sitecore;

namespace DEWAXP.Feature.CommonComponents.Helpers
{
    public static class LanguageHelper
    {
        private static IContentRepository _contentRepository = new ContentRepository(new RequestContext(new SitecoreService(SitecoreX.Context.Database)));
        private static IContextRepository _contextRepository = new ContextRepository(new RequestContext(new SitecoreService(SitecoreX.Context.Database)));
        public static LanguageSwitcher languageIdentification(string tabindex, int type, HttpRequestBase Request)
        {
            var qs = QueryString.Parse(Request.RawUrl);
            var currentItem = _contextRepository.GetCurrentItem<Item>();
            var currentLanguage = SitecoreX.Context.Language;
            var otherLanguage = Sitecore.Context.Database.Languages.FirstOrDefault(l => l.Name != currentLanguage.Name);
            var hasOtherLanguage = _contentRepository.GetItem<Item>(new GetItemByIdOptions(currentItem.ID.Guid) {Language = otherLanguage }) != null;

            var options = new Sitecore.Links.UrlBuilders.ItemUrlBuilderOptions();
            options.Language = otherLanguage;

            var altUrl = qs.CopyTo(SitecoreX.Links.LinkManager.GetItemUrl(currentItem, options));
            var langViewModel = new LanguageSwitcher(currentLanguage, otherLanguage, hasOtherLanguage, altUrl, tabindex);
            langViewModel.SwitcherType = type;
            return langViewModel;
        }
    }
}