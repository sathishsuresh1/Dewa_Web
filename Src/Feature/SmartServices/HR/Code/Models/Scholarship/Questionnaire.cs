using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.HR.Models.Scholarship
{
    public class Questionnaire
    {
        public const string MILITARY_STATUS_FILE_NAME = "Military_Service_Certificate";
        public string MilitaryStatus { get; set; }
        public string AnyMedicalCondition { get; set; }
        public string MedicalConditionDetail {get; set;}

        public HttpPostedFileBase MilitaryServiceUpload { get; set; }

        public Attachment ExistingMilitaryServiceUpload { get; set; }
    }
}