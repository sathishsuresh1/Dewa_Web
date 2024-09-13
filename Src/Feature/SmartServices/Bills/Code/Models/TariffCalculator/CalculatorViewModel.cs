using DEWAXP.Foundation.Content.Models.TariffCalculator;
using System.Collections.Generic;

namespace DEWAXP.Feature.Bills.Models.TariffCalculator
{
    public class CalculatorViewModel
    {
        public virtual Calculator CalculatorDs { get; set; }
        public virtual CustomerTypeValue Residential { get; set; }
        public virtual CustomerTypeValue Industrial { get; set; }
        public virtual CustomerTypeValue Commercial { get; set; }
        public virtual CustomerTypeValue D33 { get; set; }

        public List<TariffJsonData> TariffJsonList { get; set; }
        public string tariffJson { get; set; }
    }

    public class CustomerTypeValue
    {
        public virtual TariffTypeValue Electricity { get; set; }
        public virtual TariffTypeValue Water { get; set; }
    }

    public class TariffTypeValue
    {
        public virtual Consumption Red { get; set; }
        public virtual Consumption Green { get; set; }
        public virtual Consumption Yellow { get; set; }
        public virtual Consumption Orange { get; set; }
        public virtual string FuelSurchargeTariff { get; set; }
    }


    public class TariffData
    {
        public TariffData(CustomerTypeValue values)
        {
            if (values != null)
            {
                if (values.Electricity != null)
                {
                    if (values.Electricity.Green != null)
                    {
                        baseelec = values.Electricity.Green.To;
                        gelec = $"{values.Electricity.Green.From} - {values.Electricity.Green.To}";
                    }

                    if (values.Electricity.Yellow != null)
                    {
                        yelec = $"{values.Electricity.Yellow.From} - {values.Electricity.Yellow.To}";
                    }

                    if (values.Electricity.Orange != null)
                    {
                        oelec = $"{values.Electricity.Orange.From} - {values.Electricity.Orange.To}";
                    }

                    if (values.Electricity.Red != null)
                    {
                        relec = $"{values.Electricity.Red.From} - {values.Electricity.Red.To}";
                    }
                }

                if (values.Water != null)
                {
                    if (values.Water.Green != null)
                    {
                        basewater = values.Water.Green.To;
                        gwater = $"{values.Water.Green.From} - {values.Water.Green.To}";

                    }

                    if (values.Water.Yellow != null)
                    {
                        ywater = $"{values.Water.Yellow.From} - {values.Water.Yellow.To}";

                    }
                    if (values.Water.Orange != null)
                    {
                        owater = $"{values.Water.Orange.From} - {values.Water.Orange.To}";

                    }
                }


            }

        }
        public string baseelec { get; set; }
        public string basewater { get; set; }
        public string gelec { get; set; }
        public string yelec { get; set; }
        public string oelec { get; set; }
        public string relec { get; set; }
        public string gwater { get; set; }
        public string ywater { get; set; }
        public string owater { get; set; }
    }

    public class TariffJsonData
    {
        public string dtype { get; set; }
        public TariffData data { get; set; }
    }
}