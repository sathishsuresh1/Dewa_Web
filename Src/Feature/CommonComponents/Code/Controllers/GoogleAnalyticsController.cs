using DEWAXP.Feature.CommonComponents.Models.Analytics;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using System.Web.Mvc;

namespace DEWAXP.Feature.CommonComponents.Controllers
{
    public class GoogleAnalyticsController : BaseController
    {
        // GET: GoogleAnalytics
        public PartialViewResult GoogleAnalyticsConfig()
        {
            var model = ContentRepository.GetItem<GoogleAnalyticsConfig>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.GOOGLE_CONFIG));

            return PartialView("~/Views/Feature/CommonComponents/Analytics/_GoogleAnalytics.cshtml", model);
        }

        public PartialViewResult GoogleAnalyticsConfigHead()
        {
            var model = ContentRepository.GetItem<GoogleAnalyticsConfig>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.GOOGLE_CONFIG));

            return PartialView("~/Views/Feature/CommonComponents/Analytics/_GoogleAnalyticsHead.cshtml", model);
        }
    }
}