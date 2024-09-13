// <copyright file="RadioButtoncomponent.cs">
// Copyright (c) 2019
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using Glass.Mapper.Sc.Configuration;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="RadioButtoncomponent" />
    /// </summary>
    [SitecoreType(TemplateName = "Radio Landing Page Container", TemplateId = "{618F2900-FC72-4B01-BBB0-A7AB7A323F49}", AutoMap = true)]
    public class RadioButtoncomponent
    {
        /// <summary>
        /// Gets or sets the Children
        /// </summary>
        public virtual IEnumerable<RadioButtoncomponentItem> Children { get; set; }

        /// <summary>
        /// Gets or sets the ButtonText
        /// </summary>
        [SitecoreField("Button Text")]
        public virtual string ButtonText { get; set; }

        /// <summary>
        /// Gets or sets the datasourceid
        /// </summary>
        public virtual string datasourceid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether IsLoggedIn
        /// </summary>
        public virtual bool IsLoggedIn { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="RadioButtoncomponentItem" />
    /// </summary>
    [SitecoreType(TemplateName = "Radio Landing Page Items", TemplateId = "{F9DBBC8E-AF9D-4228-A654-88DDC1A53F57}", AutoMap = true)]
    public class RadioButtoncomponentItem
    {
        /// <summary>
        /// Gets or sets the Title
        /// </summary>
        [SitecoreField("Title")]
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the Description
        /// </summary>
        [SitecoreField("Description")]
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the URLLink
        /// </summary>
        [SitecoreField("URLLink")]
        public virtual Link URLLink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether Hideifloggedin
        /// </summary>
        [SitecoreField("Hide if loggedin")]
        public virtual bool Hideifloggedin { get; set; }
    }
}
