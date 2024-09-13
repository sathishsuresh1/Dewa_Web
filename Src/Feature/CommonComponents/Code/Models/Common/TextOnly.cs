// <copyright file="TextOnly.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Common
{
    using Glass.Mapper.Sc.Configuration.Attributes;

    /// <summary>
    /// Defines the <see cref="TextOnly" />.
    /// </summary>
    [SitecoreType(TemplateId = "{ABF310D1-DCBC-481B-B052-6C9D032B880A}", AutoMap = true)]
    public class TextOnly
    {
        /// <summary>
        /// Gets or sets the Text.
        /// </summary>
        [SitecoreField("Text")]
        public virtual string Text { get; set; }
    }
}