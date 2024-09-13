using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ShamsDubai.Models.SolarCalculator
{
    public class SolarCalculatorUsageItem
    {
        public int index { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public decimal usageDhm { get; set; }
        public decimal usageKWh { get; set; }
    }
}