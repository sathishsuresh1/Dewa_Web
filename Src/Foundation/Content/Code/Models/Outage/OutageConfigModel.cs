using DEWAXP.Foundation.ORM.Models.Outage;
using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.Outage
{
    [SitecoreType(TemplateId = "{ED577A0C-FCCD-4737-8CFC-8A7E96025949}", AutoMap = true)]
    public class OutageConfigModel
    {
        [SitecoreField(FieldName = "Turn on outage redirect")]
        public virtual bool TurnOnOutage { get; set; }

        [SitecoreField(FieldName = "Webservice Outage")]
        public virtual IEnumerable<OutageItem> WebserviceOutage { get; set; }

        [SitecoreField(FieldName = "CStartDate")]
        public virtual DateTime? CStartDate { get; set; }

        [SitecoreField(FieldName = "CEndDate")]
        public virtual DateTime? CEndDate { get; set; }

        [SitecoreField(FieldName = "SVStartDate")]
        public virtual DateTime? SVStartDate { get; set; }

        [SitecoreField(FieldName = "SVEndDate")]
        public virtual DateTime? SVEndDate { get; set; }

        [SitecoreField(FieldName = "TStartDate")]
        public virtual DateTime? TStartDate { get; set; }

        [SitecoreField(FieldName = "TEndDate")]
        public virtual DateTime? TEndDate { get; set; }

        [SitecoreField(FieldName = "DStartDate")]
        public virtual DateTime? DStartDate { get; set; }

        [SitecoreField(FieldName = "DEndDate")]
        public virtual DateTime? DEndDate { get; set; }

        [SitecoreField(FieldName = "EStartDate")]
        public virtual DateTime? EStartDate { get; set; }

        [SitecoreField(FieldName = "EEndDate")]
        public virtual DateTime? EEndDate { get; set; }
    }
}