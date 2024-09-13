using DEWAXP.Feature.CommonComponents.Models.Common;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.DI;
using Glass.Mapper.Sc;
using Sitecore.Globalization;
using System;
using System.Web;

//using static Sitecore.Configuration.State;

namespace DEWAXP.Feature.CommonComponents.Repositories
{
    [Service(typeof(ICMSComponentRepositories), Lifetime = Lifetime.Transient)]
    public class CMSComponentRepositories : ICMSComponentRepositories
    {
        private ICacheProvider _cacheProvider;
        private IContentRepository _contentRepository;
        private DewaProfile CurrentPrincipal;

        public CMSComponentRepositories(ICacheProvider cacheProvider, IContentRepository contentRepository, DewaProfile dewaProfile)
        {
            _cacheProvider = cacheProvider;
            _contentRepository = contentRepository;
            CurrentPrincipal = dewaProfile;
        }

        public bool MobileAppsView()
        {
            bool isnavhide = false;
            if (_cacheProvider.TryGet("HIDENAV", out isnavhide) && isnavhide && Translate.Text("IsFullHideEnabled") == "1")
            {
                return isnavhide;
            }
            HeaderFooterConfigModel hfConfigModel = _contentRepository.GetItem<HeaderFooterConfigModel>(new GetItemByIdOptions(new Guid(SitecoreItemIdentifiers.Header_Footer_Config)));
            if (hfConfigModel != null && hfConfigModel.QuerystrParamChecked)
            {
                if (HttpContext.Current.Request.QueryString[hfConfigModel.QuerystrParam] != null && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString[hfConfigModel.QuerystrParam]))
                {
                    if (HttpContext.Current.Request.QueryString[hfConfigModel.QuerystrParam].Trim().Replace(" ", "+").Equals(hfConfigModel.QuerystrParamVal))
                    {
                        _cacheProvider.Store("HIDENAV", new CacheItem<bool>(true, TimeSpan.FromHours(1)));
                        return true;
                    }
                }
            }
            return false;
        }
    }
}