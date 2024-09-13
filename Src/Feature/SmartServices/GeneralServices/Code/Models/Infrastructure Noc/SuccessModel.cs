using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.GeneralServices.Models.Infrastructure_Noc
{
    public class SuccessModel
    {
        public bool IsSuccess { get; set; }
        public string DEWAnum { get; set; }
        public string description { get; set; }
        public string responsecode { get; set; }
    }
}