// <copyright file="M157Container.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.Content.Models.Base;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using System.Collections.Generic;

    [SitecoreType(TemplateId = "{2B5B973F-A39A-464E-AA72-453ED1DFA4DE}", AutoMap = true)]
    public class M157ListContainer
    {
        /// <summary>
        /// Gets or sets the Image.
        /// </summary>
        public virtual Image Image { get; set; }

        /// <summary>
        /// Gets or sets the Items.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M157Container> LstItems { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="M157Container" />.
    /// </summary>
    [SitecoreType(TemplateId = "{DB298F3E-7D5A-40E2-9E9A-1793A633C39C}", AutoMap = true)]
    public class M157Container
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the Items.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M157DataItem> Items { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="M157DataItem" />.
    /// </summary>
    [SitecoreType(TemplateId = "{44897515-E0D1-4114-A3AE-F2750F52D40E}", AutoMap = true)]
    public class M157DataItem : ContentBase
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        public virtual string Value { get; set; }
    }
}
