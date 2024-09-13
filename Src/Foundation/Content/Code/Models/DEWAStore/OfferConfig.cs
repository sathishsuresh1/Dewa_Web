// <copyright file="OfferConfig.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Foundation.Content.Models.DewaStore
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    /// <summary>
    /// Defines the <see cref="OfferConfig" />.
    /// </summary>
    [SitecoreType(TemplateName = "Offer Config", TemplateId = "{BE68D56F-304E-45C6-941A-AFB43BCB74AB}", AutoMap = true)]
    public class OfferConfig :GlassBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether IsHotOffer.
        /// </summary>
        [SitecoreField("IsHotOffer")]
        public virtual bool IsHotOffer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsFeaturedOffer.
        /// </summary>
        [SitecoreField("IsFeaturedOffer")]
        public virtual bool IsFeaturedOffer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsNewOffer.
        /// </summary>
        [SitecoreField("IsNewOffer")]
        public virtual bool IsNewOffer { get; set; }

        /// <summary>
        /// Gets or sets the CompanyUno.
        /// </summary>
        [SitecoreField("CompanyUno")]
        public virtual string CompanyUno { get; set; }

        /// <summary>
        /// Gets or sets the CategoryUno.
        /// </summary>
        [SitecoreField("CategoryUno")]
        public virtual string CategoryUno { get; set; }

        /// <summary>
        /// Gets or sets the PageSize.
        /// </summary>
        [SitecoreField("PageSize")]
        public virtual string PageSize { get; set; }

        /// <summary>
        /// Gets or sets the PageNumber.
        /// </summary>
        [SitecoreField("PageNumber")]
        public virtual string PageNumber { get; set; }

        /// <summary>
        /// Gets or sets the Condition.
        /// </summary>
        [SitecoreField("Condition")]
        public virtual string Condition { get; set; }

        /// <summary>
        /// Gets or sets the BackgroundImage.
        /// </summary>
        [SitecoreField("BackgroundImage")]
        public virtual Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the ModuleTitle.
        /// </summary>
        [SitecoreField("Module Title")]
        public virtual string ModuleTitle { get; set; }

        /// <summary>
        /// Gets or sets the ModuleLink.
        /// </summary>
        [SitecoreField("Module Link")]
        public virtual Link ModuleLink { get; set; }

        [SitecoreField("Module Parameter")]
        public virtual DEWAStoreParameter ModuleParameter { get; set; }


        public string account_number { get; set; }
        public bool IsDewastoreAllowed { get; set; }
    }

    [SitecoreType(TemplateName = "DEWA Store Global Config", TemplateId = "{14A33748-2340-4E03-9745-34FDA8A12074}", AutoMap = true)]
    public class DEWAStoreGlobalConfig : GlassBase
    {
        /// <summary>
        /// Gets or sets the CompanyUno.
        /// </summary>
        [SitecoreField("Offers URL")]
        public virtual string OffersURL { get; set; }

        /// <summary>
        /// Gets or sets the CategoryUno.
        /// </summary>
        [SitecoreField("Logo URL")]
        public virtual string LogoURL { get; set; }

        /// <summary>
        /// Gets or sets the PageSize.
        /// </summary>
        [SitecoreField("Thumbnails URL")]
        public virtual string ThumbnailsURL { get; set; }
    }

    [SitecoreType(TemplateName = "Dewa Store Parameter", TemplateId = "{B8A5FFDF-AE96-4681-A6C5-B623ED9FB3DC}", AutoMap = true)]
    public class DEWAStoreParameter : GlassBase
    {
        /// <summary>
        /// Gets or sets the .
        /// </summary>
        [SitecoreField("Text")]
        public virtual string Text { get; set; }

    }
}
