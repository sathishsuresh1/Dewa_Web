using System;
using System.Collections.Generic;
using DEWAXP.Foundation.ORM.Models.Outage;
using DEWAXP.Foundation.ORM.Models.Sitemap;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;
using Sitecore.Data.Items;

namespace DEWAXP.Foundation.ORM.Models
{
	public class PageBase : ContentBase, ISitemap
    {
        [SitecoreField("Header")]
		public virtual string Header { get; set; }

        [SitecoreField("Redirect Text")]
        public virtual string RedirectText { get; set; }

		[SitecoreField("Redirect Link")]
        public virtual Item RedirectLink { get; set; }

		public virtual string Subheader { get; set; }

		[SitecoreField("Publish Date")]
		public virtual DateTime PublishDate { get; set; }

		public virtual string Summary { get; set; }

		[SitecoreField("Teaser Image")]
		public virtual Image Image { get; set; }

        public virtual bool HideFromSitemapPage { get; set; }

		[SitecoreField("Show Share Link")]
		public virtual bool ShowShareLink { get; set; }

        [SitecoreField("Uses Customer Web services")]
        public virtual bool UsesCustomerWebservices { get; set; }

        [SitecoreField("Uses Smart Vendor web services")]
        public virtual bool UsesSmartVendorwebservices { get; set; }

        [SitecoreField("Uses Tendor web services")]
        public virtual bool UsesTendorwebservices { get; set; }

        [SitecoreField("Uses Documentum web services")]
        public virtual bool UsesDocumentumwebservices { get; set; }

        [SitecoreField("Uses Eform web services")]
        public virtual bool UsesEformwebservices { get; set; }

        [SitecoreField(FieldName = "WebService")]
        public virtual IEnumerable<OutageItem> WebService { get; set; }
    }
}