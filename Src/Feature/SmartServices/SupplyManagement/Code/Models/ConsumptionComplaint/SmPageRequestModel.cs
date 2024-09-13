using DEWAXP.Foundation.Content.Models.Consumption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.SupplyManagement.Models.ConsumptionComplaint
{
    [Bind(Include = "asc,se,n,t,l,s,isLang,a")]
    public class SmPageRequestModel
    {
        public SmPageRequestModel()
        {
            asc = 0;
            se = "";
            n = "";
            t = "0";
            l = null;
            s = null;
        }
        public int? asc { get; set; }
        public string se { get; set; }
        public string n { get; set; }
        public string t { get; set; }
        public SmlangCode? l { get; set; }
        public SmScreenCode? s { get; set; }
        public int isLang { get; set; }
        public string a { get; set; }
    }
}