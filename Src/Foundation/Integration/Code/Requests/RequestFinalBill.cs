using System;

namespace DEWAXP.Foundation.Integration.Requests
{
    public class RequestFinalBill
    {
        public string ContractAccountNumber { get; set; }

        public string PremiseNumber { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string MobileNumber { get; set; }

        public DateTime? DisconnectionDate { get; set; }

        public string Remarks { get; set; }

        public string IbanNumber { get; set; }

        public string IbanRefund { get; set; }

        public byte[] Attachment { get; set; }

        public string AttachmentExtension { get; set; }

        public string RefundMode { get; set; }

    }
}