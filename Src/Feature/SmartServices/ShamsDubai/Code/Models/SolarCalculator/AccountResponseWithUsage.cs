using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ShamsDubai.Models.SolarCalculator
{
    public class AccountResponseWithUsage
    {
        public string locale { get; set; }
        public SolarCalculatorAccountWithUsage account { get; set; }
    }
}