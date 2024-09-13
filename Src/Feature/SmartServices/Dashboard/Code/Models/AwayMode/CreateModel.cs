using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Dashboard.Models.AwayMode
{
    [Bind(Include = "IsLoggedIn,ContractAccount,BeginDate,EndDate,Frequency,Email,r,otp,ActionType,key")]
    public class CreateModel
    {
        public bool IsLoggedIn { get; set; }
        public string ContractAccount { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public string Frequency { get; set; }
        public string FrequencyText { get; set; }
        public List<SelectListItem> FrequencyList { get; set; }
        public string Email { get; set; }
        public string RequestId { get; set; }
        public string otp { get; set; }
        public string ActionType { get; set; }
        public string key { get; set; }
    }
}