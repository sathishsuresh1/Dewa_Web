using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.SupplyManagement.Models.MaiDubai
{
    public class MaiDubaiViewModel
    {
        public string urlGUID { get; set; }
        public string MaskedEmail { get; set; }
        public string MaskedMobile { get; set; }
        public string ReferenceCode { get; set; }
        public string OtpRequestId { get; set; }
        public string EncryptedReference { get; set; }
        public linkState isSucess { get; set; }
    }

    public enum linkState
    {
        Success,
        Redeemed,
        Expired,
        Invalid
    }

}