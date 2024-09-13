using Glass.Mapper.Sc;
using Glass.Mapper.Sc.Web;
using Sitecore.Data.Items;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class SitecoreContextExtensions
    {
        public static string TextDirection()
        {
            var current = new RequestContext(new SitecoreService(SitecoreX.Database)).GetContextItem<Item>();
            if (current != null)
            {
                return current.Language.CultureInfo.TextInfo.IsRightToLeft ? "rtl" : "ltr";
            }
            return "ltr";
        }
    }
}