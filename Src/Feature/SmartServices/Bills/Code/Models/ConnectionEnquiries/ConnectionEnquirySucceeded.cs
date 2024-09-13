using DEWAXP.Foundation.Content.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.Bills.Models.ConnectionEnquiries
{
    [Serializable]
    public class ConnectionEnquirySucceeded
    {
        public SharedAccount Account { get; set; }

        public string Reference { get; set; }

        public string MobileNumber { get; set; }

        public string FurtherComments { get; set; }

        public byte[] Documentation { get; set; }

        public string AttachmentType { get; set; }

        public string FileName { get; set; }
    }
}