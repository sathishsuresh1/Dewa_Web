using Sitecore.Data.Items;
using Sitecore.Sites;

namespace DEWAXP.Foundation.Content.Provider
{
    public interface IRobotsTxtProvider
    {
        string GetRobotsTxtFileContent(SiteContext siteRoot);
    }
}