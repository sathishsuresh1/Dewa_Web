using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DEWAXP.Foundation.Integration.Responses;

namespace DEWAXP.Foundation.Content.Models.AccountModel
{
    [Serializable]
    public class UpdateCustomerInfoModel
    {
        #region "Manage Customer Information"
        public string Success { get; set; }
        public string BusinessPartnerNumber { get; set; }
        public string BusinessPartnerType { get; set; }
        public string AccountNumberSelected { get; set; }
        public string CustomerName { get; set; }
        public string EmailAddress { get; set; }
        public string MobileNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string HiddenEmailAddress { get; set; }
        public string HiddenMobileNumber { get; set; }
        public string POBox { get; set; }
        public string mailVerifyPending { get; set; }
        public string SelectedEmirateKey { get; set; }
        public string EmirateId { get; set; }
        public string EmirateExpDate { get; set; }
        public string NameUpdateExist { get; set; }
        public string PassportUpdateExist { get; set; }
        public string EmirateIdUpdateExist { get; set; }

        public string PassportExpired { get; set; }
        public string EmirateIdExpired { get; set; }
        public string TradelicenseExpired { get; set; }
        public string TradelicenseUpdateExist { get; set; }
        public string SelectedBusinessPartner { get; set; }
        public List<SelectListItem> BusinessPartnerList { get; set; }
        public HttpPostedFileBase EmirateIdUploader { get; set; }
        public string PassportNo { get; set; }
        public string PassportExpDate { get; set; }
        public HttpPostedFileBase PassportUploader { get; set; }
        public string SelectedNationality { get; set; }
        public List<SelectListItem> NationalityList { get; set; }

        public string TrandeLicenseNo { get; set; }
        public string TrandeLicenseExpDate { get; set; }
        public HttpPostedFileBase TrandeLicenseUploader { get; set; }
        public string SelectedIssuingAuthority { get; set; }
        public List<SelectListItem> IssuingAuthorityList { get; set; }
        public string SelectedPrefferedLanguage { get; set; }
        public List<SelectListItem> PrefferedLanguageList { get; set; }

        public UpdateCustomerInfoSuccessModel UpdateCustomerInfoSuccessModel { get; set; }
        public string OtpRequestId { get; set; }
        public string RequestType { get; set; }
        #endregion

        #region "Manage Communication"
        public string GeneralCommSMS { get; set; }
        public string GeneralCommEmail { get; set; }
        public string ShamsDubaiSMS { get; set; }
        public string ShamsDubaiEmail { get; set; }
        public List<BPCommunication> BPCommunications { get; set; }
        public AccountDetails[] Accounts { get; set; }
        public List<string> SelectedGeneralComms { get; set; }
        public List<string> SelectedShamsComms { get; set; }

        public string postedCommunication { get; set; }
        #endregion

        #region "Additional Personal Details"
        public string SelectedMaritalStatus { get; set; }
        public IEnumerable<SelectListItem> MaritalStatusList { get; set; }
        public string SelectedMonthlyIncome { get; set; }
        public List<SelectListItem> MonthlyIncomeList { get; set; }
        public string SelectedTechnologyRating { get; set; }
        public IEnumerable<SelectListItem> TechnologyUsage { get; set; }
        public string SelectedPreferredMedia { get; set; }
        public IEnumerable<string> MultiplePreferredMedia { get; set; }
        public string OtherMediaChannel { get; set; }
        public List<SelectListItem> PreferredMediaList { get; set; }
        #endregion

        #region "People of Determination & Medical Situation"
        public string SelectedAccountNumber { get; set; }
        public List<SelectListItem> CustomerAccountList { get; set; }
        public string ResidenceType { get; set; }
        public List<string> SelectedPODType { get; set; }
        public List<SelectListItem> PODTypesList { get; set; }
        public string SelectedPODCategory { get; set; }
        public List<SelectListItem> PODCategoryList { get; set; }
        public string SelectedEquipment { get; set; }
        public List<SelectListItem> EquiExistList { get; set; }
        public List<SelectListItem> RelationList { get; set; }
        public string SelectedDisabilityType { get; set; }
        public string OtherDisabilityType { get; set; }

        public List<SelectListItem> DisabilityTypeList { get; set; }
        public List<string> MultipleDisableTypes { get; set; }
        public string SelectedMedicalSituationType { get; set; }
        public string OtherMedicalSituationType { get; set; }

        public List<string> MultipleMedicalSituationTypes { get; set; }
        public List<SelectListItem> MedicalSituationTypeList { get; set; }
        public string postedPOD { get; set; }
        public string DeterminationID { get; set; }
        public bool IsMyselfPOD { get; set; }
        public HttpPostedFileBase AccountHolderEmirateID { get; set; }
        public HttpPostedFileBase DeterminationIDCopy { get; set; }
        public List<PODMembers> FamilyMembers { get; set; }
        #endregion
    }
    public class BPCommunication
    {
        public string CommunicationType { get; set; }
        public bool SubscribeEmail { get; set; }
        public bool SubscribeSMS { get; set; }
    }
    public class PODMembers
    {
        public string Action { get; set; }
        public string AccountNumber { get; set; }
        public string FamilyMemberName { get; set; }
        public string Relationship { get; set; }
        public string OtherRelationship { get; set; }
        public string PODCategory { get; set; }
        public string DisabilityType { get; set; }
        public string OtherDisabilityType { get; set; }
        public string MedicalSituationType { get; set; }
        public string OtherMedicalSituationType { get; set; }
        public string MedicalEquipmentType { get; set; }
        public List<string> MultipleMedicalSituationTypes { get; set; }
        public List<string> MultipleDisableTypes { get; set; }
        public string PODNumber { get; set; }
        public string EmirateIdFileName { get; set; }
        public string DeterminationIdFileName { get; set; }
        public byte[] EmirateId { get; set; }
        public byte[] DeterminationId { get; set; }
        public string PODType { get; set; }
    }
    public class UpdateCustomerInfoSuccessModel
    {
        public string AccountName { get; set; }

        public string AccountNumber { get; set; }
        public string AccountNumberSelected { get; set; }

        public string BusinessPartnerNumber { get; set; }

        public string NickName { get; set; }

        public string MobileNumber { get; set; }

        public string EmailAddress { get; set; }

        public string PoBox { get; set; }

        public string Emirate { get; set; }

        public string PreferredLanguage { get; set; }
    }
}