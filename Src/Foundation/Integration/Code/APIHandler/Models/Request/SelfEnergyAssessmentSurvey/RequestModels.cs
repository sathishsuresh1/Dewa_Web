using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.SelfEnergyAssessmentSurvey
{
    public class BaseRequest
    {
        public SurveyinputBase surveyinputs { get; set; }
    }
    public class SurveyinputBase
    {
        public string sessionid { get; set; }
        public string contractaccount { get; set; }
        public string survey { get { return "SNS"; } }
        public string lang { get; set; }
        public string vendorid { get; set; }
        public string mobileosversion { get; set; }
        public string appversion { get; set; }
        public string appidentifier { get; set; }
        public string userid { get; set; }

    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SurveyinputSubmit : SurveyinputBase
    {
        public string action { get; set; }

    }

    public class Answerdetaillist
    {
        public string answernumber { get; set; }
        public string answertext { get; set; }
    }

    public class Answerlist
    {
        public string questionnumber { get; set; }
        public List<Answerdetaillist> answerdetaillist { get; set; }
        public string section { get; set; }
        public string subsection { get; set; }
    }

    public class SubmitRequest
    {
        public SurveyinputSubmit surveyinputs { get; set; }
        public List<Answerlist> answerlist { get; set; }
    }

    #region Download Survey 
    public class DownloadSurveyInput : SurveyinputSubmit
    {
        public string version { get; set; }
    }
    public class SubmitSurveyRequest
    {
        public DownloadSurveyInput surveyinputs { get; set; }        
    }

    #endregion

}
