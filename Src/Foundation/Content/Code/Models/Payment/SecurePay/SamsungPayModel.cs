using Sitecore.Rules.Conditions.ItemConditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.Payment.SecurePay
{
    public class SamsungPayModel
    {
        public string totalAmount { get; set; }
        public string id { get; set; }
        public string details { get; set; }
    }
    public class SamsungPayContractaccounts
    {
        public string accountNumber { get  ; set; }
        public string amount { get; set; }
    }
}