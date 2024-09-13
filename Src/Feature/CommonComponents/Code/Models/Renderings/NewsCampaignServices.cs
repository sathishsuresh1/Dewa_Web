using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Feature.CommonComponents.Models.Renderings.Teasers;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	[SitecoreType(TemplateId = "{CC3C36FB-1AF1-49B5-A92E-F763D2F949D4}", AutoMap = true)]
	public class NewsCampaignServices : ContentBase
	{
        public virtual ArticleListing articlesListing { get; set; }

        [SitecoreField("News Header")]
        public virtual string NewsHeader { get; set; }
        [SitecoreField("News View All Link")]
        public virtual Link NewsViewAllLink { get; set; }
        [SitecoreField("Campaign Header")]
        public virtual string CampaignHeader { get; set; }
        [SitecoreField("Campaign View All Link")]
        public virtual Link CampaignViewAllLink { get; set; }
        [SitecoreField("LatestCampaign")]
        public virtual IEnumerable<PageBase> LatestCampaign { get; set; }
        [SitecoreField("Service Header")]
        public virtual string ServiceHeader { get; set; }
        [SitecoreField("Service View All Link")]
        public virtual Link ServiceViewAllLink { get; set; }
        [SitecoreField("LatestServices")]
        public virtual IEnumerable<PageBase> LatestServices { get; set; }
    }
}