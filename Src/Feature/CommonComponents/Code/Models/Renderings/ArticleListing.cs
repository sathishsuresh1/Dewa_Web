using Glass.Mapper.Sc.Configuration.Attributes;
using System.Collections.Generic;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	//[SitecoreType(TemplateId = "{78B5FC36-A966-4B30-B9E5-E5B3301A4948}", AutoMap = true)]
	public class ArticleListing : Listing
	{
        /// <summary>
        /// Feature Html
        /// </summary>
        [SitecoreField("Feature Html")]
        public virtual string FeatureHtml { get; set; }

		public virtual IEnumerable<Article> Children { get; set; }
	}
}