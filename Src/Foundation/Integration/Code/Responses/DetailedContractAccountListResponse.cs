using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Extensions;

namespace DEWAXP.Foundation.Integration.Responses
{
    //[Serializable]
    //[XmlRoot(ElementName = "AccountDetails")]
    //public class AccountDetails
    //{
    //    private string _businessPartnerNumber;
    //    private string _premiseNumber;
    //    private string _accountNumber;

    //    [XmlElement(ElementName = "Electricity")]
    //    public decimal Electricity { get; set; }

    //    [XmlElement(ElementName = "Water")]
    //    public decimal Water { get; set; }

    //    [XmlElement(ElementName = "Sewerage")]
    //    public decimal Sewerage { get; set; }

    //    [XmlElement(ElementName = "Irrigation")]
    //    public decimal DM { get; set; }

    //    [XmlElement(ElementName = "Cooling")]
    //    public decimal Cooling { get; set; }

    //    [XmlElement(ElementName = "Others")]
    //    public decimal Others { get; set; }

    //    [XmlElement(ElementName = "Total")]
    //    public decimal Balance { get; set; }

    //    [XmlElement(ElementName = "Category")]
    //    public string Category { get; set; }

    //    [XmlElement(ElementName = "AccountCategory")]
    //    public string AccountCategory { get; set; }

    //    [XmlElement(ElementName = "PAY_PROCESS")]
    //    public string PayProcess { get; set; }

    //    [XmlElement(ElementName = "Name")]
    //    public string AccountName { get; set; }

    //    [XmlElement(ElementName = "LegacyAccount")]
    //    public string CustomerPremiseNumber { get; set; }

    //    [XmlElement(ElementName = "Premise")]
    //    public string PremiseNumber
    //    {
    //        get { return !string.IsNullOrEmpty(_premiseNumber) ? _premiseNumber.TrimStart('0') : string.Empty; }
    //        set { _premiseNumber = value ?? string.Empty; }
    //    }

    //    [XmlElement(ElementName = "NickName")]
    //    public string NickName { get; set; }

    //    [XmlElement(ElementName = "BPName")]
    //    public string BPName { get; set; }

    //    [XmlElement(ElementName = "constatus")]
    //    public string AccountStatusCode { get; set; }

    //    [XmlElement(ElementName = "FinalBill")]
    //    public string FinalBillCode { get; set; }

    //    [XmlElement(ElementName = "ContractAccount")]
    //    public string AccountNumber
    //    {
    //        get { return !string.IsNullOrEmpty(_accountNumber) ? _accountNumber.TrimStart('0') : string.Empty; }
    //        set { _accountNumber = value ?? string.Empty; }
    //    }

    //    [XmlElement(ElementName = "Bpartner")]
    //    public string BusinessPartnerNumber
    //    {
    //        get { return !string.IsNullOrEmpty(_businessPartnerNumber) ? _businessPartnerNumber.TrimStart('0') : string.Empty; }
    //        set { _businessPartnerNumber = value ?? string.Empty; }
    //    }

    //    [XmlElement(ElementName = "BillingClass")]
    //    public string BillingClassCode { get; set; }

    //    [XmlElement("CAPhoto")]
    //    public string PhotoIndicator { get; set; }

    //    [XmlElement(ElementName = "customertype")]
    //    public string CustomerType { get; set; }

    //    [XmlElement(ElementName = "NotificationNumber")]
    //    public string NotificationNumber { get; set; }

    //    public bool PartialPaymentPermitted
    //    {
    //        get { return !"X".Equals(FinalBillCode); }
    //    }
    //    public bool IsFinalBill
    //    {
    //        get { return "X".Equals(FinalBillCode); }
    //    }


    //    public bool HasPhoto
    //    {
    //        get { return "X".Equals(PhotoIndicator); }
    //    }

    //    public bool IsActive { get; set; }

    //    public BillingClassification BillingClass
    //    {
    //        get
    //        {
    //            if (!string.IsNullOrEmpty(BillingClassCode))
    //            {
    //                return (BillingClassification)int.Parse(BillingClassCode);
    //            }
    //            return BillingClassification.Unknown;
    //        }
    //    }

    //    public AccountClassification AccountClass
    //    {
    //        get
    //        {
    //            if (!string.IsNullOrEmpty(AccountStatusCode))
    //            {
    //                return (AccountClassification)int.Parse(AccountStatusCode);
    //            }
    //            return AccountClassification.Unknown;
    //        }
            
    //    }

    //    public string PremiseType { get; set; }
    //    public string Street { get; set; }
    //    public string Location { get; set; }
    //    public string XCordinate { get; set; }
    //    public string YCordinate { get; set; }
    //    public bool POD { get; set; }
    //    public bool Medical { get; set; }
    //    public bool Senior { get; set; }
    //}

    //[XmlRoot(ElementName = "ContractAccounts")]
    //public class DetailedContractAccountList
    //{
    //    [XmlElement(ElementName = "AccountDetails")]
    //    public List<AccountDetails> AccountDetails { get; set; }
    //}

    //[XmlRoot(ElementName = "GetAllContractAccounts")]
    //public class DetailedContractAccountListResponse
    //{
    //    [XmlElement(ElementName = "DateTimeStamp")]
    //    public string DateTimeStamp { get; set; }

    //    [XmlElement(ElementName = "ResponseCode")]
    //    public int ResponseCode { get; set; }

    //    [XmlElement(ElementName = "Description")]
    //    public string Description { get; set; }

    //    [XmlElement(ElementName = "ContractAccounts")]
    //    public DetailedContractAccountList ContractAccounts { get; set; }

    //    //[XmlElement(ElementName = "TotalBalance")]
    //    //public decimal TotalBalance { get; set; }
    //}
}
