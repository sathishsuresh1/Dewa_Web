// <copyright file="M160ListContainer.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\hansrajsinh.rathva</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.Content.Models.Base;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using System.Collections.Generic;

    [SitecoreType(TemplateId = "{41AF1759-C807-45C8-BBC2-CEA36FB3959D}", AutoMap = true)]
    public class J160ListContainer
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public virtual string Title { get; set; }

        // <summary>
        /// Gets or sets the sub title.
        /// </summary>
        public virtual string SubTitle { get; set; }

        /// <summary>
        /// Gets or sets the Items.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M160DataItem> Items { get; set; }
    }
    [SitecoreType(TemplateId = "{2483B1AB-FB7A-49BB-8C45-0FD248FA9FE2}", AutoMap = true)]
    public class M160DataItem : ContentBase
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public virtual string Title { get; set; }
        /// <summary>
        /// Gets or sets the Percentage.
        /// </summary>
        public virtual string Percentage { get; set; }
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// Gets or sets the CompanyLogo.
        /// </summary>
        public virtual Image CompanyLogo { get; set; }
        /// <summary>
        /// Gets or sets the Year.
        /// </summary>
        public virtual string Year { get; set; }
        // <summary>
        /// Gets or sets the Items.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M107ItemList> LstItems { get; set; }
    }
}