using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingStep5Model
    {
        public string TenderNumber { get; set; }
        public string TenderBidRefNumber { get; set; }
        public string BidStatus { get; set; }
        public string TenderEndDescription { get; set; }
        public string TenderARDescription { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase TenderBond_AttachedDocument1 { get; set; }
        public byte[] TenderBond_AttachmentFileBinary1 { get; set; }
        public string TenderBond__AttachmentFileType1 { get; set; }
        public string TenderBond_AttachmentFileName1 { get; set; }
        public string TenderBond_AttachmentMimeType1 { get; set; }
        public bool TenderBond_AttachmentRemove1 { get; set; }
     
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase TenderBond_AttachedDocument2 { get; set; }
        public byte[] TenderBond_AttachmentFileBinary2 { get; set; }
        public string TenderBond__AttachmentFileType2 { get; set; }
        public string TenderBond_AttachmentFileName2 { get; set; }
        public string TenderBond_AttachmentMimeType2 { get; set; }
        public bool TenderBond_AttachmentRemove2 { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase TenderBond_AttachedDocument3 { get; set; }
        public byte[] TenderBond_AttachmentFileBinary3 { get; set; }
        public string TenderBond__AttachmentFileType3 { get; set; }
        public string TenderBond_AttachmentFileName3 { get; set; }
        public string TenderBond_AttachmentMimeType3 { get; set; }
        public bool TenderBond_AttachmentRemove3 { get; set; }

        public string DocumentNumber { get; set; }
    }
}