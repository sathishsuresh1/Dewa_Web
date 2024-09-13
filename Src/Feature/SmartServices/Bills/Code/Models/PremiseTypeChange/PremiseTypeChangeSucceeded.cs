using DEWAXP.Foundation.Content.Models;
using System;

namespace DEWAXP.Feature.Bills.Models.PremiseTypeChange
{
    [Serializable]
    public class PremiseTypeChangeSucceeded
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