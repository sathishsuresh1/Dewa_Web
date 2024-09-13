using DEWAXP.Feature.CommonComponents.Models.Analytics;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using System.Web.Mvc;

namespace DEWAXP.Feature.CommonComponents.Controllers.Common
{
    public class LinkedInAnalyticsController : BaseController
    {
        // GET: Analytics
        public PartialViewResult LinkedInAnalytic()
        {
            var model = ContentRepository.GetItem<LinkedInAnalyticConfig>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.LINKEDIN_CONFIG));
            return PartialView("~/Views/Feature/CommonComponents/Analytics/_LinkedInAnalytics.cshtml", model);
        }
    }
}