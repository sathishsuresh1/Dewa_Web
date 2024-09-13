using DEWAXP.Foundation.Integration.Responses.ConsultantSvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Builder.Models.ConnectionCalculator
{
    public class ConnectionCalculatorDetails
    {
        public int SerialNo { get; set; }
        public decimal FromLoad { get; set; }
        public decimal ToLoad { get; set; }
        public decimal UnitValue { get; set; }
        public decimal TotalValue { get; set; }
        public decimal Quantity { get; set; }
        public decimal GrandTotal { get; set; }

        public static ConnectionCalculatorDetails[] From(ConnectionCalculatorResponse payload)
        {
            int count = payload.LoadDetails.Count;
            ConnectionCalculatorDetails[] costDetails = new ConnectionCalculatorDetails[count];

            for (int i = 0; i < count; i++)
            {
                costDetails[i] = new ConnectionCalculatorDetails();
                costDetails[i].FromLoad = payload.LoadDetails[i].FromLoad;
                costDetails[i].ToLoad = payload.LoadDetails[i].ToLoad;
                costDetails[i].Quantity = payload.LoadDetails[i].Quantity;
                costDetails[i].SerialNo = payload.LoadDetails[i].SerialNo;
                costDetails[i].TotalValue = payload.LoadDetails[i].TotalValue;
                costDetails[i].UnitValue = payload.LoadDetails[i].UnitValue;
            }

            return costDetails;
        }

    }
}