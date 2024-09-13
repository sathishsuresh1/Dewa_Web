// <copyright file="RegistrationViewModel.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

using DEWAXP.Foundation.CustomDB.DRRGDataModel;
using DEWAXP.Foundation.Content.Extensions;
using Sitecore.Globalization;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using DEWAXP.Foundation.DataAnnotations;
using DEWAXP.Foundation.Helpers.Extensions;

namespace DEWAXP.Feature.DRRG.Models
{
    /// <summary>
    /// Defines the <see cref="RegistrationViewModel" />.
    /// </summary>
    public class RegistrationViewModel
    {
        public RegistrationViewModel()
        {
            NationalityList = FormExtensions.GetNationalities(null,false);
            //FactoryList = new List<Factory>();
            UserLocalRepresentative = true;
            UserGender = true;
            updateprofile = false;
            FileList = new Dictionary<string, fileResult>();
        }
        public long ManufacturerId { get; set; }
        public string ManufacturerCode { get; set; }
        public string ManufacturerFullName { get; set; }
        public string BrandName { get; set; }
        public List<SelectListItem> NationalityList { get; set; }
        public List<SelectListItem> CountryMobileList { get; set; } 
        public string Manufacturercountry { get; set; }
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string ManufacturerEmailAddress { get; set; } 
        public string ManufacturerPhonecode { get; set; } 
        public string ManufacturerPhoneNumber { get; set; }
        public string ManufacturerFaxcode { get; set; }
        public string ManufacturerFaxNumber { get; set; }
        public string Website { get; set; }
        public HttpPostedFileBase TradeMarkLogo { get; set; }
        public string TradeMarkLogo_FileName { get; set; }
        public byte[] TradeMarkLogoBinary { get; set; }
        public Factory[] factory { get; set; }
        public string StrfactoryList { get; set; }
        public bool UserLocalRepresentative { get; set; }
        public bool UserGender { get; set; }
        public string Userrepresentativename { get; set; }
        public string Userfirstname { get; set; }
        public string UserLastName { get; set; }
        public string UserCountry { get; set; }
        public string UserDesignation { get; set; }
        public string UserPhonecode { get; set; }
        public string UserPhoneNumber { get; set; }
        [EmailAddress(ValidationMessageKey = "email validation message")]
        public string UserEmail { get; set; }
        public List<HttpPostedFileBase> SupportingDocument { get; set; }
        public bool updateprofile { get; set; }
        public HttpPostedFileBase TradeLicenseDocument { get; set; }
        public byte[] TradeLicenseDocumentBinary { get; set; }
        public string TradeLicenseDocument_FileName { get; set; }
        public string signatureCopy { get; set; }
        public Dictionary<string, fileResult> FileList { get; set; }
    }
    public class Factory
    {
        public string FactoryCode { get; set; }
        public string FactoryFullName { get; set; }
        public string FactoryCountry { get; set; }
        public string FactoryAddress { get; set; }
        public string EOLPVModule { get; set; }
        public HttpPostedFileBase EOLFile { get; set; }
        public HttpPostedFileBase QMSFile { get; set; }
        public HttpPostedFileBase EnvironmentalFile { get; set; }
        public byte[] EOLFileBinary { get; set; }
        public byte[] QMSFileBinary { get; set; }
        public byte[] EnvironmentalFileBinary { get; set; }
        public string EOLFileName { get; set; }
        public string QMSFileName { get; set; }
        public string EnvironmentalFileName { get; set; }
        public string EOLFilemarker { get; set; }
        public string QMSFilemarker { get; set; }
        public string EnvironmentalFilemarker { get; set; }
    }

    internal static class FileType
    {
        public const string TrademarkLogo = "TRADEMARKLOGO";
        public const string SupportingDocument = "SUPPORTINGDOCUMENT";
        public const string EOLPVModule = "EOLPVMODULE";
        public const string QualityManagementSupport = "QUALITYMANAGEMENTSUPPORT";
        public const string EnvironmentalManagementSupport = "ENVIRONMENTALMANAGEMENTSUPPORT";
        public const string AuthorizationLetter = "AUTHORIZATIONLETTER";
        public const string RejectedFile = "REJECTEDFILE";
        public const string TradeLicenseDoc = "TRADELICENSEDOC";
        public const string SignatureCopy = "SIGNATURE";

        #region PV Module FileTypes
        public const string ModelDataSheet = "MODELDATASHEET";
        public const string IEC61215 = "IEC61215-1-X/2";
        public const string IEC61730 = "IEC61730-1/2";
        public const string IEC61701 = "IEC61701";
        public const string IEC62716 = "IEC62716";
        public const string IEC60068 = "IEC60068-2-68";
        #endregion
        #region Inverter Module Filetypes
        public const string IEC62109 = "IEC62109-1/2";
        public const string UL1741 = "UL1741";
        public const string IEC6100032 = "IEC61000-3-2";
        public const string IEC61000312 = "IEC61000-3-12";
        public const string IEC6100061 = "IEC61000-6-1";
        public const string IEC6100062 = "IEC61000-6-2";
        public const string IEC6100063 = "IEC61000-6-3";
        public const string IEC6100064 = "IEC61000-6-4";
        public const string HarmonicSpectrum = "HarmonicSpectrum";
        #endregion

        #region Interface Module Filetypes
        public const string IEC61850 = "IEC61850";
        public const string DEWADRRGSTANDARDS = "DEWADRRGSTANDARDS";
        public const string IEC610101 = "IEC61010-1";
        #endregion
    }
    public static class FileEntity
    {
        public const string Manufacturer = "MANUFACTURER";
        public const string Factory = "FACTORY";
        public const string PVmodule = "PVMODULE";
        public const string Interfacemodule = "INTERFACEMODULE";
        public const string Invertermodule = "INVERTERMODULE";
        public const string EvaluatorRejection = "EVALUATORREJECTION";

    }
    public static class NominalPowerType
    {
        public static string NominalPowerRange = "Nominal Power Range";
        public static string MultipleNominalPowerEntries = "Multiple Nominal Power Entries";
        public static string SingleNominalPowerEntry = "Single Nominal Power Entry";

    }
    internal static class UserGender
    {
        public const string Male = "MALE";
        public const string Female = "FEMALE";
    }

    internal static class DRRGERRORCODE
    {
        public static string Useremailalreadyexist = Translate.Text("DRRG.Useralreadyexists");
        public static string UserdoesntExists = Translate.Text("DRRG.UserdoesntExists");
        public static string IncorrectPassword = Translate.Text("DRRG_IncorrectPassword");
        public static string CheckLink = Translate.Text("DRRG_checklink");
        public static string SetPasswordRegistration = Translate.Text("DRRG.set password registration");
        public static string SetPasswordForgot = Translate.Text("DRRG.set password Forgot");
        public static string Noteliglibleuser = Translate.Text("DRRG.Noteliglibleuser");
    }

    internal static class DRRGStandardValues
    {
        public static string Registration = "Registration";
        public static string UpdateProfile = "UpdateProfile";
        public static string ForgotPassword = "ForgotPassword";
        public static string Success = "Success";
        public static string Rejected = "Rejected";
        public static string UserExists = "UserExists"; 
        public static string responseMessage = "responseMessage";
        public static string error = "error";
        public static string status = "status";
        public static string sessionout = "sessionout";
        public static string Usernotevaluated = "Usernotevaluated";
        public static string UserdoesntExists = "UserdoesntExists"; 
        public static string UserPendingEvaualtion = "UserPendingEvaualtion";
        public static string Incorrectpassword = "Incorrect password"; 
        public static string name = "name";
        public static string manufactercode = "manufactercode";
        public static string evaluatorcode = "evaluatorcode";
        public static string notevaluated = "notevaluated";
        public static string disabled = "disabled";
        public static string firstname = "firstname";
        public static string lastname = "lastname";
        public static string manufacturername = "manufacturername";
        public static string manufactureremail = "manufactureremail";
        public static string logofile = "logofile";
        public static string logofileName = "logofileName";
        public static string logoContentType = "logoContentType";
        public static string logoExtension = "logoExtension";
        public static string Submitted = "Submitted";
        public static string Updated = "Updated";
        public static string AuthorizedLetterSubmitted = "AuthorizedLetterSubmitted";
        public static string AuthorizedLetterUpdated = "AuthorizedLetterUpdated";
        public static string SchemaManagerRejected = "Rejected";
        public static string Approved = "Approved";
        public static string EvaluatorRegistration = "EvaluatorRegistration";
        public static string Registrationapproved = "Registrationapproved";
        public static string Registrationrejected = "Registrationrejected";
        public static string ltsubmitted = "ltsubmitted";
        public static string ltupdated = "ltupdated";
        public static string ltrejected = "ltrejected";
        public static string equipmentcount = "equipmentcount";
        public static string ADMIN = "ADMIN";
        public static string ManufacturerRejected = "ManufacturerRejected";
        public static string PVModuleRejected = "PVModuleRejected";
        public static string IVModuleRejected = "IVModuleRejected";
        public static string IPModuleRejected = "IPModuleRejected";
        public static string PVModuleApproved = "PVModuleApproved";
        public static string IVModuleApproved = "IVModuleApproved";
        public static string IPModuleApproved = "IPModuleApproved";
        public static string EvaluatorRegistrationApproved = "EvaluatorRegistrationApproved";
        public static string EvaluatorRegistrationRejected = "EvaluatorRegistrationRejected";


    }
}
