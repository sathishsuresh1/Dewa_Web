// <copyright file="EVTransactionsConfig.cs">
// Copyright (c) 2021
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.EV.Models.EVDashboard
{
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;

    /// <summary>
    /// Defines the <see cref="EVTransactionsConfig" />.
    /// </summary>
    [SitecoreType(TemplateName = "EVTransactionConfig", TemplateId = "{ED445532-3D40-4DC9-A52A-1E370011AE70}", AutoMap = true)]
    public class EVTransactionsConfig : GlassBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether Dashboard.
        /// </summary>
        [SitecoreField("Dashboard")]
        public virtual bool Dashboard { get; set; }

        /// <summary>
        /// Gets or sets the NumberofRecords.
        /// </summary>
        [SitecoreField("NumberofRecords")]
        public virtual string NumberofRecords { get; set; }
    }

    public class EVTransactionCache
    {
        public string accountnumber { get; set; }
        public string cardid { get; set; }
        public string month { get; set; }
        public string sortby { get; set; }
    }
}
