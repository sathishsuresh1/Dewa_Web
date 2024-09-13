using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingStep6Model
    {
        public string TenderNumber { get; set; }
        public string TenderEndDescription { get; set; }
        public string TenderARDescription { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase TenderForm_AttachedDocument1 { get; set; }
        public byte[] TenderForm_AttachmentFileBinary1 { get; set; }
        public string TenderForm__AttachmentFileType1 { get; set; }
        public string TenderForm_AttachmentFileName1 { get; set; }
        public string TenderForm_AttachmentMimeType1 { get; set; }
        public bool TenderForm_AttachmentRemove1 { get; set; }
        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase TenderForm_AttachedDocument2 { get; set; }
        public byte[] TenderForm_AttachmentFileBinary2 { get; set; }
        public string TenderForm__AttachmentFileType2 { get; set; }
        public string TenderForm_AttachmentFileName2 { get; set; }
        public string TenderForm_AttachmentMimeType2 { get; set; }
        public bool TenderForm_AttachmentRemove2 { get; set; }
        public string DocumentNumber { get; set; }
    }
}