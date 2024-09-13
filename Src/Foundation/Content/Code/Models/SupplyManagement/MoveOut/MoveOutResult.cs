using DEWAXP.Foundation.Integration.APIHandler.Models.Response.MoveOut;
using System.Collections.Generic;

namespace DEWAXP.Foundation.Content.Models.MoveOut
{
    public class MoveOutResult
    {
        public bool proceed { get; set; }
        public bool issuccess { get; set; }
        public string errormessage { get; set; }
        public List<string> duplicaterequests { get; set; }
        public double totalamounttopay { get; set; }
        public bool iscustomer { get; set; }

        public List<MoveOutaccountsDetailResponse> details { get; set; }
        public List<MoveOutDivisionWiseCAResponse> divisionlist { get; set; }
        public string TotalPendingBalance { get; set; }
        public string OutstandingBalance { get; set; }
        public string PaymentAmountList { get; set; }
        public string PaymentAccountList { get; set; }
        public string PaymentBP_List { get; set; }
        public string evCardnumber { get; set; }
        //public accountsDetailsOut[] details { get; set; }
    }

    public enum evdeactivatestep
    {
        accounts = 0,
        details = 1,
        review = 2,
        confirm = 3
    }
}