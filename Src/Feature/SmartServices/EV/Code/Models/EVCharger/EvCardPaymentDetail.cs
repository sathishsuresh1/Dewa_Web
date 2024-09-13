using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    public class EvCardPaymentDetail
    {
        public string accountNumber { get; set; }
        public string amount1 { get; set; }
        public string amount2 { get; set; }
        public string courierCharge { get; set; }
        public string courierVatAmount { get; set; }
        public string evCardNumber { get; set; }
        public string plateNumber { get; set; }
        public string sdAmount { get; set; }
        public string totalAmount { get; set; }
    }
}