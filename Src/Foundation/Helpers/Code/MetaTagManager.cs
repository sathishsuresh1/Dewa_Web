using Microsoft.Extensions.Primitives;
using Sitecore.Data.Items;
using Sitecore.Data.Serialization;
using System;
using System.Text;
using System.Web;
using System.Web.WebPages;

namespace DEWAXP.Foundation.Helpers.MetaTags
{
    using Extensions;
    /// <summary>
    /// This class manages rendering meta tags based on site specific
    /// configuration and item settings.
    /// </summary>
    public class MetaTagManager
    {
        public Item Item { get; set; }
        public PageMetaDataSettings PageSettings { get; set; }

        public MetaTagManager() : this(Sitecore.Context.Item)
        {
        }

        public MetaTagManager(Item item)
        {
            this.Item = item;
            this.PageSettings = new PageMetaDataSettings(item);
        }

        public string HtmlTitleTag => PageSettings.TitleTag;

        public string MetaDescription => PageSettings.MetaDescription;

        public string MetaKeywords => PageSettings.MetaKeywords;
        public string OgTitle => PageSettings.OgTitle;
        public string OgDescription => PageSettings.OgDescription;
        public string OgImageURL => PageSettings.OgImageURL;



        public string GetMetaTags()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(this.HtmlTitleTag))
            {
                sb.Append(string.Format(@"<title>{0}</title>", this.HtmlTitleTag));
                sb.Append(Environment.NewLine);
            }
            sb.Append($"<meta charset='utf-8' />");
            sb.Append(Environment.NewLine);
            sb.Append($"<meta http-equiv='X-UA-Compatible' content='IE=Edge' />");
            sb.Append(Environment.NewLine);
            sb.Append("<meta name='viewport' content='width=device-width, initial-scale=1.0' />");
            sb.Append(Environment.NewLine);
            sb.Append("<meta name='format-detection' content='telephone=no' />");
            sb.Append(Environment.NewLine);

            if (!string.IsNullOrEmpty(this.MetaDescription))
            {
                sb.Append($"<meta name='description' content='{StringExtensions.HtmlEncodeAndSanitize(this.MetaDescription)}' />");
                sb.Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(this.MetaKeywords))
            {
                sb.Append($"<meta name='keywords' content='{StringExtensions.HtmlEncodeAndSanitize(this.MetaKeywords)}' />");
                sb.Append(Environment.NewLine);
            }

            sb.Append($"<meta name='twitter:card' content='summary'/>"  );
            sb.Append(Environment.NewLine);
            if (!string.IsNullOrEmpty(this.OgTitle))
            {
                sb.Append($"<meta name='twitter:title' content='{StringExtensions.HtmlEncodeAndSanitize(this.OgTitle)}' />");
                sb.Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(this.OgDescription))
            {
                sb.Append($"<meta name='twitter:description' content='{StringExtensions.HtmlEncodeAndSanitize(this.OgDescription)}' />");
                sb.Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(this.OgImageURL))
            {
                sb.Append($"<meta name='twitter:image' content='{StringExtensions.HtmlEncodeAndSanitize(this.OgImageURL)}' />");
                sb.Append(Environment.NewLine);
            }

            sb.Append($"<meta name='og:type' content='article'>");
            sb.Append(Environment.NewLine);
            if (!string.IsNullOrEmpty(this.OgTitle))
            {
                sb.Append($"<meta name='og:title' content='{StringExtensions.HtmlEncodeAndSanitize(this.OgTitle)}' />");
                sb.Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(this.OgDescription))
            {
                sb.Append($"<meta name='og:description' content='{StringExtensions.HtmlEncodeAndSanitize(this.OgDescription)}' />");
                sb.Append(Environment.NewLine);
            }
            if (!string.IsNullOrEmpty(this.OgImageURL))
            {
                sb.Append($"<meta name='og:image' content='{StringExtensions.HtmlEncodeAndSanitize(this.OgImageURL)}' />");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}