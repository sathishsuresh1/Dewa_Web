using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ShamsDubai.Models.SolarCalculator
{
    public class AccountResponse
    {
        public string locale { get; set; }
        public SolarCalculatorAccount account { get; set; }
    }
}