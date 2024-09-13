using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingStep1Model
    {
        public string TenderNumber { get; set; }
        public string TenderBondAmount { get; set; }
        public string TenderVatPayble { get; set; }
        public string TenderStatus { get; set; }
        public string TenderBidStatus { get; set; }
        public string TenderSubmissiondate { get; set; }
        public string TenderSubmissionDeadline { get; set; }
        public string TenderBidRefNumber { get; set; }
        public string TenderARDescription { get; set; }
        public string TenderBidAmount { get; set; }
        public string TenderEndDescription { get; set; }
        public string TenderStatusValue { get; set; }
        public string TenderBondPercentage { get; set; }
        public string SubmitType { get; set; }
        public string bidMode { get; set; }
    }
}