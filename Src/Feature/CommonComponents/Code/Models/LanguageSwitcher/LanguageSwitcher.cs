using Sitecore.Globalization;

namespace DEWAXP.Feature.CommonComponents.Models.LanguageSwitcher
{
	public class LanguageSwitcher
	{
		public LanguageSwitcher(Language currentLanguage, Language otherLanguage, 
            bool itemHasOtherLanguage, string otherLanguageUrl,string ptabindex)
		{
			ItemHasOtherLanguage = itemHasOtherLanguage;
			OtherLanguage = otherLanguage;
			CurrentLanguage = currentLanguage;
            OtherLanguageUrl = otherLanguageUrl;
		    tabindex = ptabindex;
		}

		public virtual Language CurrentLanguage { get; private set; }

		public virtual Language OtherLanguage { get; private set; }

		public virtual bool ItemHasOtherLanguage { get; private set; }

        public virtual string OtherLanguageUrl { get; private set; }
        public virtual string tabindex { get; set; }

        public virtual int SwitcherType { get; set; }
             
	}
}