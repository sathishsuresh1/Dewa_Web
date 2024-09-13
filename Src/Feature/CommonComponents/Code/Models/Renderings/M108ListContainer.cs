// <copyright file="M107Container.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\hansrajsinh.rathva</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System.Collections.Generic;
    using Glass.Mapper.Sc.Fields;

    /// <summary>
    /// Defines the <see cref="M107ListContainer" />.
    /// </summary>
    [SitecoreType(TemplateId = "{B2D1F1AC-637F-43F5-9806-338B18ABAE40}", AutoMap = true)]
    public class M108ListContainer 
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [SitecoreField("Subject")]
        public virtual string Subject { get; set; }
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        [SitecoreField("Description")]
        public virtual string Description { get; set; }
        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        [SitecoreField("Location")]
        public virtual string Location { get; set; }
        /// <summary>
        /// Gets or sets the StartDate.
        /// </summary>
        [SitecoreField("StartDate")]
        public virtual System.DateTime StartDate { get; set; }
        /// <summary>
        /// Gets or sets the EndDate.
        /// </summary>
        [SitecoreField("EndDate")]
        public virtual System.DateTime EndDate { get; set; }
        /// <summary>
        /// Gets or sets the RenderLinkItems.
        /// </summary>
        [SitecoreField("RenderLinkItems")]
        public virtual IEnumerable<ATCItems> RenderLinkItems { get; set; }
    }
    [SitecoreType(TemplateId = "{5E261110-6EEB-4650-9D8D-E9312A7A8ED4}", AutoMap = true)]
    public class ATCItems
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [SitecoreField("LinkImage")]
        public virtual Image LinkImage { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether ICS Type .
        /// </summary>
        [SitecoreField("ICSType")]
        public virtual bool ICSType { get; set; }
    }
}