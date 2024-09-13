using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    //[SitecoreType(TemplateId = "{F2040448-FF53-485D-A498-59CB165A3BEE}", AutoMap = true)]
    public class DownloadListing : Listing
    {
        public virtual IEnumerable<Download> Children { get; set; }
    }
}