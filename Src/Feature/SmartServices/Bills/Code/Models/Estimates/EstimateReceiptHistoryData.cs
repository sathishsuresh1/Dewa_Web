using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DEWAXP.Foundation.Integration.DewaSvc;
using X.PagedList;

namespace DEWAXP.Feature.Bills.Models.Estimates
{
    public class EstimateReceiptHistoryData : EstimateRequest
    {

        public List<EstimateTransaction> PaymentHistoryDetails { get; set; }
        public IPagedList<EstimateTransaction> PagedPaymentHistoryDetails { get; set; }

    }
}