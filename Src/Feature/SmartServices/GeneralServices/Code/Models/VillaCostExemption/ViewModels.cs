using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Models.VillaCostExemption
{
    public class DashboardPageModel
    {
        public DashboardPageModel()
        {
            this.CustomerDetails = new List<CustomerDetail>();
        }
        public List<SelectListItem> SearchOptions { get; set; }
        public string selectedSearchType { get; set; }
        public string SearchText { get; set; }
        public List<CustomerDetail> CustomerDetails { get; set; }
        public List<SelectListItem> OwnerTypeList { get; set; }
        public List<SelectListItem> StatusTypeList { get; set; }
    }

    public class CustomerDetail : BaseApplication
    {
        public string History { get; set; }
        public List<OwnerDetail> OwnerDetails { get; set; }
        public List<OwnerType> OwnerTypes { get; set; }
        public List<Ownerattachment> OwnerAttachments { get; set; }
    }

    public class BaseApplication
    {
        //public ApplicationHistory() { this.OwnerDetails = new List<OwnerDetail>(); }
        public string Number { get; set; }
        public string Reference { get; set; }
        public string Status { get; set; }
        public string StatusDesc { get; set; }
        public string CustomerNumber { get; set; }
        public string CreatedOnDateTime { get; set; }
        public string TimeofRecordCreation { get; set; }
        public string EstimateNumber { get; set; }
        public string NotificationNumber { get; set; }
        public string SequenceNumber { get; set; }
        public string OwnerType { get; set; }
        public string OwnerTypeDescription { get; set; }
        public string Remarks { get; set; }
        public string Version { get; set; }

    }
    public class Ownerattachment
    {
        public string dateofrecordcreation { get; set; }
        public string documentnumber { get; set; }
        public string filename { get; set; }
        public string ownerapplicationitemnumber { get; set; }
        public string ownerapplicationreferencenumber { get; set; }
        public string requiredflag { get; set; }
        public string timeofrecordcreation { get; set; }
        public string uploadedflag { get; set; }
    }
    public class OwnerDetail
    {
        public string OwnerID { get; set; }
        [JsonProperty("idType")]
        public string IdType { get; set; }

        [JsonProperty("itemnumber")]
        public string Itemnumber { get; set; }

        [JsonProperty("eid")]
        public string EmiratesID { get; set; }

        [JsonProperty("ownerName")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("marsoom")]
        public string Marsoom { get; set; }

        [JsonProperty("mobile1")]
        public string Mobile1 { get; set; }

        [JsonProperty("mobile2")]
        public string Mobile2 { get; set; }

        [JsonProperty("relation")]
        public string Relation { get; set; }

        [JsonProperty("passportNumber")]
        public string PassportNumber { get; set; }

        [JsonProperty("passportExpiry")]
        public string PassportExpiry { get; set; }

        [JsonProperty("issuingAuthority")]
        public string IssuingAuthority { get; set; }

        [JsonProperty("dob")]
        public string DateOfBirth { get; set; }

        [JsonProperty("nationlaity")]
        public string Nationlaity { get; set; }

        [JsonProperty("maskedemail")]
        public string MaskedEmail { get; set; }

        [JsonProperty("maskedmobile")]
        public string MaskedMobile { get; set; }
        public bool Success { get; set; } = false;
        public string ErrorMsg { get; set; }
        public HttpPostedFileBase PassportCopy { get; set; }
        public byte[] PassportCopy_Binary { get; set; }
        public string PassportCopy_AttachmentRemove2 { get; set; }
        public string DocumentNumber { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string AttachmentType { get; set; }
        public bool isPassportAttach { get; set; }
    }

    [Serializable]
    public class ApplicationViewModel
    {
        public static DEWAXP.Feature.GeneralServices.Models.VillaCostExemption.OwnerDetail FromOwnerDetail(DEWAXP.Foundation.Integration.Responses.VillaCostExemption.Ownerdetail o)
        {
            var request = new DEWAXP.Feature.GeneralServices.Models.VillaCostExemption.OwnerDetail
            {
                OwnerID = o.Itemnumber ?? string.Empty,
                IdType = o.IdType ?? string.Empty,
                Itemnumber = o.Itemnumber ?? string.Empty,
                Email = o.Email ?? string.Empty,
                EmiratesID = o.Emiratesid ?? string.Empty,
                PassportNumber = o.Passport ?? string.Empty,
                Mobile1 = o.Mobile ?? string.Empty,
                Name = o.Name ?? string.Empty,
                Relation = o.Relation ?? string.Empty,
                Mobile2 = o.Mobile2 ?? string.Empty,
                PassportExpiry = o.Passportexpiry,
                IssuingAuthority = o.PassportissueAuthority,
                DateOfBirth = o.DateOfBirth,
                Nationlaity = o.Nationlaity
            };
            return request;
        }
        public ApplicationViewModel()
        {
            this.Owners = new List<OwnerDetail>();
            this.BPList = new List<SelectListItem>();
            this.PropertyOwnerTypeList = new List<SelectListItem>();
            this.Applications = new List<BPFrontEnd>();
            this.Estimates = new List<BPFrontEnd>();
        }
        public string VersionNumber { get; set; }
        public string ApplicationNumber { get; set; }
        public string ApplicationReferenceNumber { get; set; }
        public string AapplicationSequenceNumber { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string DateOfSubmission { get; set; }
        public string BusinessPartner { get; set; }
        public string EstimateNumber { get; set; }
        public string OwnerType { get; set; }
        public string OwnerTypeText { get; set; }

        public List<OwnerDetail> Owners { get; set; }
        public List<SelectListItem> BPList { get; set; }
        public string BPDetails { get; set; }
        //public SelectList EstimateNumbersList { get; set; }
        public List<SelectListItem> PropertyOwnerTypeList { get; set; }
        public List<BPFrontEnd> Applications { get; set; }
        public List<BPFrontEnd> Estimates { get; set; }
        public List<Attachlist> AttachmentLists { get; set; }
        public List<DEWAXP.Foundation.Integration.Responses.VillaCostExemption.CountryList> Countrylists { get; set; }
        public string CustomerComments { get; set; }
        public string OwnersJson { get; set; }
        public int Stage { get; set; }
        public int IsSubmit { get; set; } = 0;
        public static string ToJson(List<BPFrontEnd> o)
        {
            if (o == null) { return "{}"; }
            return JsonConvert.SerializeObject(o);
        }
    }

    public class Attachlist
    {
        public HttpPostedFileBase ConstructionContractSigned { get; set; }
        public byte[] ConstructionContractSigned_Binary { get; set; }
        public string ConstructionContractSigned_AttachmentRemove1 { get; set; }
        public HttpPostedFileBase ConstructionContract_02 { get; set; }
        public byte[] ConstructionContract_02_Binary { get; set; }
        public string ConstructionContract_02_AttachmentRemove2 { get; set; }

        public HttpPostedFileBase ConstructionContract_03 { get; set; }
        public byte[] ConstructionContract_03_Binary { get; set; }
        public string ConstructionContract_03_AttachmentRemove3 { get; set; }

        public HttpPostedFileBase ConstructionContract_04 { get; set; }
        public byte[] ConstructionContract_04_Binary { get; set; }
        public string ConstructionContract_04_AttachmentRemove4 { get; set; }

        public HttpPostedFileBase CompletionLetter { get; set; }
        public byte[] CompletionLetter_Binary { get; set; }
        public string CompletionLetter_AttachmentRemove5 { get; set; }

        public string DocumentNumber { get; set; }
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string AttachmentType { get; set; }

    }
    public class BPFrontEnd
    {
        public string Id { get; set; }
        public List<string> Values { get; set; }
        public bool IsSubmittedApp { get; set; } = false;
    }

    public class OwnerType
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
    public class BPartnerDetail
    {
        public string RN { get; set; }
        public string EID { get; set; }
        public string EN { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Version { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Telephone { get; set; }
        public string Relationship { get; set; }
        public string Nationality { get; set; }
        public string Valid { get; set; }
    }

    public enum AttachmentType
    {
        ConstructionContractSigned = 01,
        ConstructionContract_02 = 02,
        ConstructionContract_03 = 03,
        ConstructionContract_04 = 04,
        CompletionLetter = 05,
        Passport = 06
    }
}