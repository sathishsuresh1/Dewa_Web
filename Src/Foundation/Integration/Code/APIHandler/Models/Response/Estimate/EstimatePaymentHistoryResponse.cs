using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Response.Estimate
{
    public class EstimatePaymentHistoryResponse
    {
        public string description { get; set; }
        public List<EstimatehistoryItemResponse> estimatehistory { get; set; }
        public string responsecode { get; set; }

    }

    public class EstimatehistoryItemResponse
    {
        public object add_info1 { get; set; }
        public string add_info2 { get; set; }
        public string appno { get; set; }
        public string bill_message { get; set; }
        public string bpdetails { get; set; }
        public object cardtype { get; set; }
        public string contract_account { get; set; }
        public string create_date { get; set; }
        public string credential { get; set; }
        public string dateandtime { get; set; }
        public string deg_transid { get; set; }
        public string degamount { get; set; }
        public string dewa_transid { get; set; }
        public object dewaamount { get; set; }
        public object disputeid { get; set; }
        public string encryptedresmsg { get; set; }
        public string estimationno { get; set; }
        public string exe_date { get; set; }
        public object ip_address { get; set; }
        public string lot_no { get; set; }
        public string message { get; set; }
        public string messagecode { get; set; }
        public string old_status { get; set; }
        public string opbel { get; set; }
        public object payment_div { get; set; }
        public string pmtchnl { get; set; }
        public object pmtgatename { get; set; }
        public string pmtgaterecstat { get; set; }
        public object pmtgatetxnno { get; set; }
        public object pmtmethod { get; set; }
        public string pmtmode { get; set; }
        public object qrundatetime { get; set; }
        public string receiptid { get; set; }
        public object refundid { get; set; }
        public object refundstat { get; set; }
        public string req_mode { get; set; }
        public string runfrom { get; set; }
        public object servcode { get; set; }
        public object spcode { get; set; }
        public string status { get; set; }
        public string stazs { get; set; }
        public string tran_amount { get; set; }
        public string trans_type { get; set; }
        public object trnamount { get; set; }
        public string vendorid { get; set; }
    }
}
