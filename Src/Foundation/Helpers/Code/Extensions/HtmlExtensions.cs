using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Mvc;
using Sitecore.Resources.Media;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Glass.Mapper.Sc.Fields;
using Sitecore.Links.UrlBuilders;

namespace DEWAXP.Foundation.Helpers.Extensions
{
    public static class HtmlExtensions
    {
        public static HtmlString DynamicPlaceholder(this HtmlHelper helper, string dynamicKey)
        {
            Guid currentRenderingId = helper.Sitecore().CurrentRendering.UniqueId;
            return helper.Sitecore().Placeholder(string.Format("{0}_{1}", dynamicKey, currentRenderingId));
        }

        public static HtmlString GetUrlAttributes(this Link link)
        {
            StringBuilder sb = new StringBuilder();

	        if (link.Target != null)
	        {
		        switch (link.Target.ToUpperInvariant())
		        {
			        case "CUSTOM":
                        sb.Append(string.Format(@" target=""{0}"" data-target=""{1}""", "", link.Target));
				        break;
			        case "ACTIVE BROWSER":
                    case "SELF":
                        sb.Append(string.Format(@" target=""{0}"" data-target=""{1}""", "", link.Target));
				        break;
			        case "NEW BROWSER":
                    case "_BLANK":
                        sb.Append(string.Format(@" target=""{0}"" data-target=""{1}""", "_blank", link.Target));
				        break;
			        default:
                        sb.Append(string.Format(@" target=""{0}"" data-target=""{1}""", "", link.Target));
				        break;
		        }
	        }

	        if (link.Title != null)
            {
                sb.Append(string.Format(@" title=""{0}""", link.Title));
            }

            return new HtmlString(sb.ToString());
        }

        public static HtmlString GetSeoTagsForPage(this HtmlHelper helper, Item item)
        {
            StringBuilder sb = new StringBuilder();
            string canon = @"<link rel="" canonical"" href=""{0}"" />";
            string langAlt = @"<link rel="" alternate"" hreflang=""{0}"" href=""{1}"" />";

            foreach (var lang in item.Languages) {
                var url = LinkManager.GetItemUrl(item, new ItemUrlBuilderOptions
                {
                    LanguageEmbedding = LanguageEmbedding.Always,
                    Language = lang,
                    AlwaysIncludeServerUrl = true,
                    AddAspxExtension = false
                });

                if (lang == item.Language) {
                    sb.AppendLine(string.Format(canon, url));
                    //sb.AppendLine(string.Format(langAlt, "x", url));
                }
                sb.AppendLine(string.Format(langAlt, lang.CultureInfo, url));
            }

            return new HtmlString(sb.ToString());
        }

        public static HtmlString GetLinkUrl(this HtmlHelper helper, Item item, string fieldName)
        {

            if (item != null) {
                LinkField lf = item.Fields[fieldName];

                if(lf != null)
                {
                    var url = string.Empty;

                    switch (lf.LinkType.ToLower())
                    {
                        case "internal":
                            // Use LinkMananger for internal links, if link is not empty
                            url = lf.TargetItem != null ? LinkManager.GetItemUrl(lf.TargetItem) : string.Empty;
                            break;
                        case "media":
                            // Use MediaManager for media links, if link is not empty
                            url = lf.TargetItem != null ? MediaManager.GetMediaUrl(lf.TargetItem) : string.Empty;
                            break;
                        case "external":
                            // Just return external links
                            url = lf.Url;
                            break;
                        case "anchor":
                            // Prefix anchor link with # if link if not empty
                            url = !string.IsNullOrEmpty(lf.Anchor) ? "#" + lf.Anchor : string.Empty;
                            break;
                        case "mailto":
                            // Just return mailto link
                            url = lf.Url;
                            break;
                        case "javascript":
                            // Just return javascript
                            url = lf.Url;
                            break;
                        default:
                            // Just please the compiler, this
                            // condition will never be met
                            url = lf.Url;
                            break;
                    }

                    if (!string.IsNullOrEmpty(lf.QueryString))
                    {
                        if (!string.IsNullOrEmpty(url))
                            url = string.Format("{0}?{1}", url, HttpContext.Current.Server.UrlDecode(lf.QueryString));
                    }

                    return new HtmlString(url);
                }
            }


            return new HtmlString(string.Empty);
        }

        public static HtmlString GetFriendlyDate(this HtmlHelper helper, string dateString) {
            var splitDate = dateString.Split('.');

            if (splitDate.Length == 3)
            {
                var date = new DateTime(int.Parse(splitDate[2]), int.Parse(splitDate[1]), int.Parse(splitDate[0]));

                return new HtmlString(date.ToString("dd-MMM-yyyy", global::Sitecore.Context.Culture));
            }

            return new HtmlString("");
        }

    }
}
