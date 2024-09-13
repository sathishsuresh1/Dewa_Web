using System.Collections.Generic;
using System.Linq;

namespace DEWAXP.Feature.Dashboard.Models.Graph
{
    public class GraphModel
    {
        public List<double> UsageValue { get; set; }

        public static string ToJsonString(GraphModel model)
        {
            return string.Join(",", model.UsageValue.Select(x => x.ToString()));
        }
    }

    public class GraphRenderingModel
    {
        public bool IsSmartElectricityMeter { get; set; }
        public bool IsSmartWaterMeter { get; set; }
        public string MoveInDate { get; set; }
        public string MoveOutDate { get; set; }
    }
}