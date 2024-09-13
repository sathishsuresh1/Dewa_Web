using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    [Bind(Include ="ActionType,RefId,OTP")]
    public class SalesOrderDownloadOTPModel
    {
        public string ActionType { get; set; }
        public string RefId { get; set; }
        public string OTP { get; set; }
    }
}