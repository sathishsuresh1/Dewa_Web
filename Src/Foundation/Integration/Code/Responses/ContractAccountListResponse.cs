using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;
using DEWAXP.Foundation.Integration.Helpers;

namespace DEWAXP.Foundation.Integration.Responses
{
  //  [XmlRoot(ElementName = "Account")]
  //  public class AccountSummary
  //  {
		//private string _accountNumber;
	 //   private string _premiseNumber;
	 //   private string _businessPartnerNumber;

		//[XmlElement(ElementName = "ContractAccountName")]
	 //   public string Name { get; set; }

		//[XmlElement(ElementName = "NickName")]
		//public string Nickname { get; set; }

  //      [XmlElement(ElementName = "BPName")]
  //      public string BPName { get; set; }

  //      [XmlElement(ElementName = "BusinessPartnerNo")]
	 //   public string BusinessPartnerNumber
	 //   {
  //          get { return DewaResponseFormatter.Trimmer(_businessPartnerNumber); }
		//    set { _businessPartnerNumber = value ?? string.Empty; }
	 //   }

		//[XmlElement(ElementName = "BillingClass")]
		//public string BillingClassCode { get; set; }

		//[XmlElement(ElementName = "ContractAccount")]
	 //   public string AccountNumber
	 //   {
  //          get { return DewaResponseFormatter.Trimmer(_accountNumber); }
		//	set { _accountNumber = value ?? string.Empty; }
		//}

	 //   [XmlElement(ElementName = "AccountCategoryText")]
  //      public string Category { get; set; }

		//[XmlElement(ElementName = "AccountCategory")]
		//public string AccountCategory { get; set; }

		//[XmlElement(ElementName = "LegacyAccount")]
  //      public string LegacyAccountNumber { get; set; }

	 //   [XmlElement(ElementName = "Premise")]
	 //   public string PremiseNumber
	 //   {
  //          get { return DewaResponseFormatter.Trimmer(_premiseNumber); }
		//	set { _premiseNumber = value ?? string.Empty; }
		//}

		//[XmlElement("CAPhoto")]
		//public string PhotoIndicator { get; set; }

		//public bool HasPhoto
		//{
		//	get { return "X".Equals(PhotoIndicator); }
		//}

	 //   public bool IsActive
	 //   {
		//    get
		//    {
		//	    return true;
		//    }
	 //   }

	 //   public BillingClassification BillingClass
		//{
		//	get
		//	{
		//		if (!string.IsNullOrEmpty(BillingClassCode))
		//		{
		//			return (BillingClassification)int.Parse(BillingClassCode);
		//		}
		//		return BillingClassification.Unknown;
		//	}
		//}

  //      [XmlElement(ElementName = "customertype")]
  //      public string CustomerType { get; set; }

  //      [XmlElement(ElementName = "moveoutrequestnumber")]
  //      public string MoveOutRequestNumber { get; set; }

  //      [XmlElement(ElementName = "NotificationNumber")]
  //      public string NotificationNumber { get; set; }
  //      [XmlElement(ElementName = "PremiseType")]
  //      public string PremiseType { get; set; }
  //      [XmlElement(ElementName = "Street")]
  //      public string Street { get; set; }
  //      [XmlElement(ElementName = "Location")]
  //      public string Location { get; set; }
  //      [XmlElement(ElementName = "XCordinate")]
  //      public string XCordinate { get; set; }
  //      [XmlElement(ElementName = "YCordinate")]
  //      public string YCordinate { get; set; }
  //      [XmlElement(ElementName = "POD")]
  //      public bool POD { get; set; }
  //      [XmlElement(ElementName = "Medical")]
  //      public bool Medical { get; set; }
  //      [XmlElement(ElementName = "Senior")]
  //      public bool Senior { get; set; }
  //  }

  //  [XmlRoot(ElementName = "ContractAccounts")]
  //  public class ContractAccounts
  //  {
  //      [XmlElement(ElementName = "Account")]
  //      public List<AccountSummary> Account { get; set; }
  //  }

  //  [XmlRoot(ElementName = "GetContractAccounts")]
  //  public class ContractAccountListResponse
  //  {
  //      [XmlElement(ElementName = "DateTimeStamp")]
  //      public string DateTimeStamp { get; set; }

  //      [XmlElement(ElementName = "ResponseCode")]
  //      public int ResponseCode { get; set; }

  //      [XmlElement(ElementName = "Description")]
  //      public string Description { get; set; }

  //      [XmlElement(ElementName = "ContractAccounts")]
  //      public ContractAccounts ContractAccounts { get; set; }
  //  }
}
