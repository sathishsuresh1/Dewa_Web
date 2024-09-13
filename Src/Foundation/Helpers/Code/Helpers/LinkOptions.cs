using Sitecore.Links;
using Sitecore.Links.UrlBuilders;

namespace DEWAXP.Foundation.Helpers
{
	public static class LinkOptions
	{
		public static ItemUrlBuilderOptions Url
		{
			get
			{
				return new ItemUrlBuilderOptions
                {
					AddAspxExtension = false,
					LanguageEmbedding = LanguageEmbedding.Always
				};
			}
		}
	}
}