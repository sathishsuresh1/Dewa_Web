using System;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
	[Serializable]
	[XmlRoot(ElementName = "GetRERASecurityDepositDetails")]
	public class ReraGetSecurityDepositDetailsResponse
    {
		private string _contractAccountNumber;
		private string _businessPartnerNumber;

		[XmlElement(ElementName = "ResponseCode")]
		public int ResponseCode { get; set; }

		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }

		[XmlElement(ElementName = "BusinessPartnerNumber")]
		public string BusinessPartnerNumber
		{
            get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
			set { _businessPartnerNumber = value ?? string.Empty; }
		}

		[XmlElement(ElementName = "ContractAccountNumber")]
		public string ContractAccountNumber
		{
            get { return DewaResponseFormatter.Trimmer(_contractAccountNumber); }
			set { _contractAccountNumber = value ?? string.Empty; }
		}

		[XmlElement(ElementName = "CustomerName")]
        public string CustomerName { get; set; }

        [XmlElement(ElementName = "UserIDExist")]
        public string UserIdExistString { get; set; }

        public bool UserIdExist
        {
            get { return "X".Equals(UserIdExistString); }
        }

        [XmlElement(ElementName = "MoveInActive")]
        public string MoveInActive { get; set; }

        public bool IsMoveInActive
        {
            get { return "A".Equals(MoveInActive); }
        }

        [XmlElement(ElementName = "SecurityDepositAmount")]
        public string SecurityDepositAmount { get; set; }

        [XmlElement(ElementName = "Mobile")]
        public string Mobile { get; set; }

        [XmlElement(ElementName = "Email")]
        public string Email { get; set; }

        [XmlElement(ElementName = "MappedUserID")]
        public string UserId { get; set; }
    }
}
