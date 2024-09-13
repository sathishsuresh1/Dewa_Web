using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.ConsumptionComplaint
{
    [Serializable]
    public class SurveyPostModel
    {
        public SurveyPostModel()
        {
          //  SurveyDatas = new List<DEWAXP.Foundation.Integration.SmartConsultantSvc.surveyData>();
        }
        public string n { get; set; }
        public string s { get; set; }
        public  List< DEWAXP.Foundation.Integration.SmartConsultantSvc.surveyData> SurveyDatas { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsError { get; set; }

    }
}