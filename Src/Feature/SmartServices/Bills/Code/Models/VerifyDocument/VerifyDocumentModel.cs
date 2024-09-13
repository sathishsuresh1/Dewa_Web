using DEWAXP.Foundation.DataAnnotations;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Feature.Bills.VerifyDocument
{
    public class VerifyDocumentModel
    {
        public string DocumentType { get; set; }
        public bool DocumentTypeRequired { get; set; }

        public string ReferenceNumber { get; set; }

        [RegularExpression("^[0-9]{4}$", ValidationMessageKey = "Verification.errorMessage_PINNumber")]
        public string PinNumber { get; set; }

        public string Recaptcha { get; set; }
        public QRCodeResponse QRCodeResponse { get; set; }
    }
}