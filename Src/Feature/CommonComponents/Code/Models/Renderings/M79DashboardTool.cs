// <copyright file="M79DashboardTool.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    /// <summary>
    /// Defines the <see cref="M79DashboardTool" />.
    /// </summary>
    [SitecoreType(TemplateId = "{F1F30DB0-8CC4-4F2B-ABCC-9756D25B21C3}", AutoMap = true)]
    public class M79DashboardTool : GlassBase
    {
        /// <summary>
        /// Gets or sets the BackgroundImage
        /// </summary>
        [SitecoreField("Button Link")]
        public virtual Link ButtonLink { get; set; }

        public virtual string FullName { get; set; }
    }
}
