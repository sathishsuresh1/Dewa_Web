// <copyright file="SmartCommunicationServiceCategory.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    using DEWAXP.Feature.USC.Models;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="SmartCommunicationServiceCategory" />.
    /// </summary>
    [SitecoreType(TemplateId = "{F2343ABF-53FD-4B9E-ADDA-C21DBB6AAC05}", AutoMap = true)]
    public class SmartCommunicationServiceCategory : GlassBase
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the Categories.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<SmartCommunicationServices> Categories { get; set; }
    }
}
