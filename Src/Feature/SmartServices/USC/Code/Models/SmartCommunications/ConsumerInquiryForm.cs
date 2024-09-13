using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.USC.Models.SmartCommunications
{
    public class ConsumerInquiryForm
    {
        public int Type { get; set; }
        public int SubType { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Mobile { get; set; }
        public string InquiryDetails { get; set; }
        public string Generalinquirytype { get; set; }
        public string AccountNumber { get; set; }
        public string vMobile { get; set; }
        public string InquiryType { get; set; }
        public string InquiryTypetxt { get; set; }
        public string recaptcha { get; set; }
    }

    public class BuilderTDInquiryForm
    {
        //public string Channel { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string Mobile { get; set; }
        public string Discussionarea { get; set; }
        public string Discussionareatxt { get; set; }
        public string InquiryDetails { get; set; }
        public string BuilderSubType { get; set; }
        public string NOCCategory { get; set; }
        public string NOCCategorytxt { get; set; }
        public string recaptcha { get; set; }
    }
}