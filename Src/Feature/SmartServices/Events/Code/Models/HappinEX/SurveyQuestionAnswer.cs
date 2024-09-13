using Glass.Mapper.Sc.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Events.Models.HappinEX
{
    //[SitecoreType(TemplateName = "Basic Survey", TemplateId = "{0379D918-21CA-43B2-81D7-22BF33B0DE33}", AutoMap = true)]
    public class SurveyQuestionAnswer
    {
        public SurveyQuestion MainQuestion { get; set; }
        public List<string> SubQuestions { get; set; }
        public string Comments { get; set; }
    }
}