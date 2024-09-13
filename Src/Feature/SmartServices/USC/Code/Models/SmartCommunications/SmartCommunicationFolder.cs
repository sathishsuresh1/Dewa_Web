// <copyright file="SmartCommunicationFolder.cs">
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
    /// Defines the <see cref="SmartCommunicationFolder" />.
    /// </summary>
    [SitecoreType(TemplateId = "{A8D0D487-6C02-4EC8-A81F-5D2310EE4896}", AutoMap = true)]
    public class SmartCommunicationFolder : GlassBase
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the SubTitle.
        /// </summary>
        [SitecoreField("SubTitle")]
        public virtual string SubTitle { get; set; }

        /// <summary>
        /// Gets or sets the Personas.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<SmartCommunicationPersona> Personas { get; set; }
    }
}
