using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DEWAXP.Foundation.Content.Models.SupplyManagement.MoveTo
{
    [Serializable]
    public class MoveToWorkflowState
    {
        public SharedAccount Account { get; set; }

        public string ContractAccount { get; set; }

        public DateTime? DisconnectDate { get; set; }

        public string MobileNumber { get; set; }

        public List<string> PremiseNos { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string BusinessPartner { get; set; }

        public string CustomerCategory { get; set; }

        public string createuseraccount { get; set; }

        public bool Succeeded { get; set; }

        public string ResponseMessage { get; set; }

        public string ErrorMessage { get; set; }

        public bool AttachmentFlag { get; set; }

        public string AccountType { get; set; }

        public string CustomerType { get; set; }

        public string PremiseNo { get; set; }

        public string IdType { get; set; }

        public string IdNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? ContractStartDate { get; set; }

        public DateTime? MoveinStartDate { get; set; }

        public DateTime? MoveOuttDate { get; set; }

        public string MoveOutAccount { get; set; }

        public DateTime? ContractEndDate { get; set; }

        public int NumberOfRooms { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PoBox { get; set; }

        public string Emirate { get; set; }

        public string MobilePhone { get; set; }

        public string EmailAddress { get; set; }

        public string Nationality { get; set; }

        public string ContractValue { get; set; }

        public string Reference { get; set; }

        public string ProcessMovein { get; set; }

        public string PremiseType { get; set; }

        public string Ejari { get; set; }
        public string ReconnectionAddressRegistrationFee { get; set; }

        public string OutstandingBalance { get; set; }

        public string KnowledgeFee { get; set; }

        public string InnovationFee { get; set; }

        public string PremiseAccount { get; set; }

        public string VatNumber { get; set; }

        public bool Owner { get; set; }

        public bool moveinindicator { get; set; }
        public byte[] Attachment1 { get; set; }

        public string Attachment1Filename { get; set; }

        public string Attachment1FileType { get; set; }

        public byte[] Attachment2 { get; set; }

        public string Attachment2Filename { get; set; }

        public string Attachment2FileType { get; set; }

        public byte[] Attachment3 { get; set; }

        public string Attachment3Filename { get; set; }

        public string Attachment3FileType { get; set; }

        public byte[] Attachment4 { get; set; }

        public string Attachment4Filename { get; set; }

        public string Attachment4FileType { get; set; }

        public byte[] Attachment5 { get; set; }

        public string Attachment5Filename { get; set; }

        public string Attachment5FileType { get; set; }

        public string LogonMode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(BusinessPartner))
                {
                    return "A";
                }
                return "R";
            }
        }

        public bool HasBeenLodged
        {
            get { return !string.IsNullOrWhiteSpace(this.Reference); }
        }

        public string RoomType
        {
            get
            {
                if (this.NumberOfRooms == 1)
                {
                    return Translate.Text(DictionaryKeys.MoveIn.Studio);
                }
                if (this.NumberOfRooms <= 7)
                {
                    return (this.NumberOfRooms - 1).ToString() + " " + Translate.Text(DictionaryKeys.MoveIn.Bedroom);
                }
                return "6+ " + Translate.Text(DictionaryKeys.MoveIn.Bedroom);
            }
        }

        public string page { get; set; }
    }
}