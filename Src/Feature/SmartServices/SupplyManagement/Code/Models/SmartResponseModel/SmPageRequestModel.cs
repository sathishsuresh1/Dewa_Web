using DEWAXP.Foundation.Content.Models.SmartResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.SmartResponseModel
{
    public class SmPageRequestModel
    {
        public SmPageRequestModel()
        {
            this.asc = 0;
            this.se = "";
            this.n = "";
            this.t = "0";
            this.l = null;
            this.s = null;
        }
        public int? asc { get; set; }
        public string se { get; set; }
        public string n { get; set; }
        public string t { get; set; }
        public SmlangCode? l { get; set; }
        public SmScreenCode? s { get; set; }
        public int isLang { get; set; }
    }
}