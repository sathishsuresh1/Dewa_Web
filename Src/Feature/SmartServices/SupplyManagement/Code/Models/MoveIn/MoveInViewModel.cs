using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Globalization;
using System.Web.Mvc;
using System.Globalization;
using DEWAXP.Foundation.Integration.DewaSvc;
using DEWAXP.Foundation.Content;

namespace DEWAXP.Feature.SupplyManagement.Models.MoveIn
{
    #region old
    [Serializable]
	public class MoveInViewModel
	{
		public MoveInViewModel()
		{
			Attachment1 = new byte[0];
			Attachment2 = new byte[0];
			Attachment3 = new byte[0];
			Attachment4 = new byte[0];
            Attachment5 = new byte[0];
		}

		public List<string> BusinessPartners { get; set; }

		public List<string> CustomerTypes { get; set; }

		public List<string> AccountTypes { get; set; }

		public List<string> PremiseNos { get; set; }

		public string UserId { get; set; }

		public string Password { get; set; }

		public string ConfirmPassword { get; set; }

		public string BusinessPartner { get; set; }

		public string CustomerCategory { get; set; }

        public string createuseraccount { get; set; }

        public bool AttachmentFlag { get; set; }

		public string AccountType { get; set; }

		public string CustomerType { get; set; }

		public string PremiseNo { get; set; }
		
		public string IdType { get; set; }

		public string IdNumber { get; set; }

        public DateTime? ExpiryDate { get; set; }

		public DateTime? ContractStartDate { get; set; }

        public DateTime? MoveinStartDate { get; set; }

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

        public string Ejari { get; set; }

        public string ProcessMovein { get; set; }

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

        public string PremiseType { get; set; }

		public string OwnerName { get; set; }

		public string UnitNumber { get; set; }

		public string LocationDetails { get; set; }

		public string SecurityDeposit { get; set; }

		public string ReconnectionRegistrationFee { get; set; }

        public string AddressRegistrationFee { get; set; }

        public string ReconnectionVATrate { get; set; }

        public string ReconnectionVATamt { get; set; }

        public string AddressVATrate { get; set; }

        public string AddressVAtamt { get; set; }

        public string OutstandingBalance { get; set; }

        public string KnowledgeFee { get; set; }

        public string InnovationFee { get; set; }

        public string[] PremiseAccount { get; set; } 

        public bool Owner { get; set; }

        public bool moveinindicator { get; set; }

        public bool LandingPage { get; set; }

        public bool CreatePage { get; set; }

        public bool ContactPage { get; set; }

        public bool TenantPage { get; set; }

        public bool PaymentPage { get; set; }

        public string Vatnumber { get; set; }

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
	}
    #endregion
    

        internal static class AttachmentTypeCodes
	{
		public const string TenancyContract = "TC1";

		public const string EmiratesIdDocument = "ED1";

		public const string EmiratesIdDocumentCopy = "ED2";

		public const string Passport = "PP1";

		public const string PassportCopy = "PP2";

		public const string TradeLicense = "TD1";

        public const string PurchaseAgreement = "PA1";

		public const string PurchaseAgreementCopy = "PA2";

        public const string VatDocument = "VAT";

        public const string GuaranteeLetter = "GL";

        public const string InitialApproval = "IA";

        public const string NationalSocialcard = "NSC";

        public const string Thukercard = "TC";

        public const string SanadSmartcard = "SSC";

        public const string DecreeLetter = "DL";


    }

    public class moveinpassedmodel
    {
        public string idtype { get; set; }
        public string businesspartner { get; set; }
        public string moveoutaccount { get; set; }
        public string moveoutdate { get; set; }
        public string idnumber { get; set; }
        public string customerCategory { get; set; }
        public string customerType { get; set; }
        public string accounttype { get; set; }
        public string occupiedby { get; set; }
        public string ejarinumber { get; set; }
        public string contractnumber { get; set; }
        public string startdate { get; set; }
        public string enddate { get; set; }
        public string contractvalue { get; set; }
        public string numberofrooms { get; set; }
        public string[] premiselist { get; set; }
    }

}