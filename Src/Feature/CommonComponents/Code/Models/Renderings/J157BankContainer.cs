// <copyright file="J157BankContainer.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.CommonComponents.Models.Renderings
{
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the <see cref="J157BankContainer" />.
    /// </summary>
    [SitecoreType(TemplateId = "{470A19D9-EFA4-4118-88A1-75B1EE77B726}", AutoMap = true)]
    public class J157BankContainer
    {
        /// <summary>
        /// Gets or sets the Description.
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Gets or sets the BankList.
        /// </summary>
        [SitecoreChildren]
        public virtual IEnumerable<J157BankItem> BankList { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="J157BankItem" />.
    /// </summary>
    [SitecoreType(TemplateId = "{7DAEF890-FCAC-4C55-ADB7-2C2CE9717E78}", AutoMap = true)]
    public class J157BankItem
    {
        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        public virtual string Title { get; set; }

        /// <summary>
        /// Gets or sets the Pobox.
        /// </summary>
        public virtual string Pobox { get; set; }

        /// <summary>
        /// Gets or sets the Fax.
        /// </summary>
        public virtual string Fax { get; set; }

        /// <summary>
        /// Gets or sets the Telephone.
        /// </summary>
        public virtual string Telephone { get; set; }

        /// <summary>
        /// Gets or sets the Email.
        /// </summary>
        public virtual string Email { get; set; }

        /// <summary>
        /// Gets or sets the BankName.
        /// </summary>
        [SitecoreField("Bank Name")]
        public virtual string BankName { get; set; }

        /// <summary>
        /// Gets or sets the BankAddress.
        /// </summary>
        [SitecoreField("Bank Address")]
        public virtual string BankAddress { get; set; }

        /// <summary>
        /// Gets or sets the BackgroundImage.
        /// </summary>
        [SitecoreField("Background Image")]
        public virtual Image BackgroundImage { get; set; }

        /// <summary>
        /// Gets or sets the Image.
        /// </summary>
        public virtual Image Image { get; set; }
        [SitecoreField("Website")]
        public virtual Link Website { get; set; }
        [SitecoreField("LeadingBank")]
        public virtual bool LeadingBank { get; set; }
        [SitecoreField("Leading Bank Text")]
        public virtual string LeadingBankText { get; set; }
    }
}
