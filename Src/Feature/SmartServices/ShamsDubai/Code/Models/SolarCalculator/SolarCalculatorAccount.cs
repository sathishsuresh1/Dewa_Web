using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ShamsDubai.Models.SolarCalculator
{
    public class SolarCalculatorAccount
    {
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public decimal YearlyUsageKWh { get; set; }
        public decimal YearlyUsageDhm { get; set; }
        public string TariffType { get; set; }
        public string CorrectYear { get; set; }
    }
}