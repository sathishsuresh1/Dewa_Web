using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.SelfEnergyAssessmentSurvey
{
    public class QuestionsAndAnswersResponse : BaseResponse
    {

        [JsonProperty("dynamicquestionlist")]
        public List<Questionlist> Dynamicquestionlist { get; set; }


        [JsonProperty("sectionlist")]
        public List<Sectionlist> Sectionlist { get; set; }
    }

    public class BaseResponse
    {
        [JsonProperty("description")]
        public object Description { get; set; }
        [JsonProperty("responsecode")]
        public string Responsecode { get; set; }
    }

    public class Answerlist
    {
        [JsonProperty("answernumber")]
        public string Answernumber { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imgurl")]
        public string Imgurl { get; set; }

        [JsonProperty("tipurl")]
        public object Tipurl { get; set; }

        [JsonProperty("dynamicquestionlist")]
        public List<string> Dynamicquestionlist { get; set; }
    }

    /*
    public class Dynamicquestionlist
    {
        [JsonProperty("answerlist")]
        public List<Answerlist> Answerlist { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("interval")]
        public string Interval { get; set; }

        [JsonProperty("maximum")]
        public string Maximum { get; set; }

        [JsonProperty("minimum")]
        public string Minimum { get; set; }

        [JsonProperty("questionnumber")]
        public string Questionnumber { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("unit")]
        public object Unit { get; set; }
    }
    */
    public class Questionlist
    {
        [JsonProperty("answerlist")]
        public List<Answerlist> Answerlist { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("interval")]
        public string Interval { get; set; }

        [JsonProperty("maximum")]
        public string Maximum { get; set; }

        [JsonProperty("minimum")]
        public string Minimum { get; set; }

        [JsonProperty("questionnumber")]
        public string Questionnumber { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("unit")]
        public object Unit { get; set; }
    }

    public class Subsectionlist
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("questionlist")]
        public List<Questionlist> Questionlist { get; set; }

        [JsonProperty("subsection")]
        public string Subsection { get; set; }
    }

    public class Sectionlist
    {
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("section")]
        public string Section { get; set; }

        [JsonProperty("sectiontype")]
        public string SectionType { get; set; }

        [JsonProperty("subsectionlist")]
        public List<Subsectionlist> Subsectionlist { get; set; }
    }

    #region SavedAnswers
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class SavedAnswerDetail
    {
        [JsonProperty("answernumber")]
        public string Answernumber { get; set; }

        [JsonProperty("answertext")]
        public object Answertext { get; set; }
    }

    public class SavedAnswer
    {
        [JsonProperty("answerdetaillist")]
        public List<SavedAnswerDetail> Answerdetaillist { get; set; }

        [JsonProperty("questionnumber")]
        public string Questionnumber { get; set; }

        [JsonProperty("section")]
        public string Section { get; set; }

        [JsonProperty("subsection")]
        public object Subsection { get; set; }
    }

    public class SavedAnsersResponse : BaseResponse
    {
        [JsonProperty("answerlist")]
        public List<SavedAnswer> Answerlist { get; set; }

        [JsonProperty("percentage")]
        public string Percentage { get; set; }
    }


    #endregion

    #region Previously Submitted Surveys
    public class Versiondatelist
    {
        [JsonProperty("contractaccountnumber")]
        public string Contractaccountnumber { get; set; }

        [JsonProperty("createddate")]
        public string Createddate { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class SubmittedSurveysResponse:BaseResponse
    {
        [JsonProperty("versiondatelist")]
        public List<Versiondatelist> Versiondatelist { get; set; }
    }

    #endregion

    #region Download Survey
    public class DownloadSurveyResponse:BaseResponse
    {
        [JsonProperty("binarydata")]
        public string Binarydata { get; set; }
    }
    #endregion

    #region Submit Survey Response
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Breakdownlist
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("value1")]
        public string Value1 { get; set; }

        [JsonProperty("value2")]
        public string Value2 { get; set; }

        [JsonProperty("value3")]
        public string Value3 { get; set; }
    }

    public class Tipslist
    {
        [JsonProperty("header")]
        public string Header { get; set; }

        [JsonProperty("tipurl")]
        public string Tipurl { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("tips")]
        public string Tips { get; set; }
    }

    public class SubmitSurveyResponse:BaseResponse
    {
        [JsonProperty("breakdownlist")]
        public List<Breakdownlist> Breakdownlist { get; set; }               

        [JsonProperty("percentage")]
        public string Percentage { get; set; }        

        [JsonProperty("tipslist")]
        public List<Tipslist> Tipslist { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }
    }

    public class SaveSurveyResponse : BaseResponse
    {
        [JsonProperty("percentage")]
        public string Percentage { get; set; }
    }
        #endregion
    }
