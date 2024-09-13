using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
    public class EstimateCustomerListResponse
    {
        public string description { get; set; }
        public List<EstimateCustomerItemResponse> estimatedetailitem { get; set; }
        public string responsecode { get; set; }
    }

    public class EstimateCustomerItemResponse
    {
        public string area { get; set; }
        public string ca_number { get; set; }
        public string city { get; set; }
        public string comment { get; set; }
        public string con_email { get; set; }
        public string con_name { get; set; }
        public string consultantno { get; set; }
        public string customer_po_number { get; set; }
        public string est_time { get; set; }
        public string est_timet { get; set; }
        public string estimateno { get; set; }
        public string estimatevalidfromdate { get; set; }
        public string estimatevalidtodate { get; set; }
        public decimal netvalue1 { get; set; }
        public decimal netvalue2 { get; set; }
        public decimal netvalue3 { get; set; }
        public string ownername { get; set; }
        public string ownernum { get; set; }
        public string plot { get; set; }
        public string sales_distribution_doc { get; set; }
        public string sales_document_type { get; set; }
        public string sold_to_party { get; set; }
        public object stat_icon { get; set; }
        public object vkont_pc1 { get; set; }
        public string vkont_region { get; set; }
        public string vkont_street { get; set; }
        public string waerk { get; set; }
        public string waerk2 { get; set; }
        public decimal PartialPayment { get; set; }
    }
}
