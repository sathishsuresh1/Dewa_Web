using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Dashboard.Models.AwayMode
{
    public class ConsumptionGraphSettingModel
    {
        public int[] dates { get; set; }

        public List<string> ElectricityDates { get; set; }
        public List<ConsumptionDataSeries> ElectricityConsumptionDetail { get; set; }
        public List<string> WaterDates { get; set; }
        public List<ConsumptionDataSeries> WaterConsumptionDetail { get; set; }


        public string DateJsonArray { get; set; }

        public string ElectricityConsumptionDetailJsonData { get; set; }
        public string WaterConsumptionDetailJsonData { get; set; }
    }

    public class ConsumptionDetail
    {
        public string xdata { get; set; }
        public string DataSeries { get; set; }
    }

    public class ConsumptionDataSeries
    {
        public List<double?> data { get; set; }

        public string name { get; set; }

        public string color { get; set; }
    }
}