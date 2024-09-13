using DEWAXP.Foundation.ORM.Models;
using Glass.Mapper.Sc.Configuration;
using Glass.Mapper.Sc.Configuration.Attributes;
using Sitecore.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer
{

    [SitecoreType(TemplateId = "{5B9BE67C-BB20-4CDE-8C04-FAD2179B6A59}", AutoMap = true)]
    public class SectionList : GlassBase
    {
        [SitecoreField(FieldName = "Section List")]
        public virtual IEnumerable<Section> Sections { get; set; }

        public virtual Section SelectedSection { get; set; }
        public virtual int TotalStep { get; set; }
        public virtual int CurrentStep { get; set; }

        public virtual List<string> StoredAnsList { get; set; }
        public virtual int isProgress { get; set; } 
        public virtual string isSaveExist { get; set; }
        public virtual bool IsStepComplete { get; set; }
    }

    [SitecoreType(TemplateId = "{06183D04-97AF-4FF4-9C28-052DC3C2A746}", AutoMap = true)]
    public class Section : GlassBase
    {
        public virtual int ItemIndex { get; set; }

        [SitecoreField(FieldName = "SectionTitle")]
        public virtual string SectionTitle { get; set; }

        [SitecoreField(FieldName = "Complete")]
        public virtual bool Complete { get; set; }

        [SitecoreChildren]
        public virtual IEnumerable<Questions> QuestionsList { get; set; }
    }

    [SitecoreType(TemplateId = "{A47BED1F-422D-4DEC-B16C-0CDF1B7F0141}", AutoMap = true, EnforceTemplate = SitecoreEnforceTemplate.Template)]
    public class SurveyResponse : GlassBase
    {
        [SitecoreField(FieldName = "Correct")]
        public virtual string Correct { get; set; }

        [SitecoreField(FieldName = "Wrong")]
        public virtual string Wrong { get; set; }

        [SitecoreField(FieldName = "IsFirstAttemptCompleted")]
        public virtual bool IsFirstAttemptCompleted { get; set; }

        [SitecoreField(FieldName = "IsSecondAttemptCompleted")]
        public virtual bool IsSecondAttemptCompleted { get; set; }

        [SitecoreField(FieldName = "Progress")]
        public virtual int Progress { get; set; }

        [SitecoreField(FieldName = "Marks")]
        public virtual int Marks { get; set; }
        [SitecoreChildren]
        public virtual IEnumerable<SectionResponse> SectionResponses { get; set; }

        public virtual string Assessment { get; set; }
    }

    [SitecoreType(TemplateId = "{7BE8E562-A25E-4AA3-96DA-F474BD06236A}", AutoMap = true)]
    public class SectionResponse : GlassBase
    {
        [SitecoreField(FieldName = "Section Id")]
        public virtual string SectionID { get; set; }

        [SitecoreField(FieldName = "Complete")]
        public virtual bool IsCompleted { get; set; }

        [SitecoreField(FieldName = "User Response")]
        public virtual System.Collections.Specialized.NameValueCollection NameValue { get; set; }

    }



}