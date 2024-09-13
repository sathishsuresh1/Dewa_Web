// <copyright file="SmartCommunicationFolder.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    using DEWAXP.Feature.USC.Models;
    using DEWAXP.Foundation.ORM.Models;
    using Glass.Mapper.Sc.Configuration.Attributes;
    using Glass.Mapper.Sc.Fields;

    /// <summary>
    /// Defines the <see cref="SmartCommunicationSettings" />.
    /// </summary>
    [SitecoreType(TemplateId = "{3ADF3EEE-C57A-4F3E-88D0-D7A536E1F934}", AutoMap = true)]
    public class SmartCommunicationSettings : GlassBase
    {
        /// <summary>
        /// Gets or sets the Schedule_a_call_Email.
        /// </summary>
        [SitecoreField("Schedule a call Email")]
        public virtual string Schedule_a_call_Email { get; set; }

        /// <summary>
        /// Gets or sets the Consumer_Inquiry.
        /// </summary>
        [SitecoreField("Consumer Inquiry")]
        public virtual string Consumer_Inquiry { get; set; }

        /// <summary>
        /// Gets or sets the Builder_Inquiry.
        /// </summary>
        [SitecoreField("Builder Inquiry")]
        public virtual string Builder_Inquiry { get; set; }

        /// <summary>
        /// Gets or sets the Visitor_Inquiry.
        /// </summary>
        [SitecoreField("Visitor Inquiry")]
        public virtual string Visitor_Inquiry { get; set; }

        /// <summary>
        /// Gets or sets the Electricity_Service_Email.
        /// </summary>
        [SitecoreField("Electricity Service Email")]
        public virtual string Electricity_Service_Email { get; set; }

        /// <summary>
        /// Gets or sets the Water_Service_Email.
        /// </summary>
        [SitecoreField("Water Service Email")]
        public virtual string Water_Service_Email { get; set; }

        /// <summary>
        /// Gets or sets the Infrastructure_NOC_Email.
        /// </summary>
        [SitecoreField("Infrastructure NOC Email")]
        public virtual string Infrastructure_NOC_Email { get; set; }

        /// <summary>
        /// Gets or sets the Builder_Template.
        /// </summary>
        [SitecoreField("Builder Template")]
        public virtual string Builder_Template { get; set; }

        /// <summary>
        /// Gets or sets the Consumer_Template.
        /// </summary>
        [SitecoreField("Consumer Template")]
        public virtual string Consumer_Template { get; set; }

        /// <summary>
        /// Gets or sets the Schedule_a_call_Template.
        /// </summary>
        [SitecoreField("Schedule a call Template")]
        public virtual string Schedule_a_call_Template { get; set; }

        /// <summary>
        /// Gets or sets the Inquiry_Form_Subject.
        /// </summary>
        [SitecoreField("Inquiry Form subject")]
        public virtual string Inquiry_Form_Subject { get; set; }

        /// <summary>
        /// Gets or sets the General_Inquiry_Template.
        /// </summary>
        [SitecoreField("General Inquiry Template")]
        public virtual string General_Inquiry_Template { get; set; }

        /// <summary>
        /// Gets or sets the FromEmail.
        /// </summary>
        [SitecoreField("From Email")]
        public virtual string FromEmail { get; set; }

        /// <summary>
        /// Gets or sets the Finance Email.
        /// </summary>
        [SitecoreField("HR Email")]
        public virtual string HREmail { get; set; }

        /// <summary>
        /// Gets or sets the Finance Email.
        /// </summary>
        [SitecoreField("Finance Email")]
        public virtual string FinanceEmail { get; set; }

        /// <summary>
        /// Gets or sets the Legal Affairs Email.
        /// </summary>
        [SitecoreField("Legal Affairs Email")]
        public virtual string LegalAffairsEmail { get; set; }

        /// <summary>
        /// Gets or sets the Contracts  Email.
        /// </summary>
        [SitecoreField("Contracts Email")]
        public virtual string ContractsEmail { get; set; }

        /// <summary>
        /// Gets or sets the Local Purchase Email.
        /// </summary>
        [SitecoreField("Local Purchase Email")]
        public virtual string LocalPurchaseEmail { get; set; }

        /// <summary>
        /// Gets or sets the Local Purchase Email.
        /// </summary>
        [SitecoreField("DEWA Academy Email")]
        public virtual string DEWAAcademyEmail { get; set; }

        /// <summary>
        /// Gets or sets the Media Center Email.
        /// </summary>
        [SitecoreField("Media Queries Email")]
        public virtual string MediaCenterEmail { get; set; }

        /// <summary>
        /// Gets or sets the Benchmarking Request Email.
        /// </summary>
        [SitecoreField("Benchmarking Email")]
        public virtual string BenchmarkingRequestEmail { get; set; }

        /// <summary>
        /// Gets or sets the HR Email.
        /// </summary>
        [SitecoreField("POD Email")]
        public virtual string PODEmail { get; set; }

        /// <summary>
        /// Gets or sets the Consumption Verification link.
        /// </summary>
        [SitecoreField("Consumption Verification Link")]
        public virtual Link ConsumptionVerificationLink { get; set; }

    }
}
