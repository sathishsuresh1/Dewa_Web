using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ShamsDubai.Models.SolarCalculator
{
    public class Tariff
    {
        public Tariff(double[] breaks, double[] prices, double fuelcharges)
        {
            Breaks = breaks;
            Prices = prices;
            FuelCharges = fuelcharges;
        }
        
        //public string Name { get; set; }

        public double[] Breaks { get; set; }

        public double[] Prices { get; set; }

        public double FuelCharges { get; set; } 
    }
}