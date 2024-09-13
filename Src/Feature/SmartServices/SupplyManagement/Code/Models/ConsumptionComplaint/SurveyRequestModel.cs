using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.ConsumptionComplaint
{
    [Bind(Include = "s,n,vd")]
    public class SurveyRequestModel
    {
        public SurveyRequestModel()
        {
            vd = "1";
        }
        public string s { get; set; }
        public string n { get; set; }
        public string vd { get; set; }

    }
}