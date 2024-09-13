using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Header
{
    [SitecoreType(TemplateId = "{B4EF974C-0B8D-42AB-8C30-E96CE40F18C8}", AutoMap = true)]
    public class MyAccountMenu : GlassBase
    {
        [SitecoreField("Dashboard Title")]
        public virtual string DashboardTitle { get; set; }

        [SitecoreField("Dashboard Items")]
        public virtual IEnumerable<ContentBase> DashboardItems { get; set; }

        [SitecoreField("Column 1 Title")]
        public virtual string Column1Title { get; set; }

        [SitecoreField("Column 1 Items")]
        public virtual IEnumerable<ContentBase> Column1Items { get; set; }

        [SitecoreField("Column 1 Show More Text")]
        public virtual string Column1ShowMoreText { get; set; }

        [SitecoreField("Column 1 Show More Link")]
        public ContentBase Column1ShowMoreLink { get; set; }

        [SitecoreField("Column 2 Title")]
        public virtual string Column2Title { get; set; }

        [SitecoreField("Column 2 Items")]
        public virtual IEnumerable<ContentBase> Column2Items { get; set; }

        [SitecoreField("Column 2 Show More Text")]
        public virtual string Column2ShowMoreText { get; set; }

        [SitecoreField("Column 2 Show More Link")]
        public ContentBase Column2ShowMoreLink { get; set; }

        [SitecoreField("Column 3 Title")]
        public virtual string Column3Title { get; set; }

        [SitecoreField("Column 3 Items")]
        public virtual IEnumerable<ContentBase> Column3Items { get; set; }

        [SitecoreField("Column 3 Show More Text")]
        public virtual string Column3ShowMoreText { get; set; }

        [SitecoreField("Column 3 Show More Link")]
        public ContentBase Column3ShowMoreLink { get; set; }

        [SitecoreField("Logout Text")]
        public virtual string LogoutText { get; set; }
    }
}