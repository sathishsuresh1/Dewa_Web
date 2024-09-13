using DEWAXP.Foundation.DI;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Sites;
using System;
using System.Web;

namespace DEWAXP.Foundation.Content.Provider
{
    [Service(typeof(IRobotsTxtProvider),Lifetime =Lifetime.Singleton)]
    public class RobotsTxtProvider: IRobotsTxtProvider
    {
        public string GetRobotsTxtFileContent(SiteContext siteContext)
        {
            string robotsTxtContent = @"User-agent: *"
                                      + Environment.NewLine +
                                      "Disallow: /sitecore";
            if (siteContext != null)
            {
                var robotSettings = siteContext.Database.Items.GetItem(siteContext.Properties["robots"]);
                
                if (robotSettings != null)
                {
                    robotsTxtContent = robotSettings.Fields["Robots File Content"].Value;
                }
            }
            return robotsTxtContent;
        }
    }
}