// <copyright file="FormattedText - Copy.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;

    /// <summary>
    /// Defines the <see cref="MultiLineText" />
    /// </summary>
    [SitecoreType(TemplateId = "{1B02054E-B87D-40AC-9120-794F634AAC10}", AutoMap = true)]
    public class MultiLineText:GlassBase
    {
        /// <summary>
        /// Gets or sets the MultiLineText
        /// </summary>
        [SitecoreField("Multi Line")]
        public virtual string MultiLineTextField { get; set; }
    }
}
