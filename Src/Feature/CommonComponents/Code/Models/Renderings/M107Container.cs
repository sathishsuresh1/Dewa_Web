// <copyright file="M107Container.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\hansrajsinh.rathva</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using DEWAXP.Foundation.Content.Models.Base;
    using DEWAXP.Feature.CommonComponents.Models.Common;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="M107ListContainer" />.
    /// </summary>
    [SitecoreType(TemplateId = "{8F510B5A-1C2C-450D-AEA8-74F6E404559E}", AutoMap = true)]
    public class M107ListContainer
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
        /// Gets or sets the GridCol.
        /// </summary>
        [SitecoreField("Grid Col")]
        public virtual TextOnly GridCol { get; set; }

        /// <summary>
        /// Gets or sets the BarClass.
        /// </summary>
        [SitecoreField("Bar Class")]
        public virtual TextOnly BarClass { get; set; }

        

        /// <summary>
        /// Gets or sets the LstItems.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M107ItemContainer> LstItems { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="M107ItemContainer" />.
    /// </summary>
    [SitecoreType(TemplateId = "{4FE05F97-D0D1-49BC-BE1E-4C1ACC45CECD}", AutoMap = true)]
    public class M107ItemContainer
    {
        /// <summary>
        /// Gets or sets the Heading.
        /// </summary>
        [SitecoreField("Heading")]
        public virtual string Heading { get; set; }

        /// <summary>
        /// Gets or sets the LstItems.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M107ItemList> LstItems { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="M107ItemList" />.
    /// </summary>
    [SitecoreType(TemplateId = "{97771E44-BECA-4AAB-A79A-A8110281DBD0}", AutoMap = true)]
    public class M107ItemList
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
        /// Gets or sets the ChartType.
        /// </summary>
        [SitecoreField("Chart Type")]
        public virtual TextOnly ChartType { get; set; }
        /// <summary>
        /// Gets or sets the DataCategory.
        /// </summary>
        [SitecoreField("Data Category")]
        public virtual TextOnly DataCategory { get; set; }

        /// <summary>
        /// Gets or sets the BarItems.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<M107DataItem> BarItems { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="M107DataItem" />.
    /// </summary>
    [SitecoreType(TemplateId = "{887FAD91-BA87-4976-A2C1-813B045B5124}", AutoMap = true)]
    public class M107DataItem : ContentBase
    {
        /// <summary>
        /// Gets or sets the BarName.
        /// </summary>
        [SitecoreField("BarName")]
        public virtual string BarName { get; set; }

        /// <summary>
        /// Gets or sets the Color.
        /// </summary>
        [SitecoreField("Color")]
        public virtual string Color { get; set; }

        /// <summary>
        /// Gets or sets the Value.
        /// </summary>
        [SitecoreField("Value")]
        public virtual string Value { get; set; }

        /// <summary>
        /// Gets or sets the TargetColor.
        /// </summary>
        [SitecoreField("Target Color")]
        public virtual string TargetColor { get; set; }

        /// <summary>
        /// Gets or sets the TargetValue.
        /// </summary>
        [SitecoreField("Target Value")]
        public virtual string TargetValue { get; set; }
    }
}
