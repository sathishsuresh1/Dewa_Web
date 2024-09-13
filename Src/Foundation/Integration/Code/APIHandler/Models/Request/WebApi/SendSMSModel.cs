using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEWAXP.Foundation.Integration.APIHandler.Models.Request.WebApi
{
    public class SendSMSModel
    {   
        public SendSMSModel()
        {
            Lang = "en";
        }
        public string MoblieNumber { get; set; }
        public string TextMessage { get; set; }
        public string ApplicationName { get; set; }
        public string SenderName { get; set; }
        public string SMSPriority { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
        public string SMSSHORTCODE { get; set; }
        /// <summary>
        /// "en" or "ar" no other value
        /// </summary>
        public string Lang { get; set; }
    }
}
