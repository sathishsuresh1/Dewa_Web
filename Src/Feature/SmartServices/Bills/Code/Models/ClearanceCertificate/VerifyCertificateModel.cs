using DEWAXP.Foundation.DataAnnotations;
using System;

namespace DEWAXP.Feature.Bills.ClearanceCertificate
{
    [Serializable]
    public class VerifyCertificateModel : ClearanceCertificateModel
    {
        [Required]
        public string ReferenceNumber { get; set; }

        [Required]
        public string PinNumber { get; set; }

        public string RequesterName { get; set; }

        public string CertificateNote { get; set; }

        public byte[] pdfData { get; set; }
    }
}