using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartSurvey
{
    public class Grouplist
    {
        [JsonProperty("group")]
        public string group { get; set; }

        [JsonProperty("groupnumber")]
        public string groupnumber { get; set; }

        [JsonProperty("questiontypeslist")]
        public List<Questiontypeslist> questiontypeslist { get; set; }
    }

    public class Optionslist
    {
        [JsonProperty("questionoption")]
        public string questionoption { get; set; }

        [JsonProperty("questionoptiondescription")]
        public string questionoptiondescription { get; set; }

        [JsonProperty("selectedflag")]
        public string selectedflag { get; set; }
    }

    public class Questiontypeslist
    {
        [JsonProperty("question")]
        public string question { get; set; }

        [JsonProperty("questionnumber")]
        public string questionnumber { get; set; }

        [JsonProperty("questiontype")]
        public string questiontype { get; set; }

        [JsonProperty("surveyfeedback")]
        public string surveyfeedback { get; set; }

        [JsonProperty("optionslist")]
        public List<Optionslist> optionslist { get; set; }

        [JsonProperty("alignment")]
        public string alignment { get; set; }

        [JsonProperty("allowedfilesize")]
        public string allowedfilesize { get; set; }

        [JsonProperty("allowedfiletypes")]
        public string allowedfiletypes { get; set; }

        [JsonProperty("asheading")]
        public string asheading { get; set; }

        [JsonProperty("backendvalue")]
        public string backendvalue { get; set; }

        [JsonProperty("displayonly")]
        public string displayonly { get; set; }

        [JsonProperty("filename")]
        public string filename { get; set; }

        [JsonProperty("filetype")]
        public string filetype { get; set; }

        [JsonProperty("mandatory")]
        public string mandatory { get; set; }

        [JsonProperty("othermandatory")]
        public string OtherMandatory { get; set; }

        [JsonProperty("othermandatoryoptionno")]
        public string OtherMandatoryOptionno { get; set; }

        [JsonProperty("othernotes")]
        public string othernotes { get; set; }

        [JsonProperty("otherplaceholder")]
        public string otherplaceholder { get; set; }

        [JsonProperty("questionplaceholder")]
        public string questionplaceholder { get; set; }
    }

    public class SurveyDataInput
    {
        [JsonProperty("surveyinput")]
        public Surveyinput surveyinput { get; set; }
    }

    public class Surveydatainput
    {
        [JsonProperty("accountnumber")]
        public string accountnumber { get; set; }

        [JsonProperty("candidateid")]
        public string candidateid { get; set; }

        [JsonProperty("closemessage")]
        public string closemessage { get; set; }

        [JsonProperty("customernumber")]
        public string customernumber { get; set; }

        [JsonProperty("emailid")]
        public string emailid { get; set; }

        [JsonProperty("employeeid")]
        public string employeeid { get; set; }

        [JsonProperty("grouplist")]
        public List<Grouplist> grouplist { get; set; }

        [JsonProperty("introduction")]
        public string introduction { get; set; }

        [JsonProperty("introductionheader")]
        public string introductionheader { get; set; }

        [JsonProperty("phone")]
        public string phone { get; set; }

        [JsonProperty("processdescription")]
        public string processdescription { get; set; }
    }

    public class Surveyinput
    {
        [JsonProperty("appidentifier")]
        public string appidentifier { get; set; }

        [JsonProperty("appversion")]
        public string appversion { get; set; }

        [JsonProperty("lang")]
        public string lang { get; set; }

        [JsonProperty("mobileosversion")]
        public string mobileosversion { get; set; }

        [JsonProperty("processid")]
        public string processid { get; set; }

        [JsonProperty("surveydatainput")]
        public Surveydatainput surveydatainput { get; set; }

        [JsonProperty("surveyidentifier")]
        public string surveyidentifier { get; set; }

        [JsonProperty("surveymode")]
        public string surveymode { get; set; }

        [JsonProperty("vendorid")]
        public string vendorid { get; set; }
    }

    public class SurveyOTPInput
    {
        public Surveyotpinput surveyotpinput { get; set; }
    }
    public class Surveyotpinput
    {
        public string appidentifier { get; set; }
        public string appversion { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string mode { get; set; }
        public string OTP { get; set; }
        public string lang { get; set; }
        public string mobileosversion { get; set; }
        public string surveyidentifier { get; set; }
        public string vendorid { get; set; }
    }
}
