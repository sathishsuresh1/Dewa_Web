using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Foundation.DataAnnotations;
using DEWAXP.Foundation.Integration.CustomerSmartSalesSvc;

namespace DEWAXP.Feature.ScrapSale.Models.ScrapSale
{
    [Serializable]
    public class ProfileUpdateModel
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string Region { get; set; }
        public string POBox { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmiratesID { get; set; }
        public string Countrykey { get; set; }
        public string PassportNumber { get; set; }
        public string DOB { get; set; }
        public string customeraccountgroup { get; set; }
        public string City { get; set; }
        public string ActualCity { get; set; }
        public string Street { get; set; }

        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase EIDAttachment { get; set; }

        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase PasspportAttachment { get; set; }

        [MaxFileSize(2 * 1024 * 1024, ValidationMessageKey = "The file may not be bigger than 2MB")]
        public HttpPostedFileBase TradeLicenseAttachment { get; set; }
        public string TradelicenseNumber { get; set; }

        public bool IsSuccess { get; set; }
        public string ApplicationNO { get; set; }

        public List<SelectListItem> CountryList { get; set; }
        public List<SelectListItem> RegionList { get; set; }

        public List<SelectListItem> TradeLicenseIssuingAuthority { get; set; }

        public string TradeLicenseIssuingAuthorityKey { get; set; }

        public string TradeLicenseIssueDate { get; set; }
        public string TradeLicenseExpiryDate { get; set; }

        public string VatRegistrationNo { get; set; }

        public string CompanyTelephone { get; set; }

        public string CompanyTelephoneExtension { get; set; }

        public string IndividualCompanyTelephone { get; set; }

        public string IndividualCompanyTelephoneExtension { get; set; }

        public string CompanyMobileNumber { get; set; }

        public string CompanyEmail { get; set; }

        public string UserType { get; set; }

        public List<SelectListItem> AreaList { get; set; }

        public string Area { get; set; }

        public string OtpRequestId { get; set; }

        public bool IsTradeLicenseDocRequired { get; set; }

        public string IssuingAuthorityDescription { get; set; }

        public string ExpiryDate { get; set; }

        public List<SelectListItem> CompanyTelephoneCodeList { get; set; }
        public string CompanyTelephoneCode { get; set; }
        public string CompanyMobileCode { get; set; }
        public string MobileCode { get; set; }
        public string TelephoneCode { get; set; }
        public string TradenumberFlag { get; set; }
        public string IssuedateFlag { get; set; }
        public string ExpirydateFlag { get; set; }
        public string AttachmentFlag { get; set; }
        public string IssueauthorityFlag { get; set; }
    }
}