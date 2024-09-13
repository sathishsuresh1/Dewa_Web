using DEWAXP.Foundation.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class TenderBidingStep7Model
    {
        public string TenderNumber { get; set; }
        public string TenderEndDescription { get; set; }
        public string TenderARDescription { get; set; }

        [MaxFileSize(10 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 10MB")]
        public HttpPostedFileBase TenderOther_AttachedDocument1 { get; set; }
        public byte[] TenderOther_AttachmentFileBinary1 { get; set; }
        public string TenderOther__AttachmentFileType1 { get; set; }
        public string TenderOther_AttachmentFileName1 { get; set; }
        public string TenderOther_AttachmentMimeType1 { get; set; }
        public bool TenderOther_AttachmentRemove1 { get; set; }
        public string DocumentNumber { get; set; }
        //[MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        //public HttpPostedFileBase TenderOther_AttachedDocument2 { get; set; }
        //public byte[] TenderOther_AttachmentFileBinary2 { get; set; }
        //public string TenderOther__AttachmentFileType2 { get; set; }

        //[MaxFileSize(3 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        //public HttpPostedFileBase TenderOther_AttachedDocument3 { get; set; }
        //public byte[] TenderOther_AttachmentFileBinary3 { get; set; }
        //public string TenderOther__AttachmentFileType3 { get; set; }
    }
}