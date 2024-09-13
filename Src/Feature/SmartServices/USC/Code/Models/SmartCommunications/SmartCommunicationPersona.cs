// <copyright file="SmartCommunicationPersona.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    using DEWAXP.Feature.USC.Models;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SmartCommunicationPersona" />.
    /// </summary>
    [SitecoreType(TemplateId = "{EADE7A66-8E48-4CCA-9CF1-6EEA9E583D4F}", AutoMap = true)]
    public class SmartCommunicationPersona : GlassBase
    {
        /// <summary>
        /// Gets or sets the PersonaName.
        /// </summary>
        [SitecoreField("Name")]
        public virtual string PersonaName { get; set; }

        /// <summary>
        /// Gets or sets the BackgroundImage.
        /// </summary>
        [SitecoreField("Image")]
        public virtual Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the InquiryType.
        /// </summary>
        [SitecoreField("InquiryType")]
        public virtual string InquiryType { get; set; }

        /// <summary>
        /// Gets or sets the CardTarget.
        /// </summary>
        [SitecoreField("CardTarget")]
        public virtual string CardTarget { get; set; }

        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [SitecoreField("Description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the TileCount.
        /// </summary>
        [SitecoreField("Tile Count")]
        public virtual string TileCount { get; set; }

        /// <summary>
        /// Gets or sets the Categories.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<SmartCommunicationServiceCategory> Categories { get; set; }
    }
}
