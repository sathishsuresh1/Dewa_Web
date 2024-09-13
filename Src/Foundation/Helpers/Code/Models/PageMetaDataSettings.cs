using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace DEWAXP.Foundation.Helpers.MetaTags
{
    /// <summary>
    /// Represents the Meta Data Base Date Template
    /// </summary>
    public class PageMetaDataSettings
    {
        public PageMetaDataSettings() : this(Sitecore.Context.Item)
        {
        }

        public PageMetaDataSettings(Item item)
        {
            if (item.HasField("Browser Title"))
            {
                this.TitleTag = item.Fields["Browser Title"].Value;
            }
            if (item.HasField("Meta Description"))
            {
                this.MetaDescription = item.Fields["Meta Description"].Value;
            }
            if (item.HasField("Meta Keywords"))
            {
                this.MetaKeywords = item.Fields["Meta Keywords"].Value;
            }
            if (item.HasField("Og Title"))
            {
                this.OgTitle = !string.IsNullOrWhiteSpace(item.Fields["Og Title"].Value)? item.Fields["Og Title"].Value : this.TitleTag;
            }
            if (item.HasField("Og Description"))
            {
                this.OgDescription = !string.IsNullOrWhiteSpace(item.Fields["Og Description"].Value) ? item.Fields["Og Description"].Value : this.MetaDescription;
            }
            if (item.HasField("Og Image") && ((ImageField)item.Fields["Og Image"]).MediaItem != null)
            {
                this.OgImageURL = ((Sitecore.Data.Fields.ImageField)item.Fields["Og Image"]).MediaItem.GetFullyQualifiedMediaUrl();
                if(string.IsNullOrWhiteSpace(this.OgImageURL))
                {
                    if (item.HasField("Teaser Image") && ((ImageField)item.Fields["Teaser Image"]).MediaItem != null)
                    {
                        this.OgImageURL = ((Sitecore.Data.Fields.ImageField)item.Fields["Teaser Image"]).MediaItem.GetFullyQualifiedMediaUrl();
                    }
                }
            }
        }

        public string TitleTag { get; set; }
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string OgTitle { get; set; }
        public string OgDescription { get; set; }
        public string OgImageURL { get; set; }
    }

    public interface ISiteSettings
    {
        void Load(Item item);
    }
}