using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.SmartSurvey
{
    public class Grouplist
    {
        [JsonProperty("group")]
        public string Group;

        [JsonProperty("groupdescription")]
        public string Groupdescription;

        [JsonProperty("groupnumber")]
        public string Groupnumber;

        [JsonProperty("pagenumber")]
        public string Pagenumber;

        [JsonProperty("note")]
        public string Note;

        [JsonProperty("questiontypeslist")]
        public List<Questiontypeslist> Questiontypeslist;

        [JsonProperty("showheading")]
        public string Showheading;
    }

    public class Optionslist
    {
        [JsonProperty("questionoption")]
        public string Questionoption;

        [JsonProperty("questionoptiondescription")]
        public string Questionoptiondescription;

        [JsonProperty("selectedflag")]
        public string Selectedflag;
    }

    public class Questiontypeslist
    {
        [JsonProperty("alignment")]
        public string Alignment;

        [JsonProperty("allowedfilesize")]
        public string Allowedfilesize;

        [JsonProperty("allowedfiletypes")]
        public string Allowedfiletypes;

        [JsonProperty("asheading")]
        public string Asheading;

        [JsonProperty("backendvalue")]
        public string Backendvalue;

        [JsonProperty("displayonly")]
        public string Displayonly;

        [JsonProperty("filename")]
        public string Filename;

        [JsonProperty("filetype")]
        public string Filetype;

        [JsonProperty("mandatory")]
        public string Mandatory;

        [JsonProperty("othermandatory")]
        public string Othermandatory;

        [JsonProperty("othermandatoryoptionno")]
        public string Othermandatoryoptionno;

        [JsonProperty("othernotes")]
        public string Othernotes;

        [JsonProperty("otherplaceholder")]
        public string Otherplaceholder;

        [JsonProperty("question")]
        public string Question;

        [JsonProperty("questionnote")]
        public string Questionnote;

        [JsonProperty("questionnumber")]
        public string Questionnumber;

        [JsonProperty("questionplaceholder")]
        public string Questionplaceholder;

        [JsonProperty("questiontype")]
        public string Questiontype;

        [JsonProperty("surveyfeedback")]
        public string Surveyfeedback;

        [JsonProperty("optionslist")]
        public List<Optionslist> Optionslist;
    }

    public class SurveyDataOutputModel
    {
        [JsonProperty("description")]
        public string Description;

        [JsonProperty("responsecode")]
        public string Responsecode;

        [JsonProperty("surveydataoutput")]
        public Surveydataoutput Surveydataoutput;
    }
    public class SurveyOTPOutputModel
    {
        [JsonProperty("description")]
        public string Description;

        [JsonProperty("responsecode")]
        public string Responsecode;

    }
    public class Surveydataoutput
    {
        [JsonProperty("accountnumber")]
        public string Accountnumber;

        [JsonProperty("authenticate")]
        public string Authenticate;

        [JsonProperty("candidateid")]
        public string Candidateid;

        [JsonProperty("closemessage")]
        public string Closemessage;

        [JsonProperty("customernumber")]
        public string Customernumber;

        [JsonProperty("emailid")]
        public string Emailid;

        [JsonProperty("employeeid")]
        public string Employeeid;

        [JsonProperty("grouplist")]
        public List<Grouplist> Grouplist;

        [JsonProperty("introduction")]
        public string Introduction;

        [JsonProperty("introductionheader")]
        public string Introductionheader;

        [JsonProperty("maskedemailid")]
        public string MaskedEmailid;

        [JsonProperty("maskedphone")]
        public string MaskedPhone;

        [JsonProperty("phone")]
        public string Phone;

        [JsonProperty("processdescription")]
        public string Processdescription;

        [JsonProperty("status")]
        public string Status;
    }

    public enum QuestionType
    {
        SR, MB, DD, DT, RB
    }
}
