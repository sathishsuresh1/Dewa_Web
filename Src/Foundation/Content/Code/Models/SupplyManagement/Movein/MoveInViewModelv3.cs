using DEWAXP.Foundation.Integration.DewaSvc;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Foundation.Content.Models.SupplyManagement.Movein
{
    [Serializable]
    public class MoveInViewModelv3
    {
        public MoveInViewModelv3()
        {
            Attachment1 = new byte[0];
            Attachment2 = new byte[0];
            Attachment3 = new byte[0];
            Attachment4 = new byte[0];
            Attachment5 = new byte[0];
            Attachment6 = new byte[0];
            occupiedbyowner = true;
            isEjari = true;
        }


        #region new

        public string AccountType { get; set; }

        public string CustomerType { get; set; }

        public string CustomerTypeForNonIndividual { get; set; }

        public bool occupiedbyowner { get; set; }

        public bool isEjari { get; set; }

        public string PremiseNo { get; set; }

        public List<string> PremiseNos { get; set; }

        public string strPremiseNumberdetails { get; set; }
        public List<AddedPremiseDetails> PremiseNumberdetails { get; set; }
        public string[] PremiseAccount { get; set; }

        public string Purchase_TitleDeed { get; set; }

        public string Tenancy_Contract_Number { get; set; }
        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string BusinessPartner { get; set; }

        public string PBusinessPartner { get; set; }

        public string OBusinessPartner { get; set; }

        public string GBusinessPartner { get; set; }

        public string departmentnameid { get; set; }

        public string departmentnameothers { get; set; }

        public string CustomerCategory { get; set; }

        public string createuseraccount { get; set; }

        public bool AttachmentFlag { get; set; }

        public string IdType { get; set; }

        public string IdNumber { get; set; }

        public bool IsIdNumber { get; set; }
        public bool IsVATNumber { get; set; }
        public bool Isfirstname { get; set; }
        public bool Islastname { get; set; }
        public bool Isbp { get; set; }
        public bool IsDeptid { get; set; }

        public string DateOfBirth { get; set; }

        public string uidnumber { get; set; }

        public string ExpiryDate { get; set; }

        public string strContractStartDate { get; set; }
        public DateTime? ContractStartDate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(strContractStartDate))
                {
                    DateTime fromdateTime;
                    CultureInfo culture = global::Sitecore.Context.Culture;
                    if (culture.ToString().Equals("ar-AE"))
                    {
                        strContractStartDate = strContractStartDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    }
                    DateTime.TryParse(strContractStartDate, out fromdateTime);
                    return fromdateTime;
                }
                return null;
            }
        }

        public DateTime? MoveinStartDate { get; set; }
        public string strContractEndDate { get; set; }

        public DateTime? ContractEndDate
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(strContractEndDate))
                {
                    DateTime todateTime;
                    CultureInfo culture = global::Sitecore.Context.Culture;
                    if (culture.ToString().Equals("ar-AE"))
                    {
                        strContractEndDate = strContractEndDate.Replace("يناير", "January").Replace("فبراير", "February").Replace("مارس", "March").Replace("أبريل", "April").Replace("مايو", "May").Replace("يونيو", "June").Replace("يوليو", "July").Replace("أغسطس", "August").Replace("سبتمبر", "September").Replace("أكتوبر", "October").Replace("نوفمبر", "November").Replace("ديسمبر", "December");
                    }
                    DateTime.TryParse(strContractEndDate, out todateTime);
                    return todateTime;
                }
                return null;
            }


        }

        public int NumberOfRooms { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string PoBox { get; set; }
        public bool IsPoBox { get; set; }

        public string Emirate { get; set; }
        public bool IsEmirate { get; set; }

        public string MobilePhone { get; set; }

        public bool IsMobilePhone { get; set; }

        public string EmailAddress { get; set; }

        public bool UsemaskedEmailAddress { get; set; }

        public bool UsemaskedMobilenumber { get; set; }

        public bool IsEmailAddress { get; set; }

        public string unmaskedMobilePhone { get; set; }
        public bool IsunmaskedMobilePhone { get; set; }

        public string unmaskedEmailAddress { get; set; }
        public bool IsunmaskedEmailAddress { get; set; }

        public string Nationality { get; set; }


        public bool IsNationality { get; set; }

        public string Propertyid { get; set; }

        public string ContractValue { get; set; }

        public HttpPostedFileBase VatDocument { get; set; }

        public IEnumerable<SelectListItem> CustomerTypeList { get; set; }
        public IEnumerable<SelectListItem> AccountTypeList { get; set; }
        public IEnumerable<SelectListItem> OwnerTypeList { get; set; }
        public IEnumerable<SelectListItem> NumberOfRoomsList { get; set; }

        public IEnumerable<SelectListItem> NationalityList { get; set; }

        public IEnumerable<SelectListItem> GovernmentDDList { get; set; }

        public IEnumerable<SelectListItem> RegionList { get; set; }

        public IEnumerable<SelectListItem> IDTypeList { get; set; }

        public IEnumerable<SelectListItem> IssuingAuthorityList { get; set; }
        public IEnumerable<SelectListItem> PayChannelList { get; set; }

        public IEnumerable<SelectListItem> PBusinessPartners { get; set; }
        public IEnumerable<SelectListItem> OBusinessPartners { get; set; }
        public IEnumerable<SelectListItem> GBusinessPartners { get; set; }



        public bool isnationalsocialcard { get; set; }
        public bool isthukercard { get; set; }
        public bool issanadsmartcard { get; set; }

        public HttpPostedFileBase idupload { get; set; }

        //trenancy contract/title deed/purchase agreement
        public HttpPostedFileBase tenancycontract { get; set; }

        //tradelicense/initial approval
        public HttpPostedFileBase TradeLicense { get; set; }


        public HttpPostedFileBase nationalsocialcard { get; set; }
        public string nationalsocialcardnumber { get; set; }


        public HttpPostedFileBase thukercard { get; set; }

        public string thukercardnumber { get; set; }


        public HttpPostedFileBase sanadsmartcard { get; set; }

        public string sanadsmartcardnumber { get; set; }

        public HttpPostedFileBase guranteeleter { get; set; }

        public bool isguaranteeleter { get; set; }

        public HttpPostedFileBase Decreeletter { get; set; }

        public string Vatnumber { get; set; }

        public string IssuingAuthority { get; set; }

        public string IssuingAuthorityName { get; set; }

        public bool Owner
        {
            get
            {
                if (this.AccountType == "O")
                { return true; }
                return false;
            }
        }
        public string UserId { get; set; }
        public string FirstUserId { get; set; }
        public string TempUserId { get; set; }
        public bool IsTempUserId { get; set; }
        public bool FirstnameHide { get; set; }
        public bool LastnameHide { get; set; }
        public string Ejari { get; set; }
        public string Guranteeletterflag { get; set; }
        public string SkiptoPayment { get; set; }

        public string ShowDiscount { get; set; }

        public string SecurityDeposit { get; set; }

        public string ReconnectionRegistrationFee { get; set; }

        public string AddressRegistrationFee { get; set; }

        public string ReconnectionVATrate { get; set; }

        public string ReconnectionVATamt { get; set; }

        public string AddressVATrate { get; set; }

        public string AddressVAtamt { get; set; }
        public string OutstandingBalance { get; set; }

        public string KnowledgeFee { get; set; }

        public string TotalOutstandingFee { get; set; }

        public string InnovationFee { get; set; }

        public string paylater { get; set; }
        public string payother { get; set; }
        public bool Easypayflag { get; set; }
        public bool payotherchannelflag { get; set; }
        public string[] messagepaychannel { get; set; }
        public string[] messagewhatsnext { get; set; }

        public bool loggedinuser { get; set; }
        #endregion

        public bool maidubaigift { get; set; }
        public bool maidubaicontribution { get; set; }

        public string maidubaimsgtext { get; set; }

        public string maidubaimsgtitle { get; set; }



        public string Reference { get; set; }



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

        public byte[] Attachment6 { get; set; }

        public string Attachment6Filename { get; set; }

        public string Attachment6FileType { get; set; }

        public string PremiseType { get; set; }

        public string OwnerName { get; set; }

        public string UnitNumber { get; set; }



        public string LocationDetails { get; set; }









        public bool moveinindicator { get; set; }

        public bool LandingPage { get; set; }

        public bool CreatePage { get; set; }

        public bool ContactPage { get; set; }

        public premiseAmountDetails[] premiseAmountDetails { get; set; }

        public string[] ContractAccountnumber { get; set; }

        public bool PaymentPage { get; set; }

        public string MoveInNotificationNumber { get; set; }
        public string MovetoTransactionNumber { get; set; }

        public string[] transactionList { get; set; }

        public double Total { get; set; }

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
    public class AddedPremiseDetails
    {
        public string premisenumber { get; set; }
        public string premisebuildingname { get; set; }
    }
}