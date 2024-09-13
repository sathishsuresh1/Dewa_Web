// <copyright file="SmartCommunicationServices.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    using DEWAXP.Feature.USC.Models;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    /// <summary>
    /// Defines the <see cref="SmartCommunicationServices" />.
    /// </summary>
    [SitecoreType(TemplateId = "{BE5FD49F-B2CD-4513-BA56-028405F1AA00}", AutoMap = true)]
    public class SmartCommunicationServices : GlassBase
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the BackgroundImage.
        /// </summary>
        [SitecoreField("Image")]
        public virtual Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether RedirectLink.
        /// </summary>
        [SitecoreField("RedirectLink")]
        public virtual bool RedirectLink { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        [SitecoreField("URL")]
        public virtual Link URL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsCardType.
        /// </summary>
        [SitecoreField("IsCardType")]
        public virtual bool IsCardType { get; set; }

        /// <summary>
        /// Gets or sets the CardTarget.
        /// </summary>
        [SitecoreField("CardTarget")]
        public virtual string CardTarget { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether InquiryType .
        /// </summary>
        [SitecoreField("InquiryType")]
        public virtual string InquiryType { get; set; }

        /// <summary>
        /// Gets or sets a value service code .
        /// </summary>
        [SitecoreField("ServiceCode")]
        public virtual string ServiceCode { get; set; }
    }
}
