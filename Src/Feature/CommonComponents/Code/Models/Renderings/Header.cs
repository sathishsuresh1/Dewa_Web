using DEWAXP.Foundation.Content.Models.Base;
using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration.Attributes;
using Glass.Mapper.Sc.Fields;

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
	public class Header : ContentBase
	{
		[SitecoreField("Government Image")]
		public virtual Image GovernmentImage { get; set; }

		[SitecoreField("Government Link")]
		public virtual Link GovernmentLink { get; set; }

		[SitecoreField("DEWA Image")]
		public virtual Image DewaImage { get; set; }

		[SitecoreField("Dewa Link")]
		public virtual Link DewaLink { get; set; }

		[SitecoreField("Dubai Image")]
		public virtual Image DubaiImage { get; set; }

        [SitecoreField("Dubai Link")]
       public virtual Link DubaiLink { get; set; }

        [SitecoreField("Year of Image")]
        public virtual Image YearofImage { get; set; }

        [SitecoreField("Year of Link")]
       public virtual Link YearofLink { get; set; }

        [SitecoreField("LoginHeading")]
        public virtual string LoginHeading { get; set; }
        [SitecoreField("LoginParagraph")]
        public virtual string LoginParagraph { get; set; }
        [SitecoreField("GooglePlaystore")]
        public virtual Image GooglePlaystore { get; set; }
        [SitecoreField("GooglePlayStoreLink")]
       public virtual Link GooglePlayStoreLink { get; set; }
        [SitecoreField("AppleStore")]
        public virtual Image AppleStore { get; set; }
        [SitecoreField("AppleStoreLink")]
       public virtual Link AppleStoreLink { get; set; }
		[SitecoreField("Enable cookie")]
        public virtual bool Enablecookie { get; set; }
		[SitecoreField("Policy URL")]
		public virtual Link PolicyURL { get; set; }

		[SitecoreField("Display Button")]
		public virtual string DisplayButtonText { get; set; }

		[SitecoreField("Display Message")]
		public virtual string CookieDisplayMessage { get; set; }

		[SitecoreField("Policy Short Text")]
		public virtual string PolicyShortText { get; set; }

	}
}