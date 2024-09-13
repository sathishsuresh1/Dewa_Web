using DEWAXP.Foundation.Content.Models;
using System;

namespace DEWAXP.Feature.EV.Models.EVCharger
{
    [Serializable]
    public class EnquirySuccess
    {
        public SharedAccount Account { get; set; }

        public string Reference { get; set; }

        public string MobileNumber { get; set; }

        public string FurtherComments { get; set; }

        public byte[] Documentation { get; set; }

        public string AttachmentType { get; set; }

        public string FileName { get; set; }

        public string QueryType { get; set; }

        public string SelectedQueryType { get; set; }
    }
}