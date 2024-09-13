// <copyright file="DRRGBaseController.cs">
// Copyright (c) 2022
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.DRRG.Controllers
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.Integration.InternshipSvc;
    using DEWAXP.Foundation.CustomDB.DRRGDataModel;
    using DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG;
    using DEWAXP.Feature.DRRG.Models;
    using Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Sitecorex = Sitecore;
    using Sitecore.Data.Items;
    using System.DirectoryServices.Protocols;
    using System.Net;
    using System.DirectoryServices.AccountManagement;
    using System.Data.Entity.Core.Objects;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers;
    using Roles = Foundation.Content.Roles;
    using DEWAXP.Foundation.Content.Services;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Feature.DRRG.Helpers;
    using Aspose.Pdf.Operators;

    /// <summary>
    /// Defines the <see cref="DRRGBaseController" />.
    /// </summary>
    public class DRRGBaseController : BaseController
    {
        /// <summary>
        /// Defines the _lock.
        /// </summary>
        private static readonly object _lock = new object();

        /// <summary>
        /// Defines the _efConn.
        /// </summary>
        protected string _efConn = ConfigurationManager.ConnectionStrings["DRRGEntities"].ConnectionString;

        private string passphrase = ConfigurationManager.AppSettings["DRRG_PassPhrase"];// "DRRG$et9@$$";
        private string DRRG_PORTAL_WEBURL = ConfigurationManager.AppSettings["DRRG_PORTAL_WEBURL"];
        private string DRRG_PORTAL_ADMINURL = ConfigurationManager.AppSettings["DRRG_PORTAL_ADMINURL"];

        internal void ClearCookiesSignOut()
        {
            DewaApiClient.Logout(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken);
            FormsAuthentication.SignOut();

            if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
            {
                Response.Cookies[GenericConstants.AntiHijackCookieName].Value = string.Empty;
                Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddYears(-1);
            }
        }
        /// <summary>
        /// The GetCountryMobilelist.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public List<SelectListItem> GetCountrylist()
        {
            List<SelectListItem> countrycodelist = new List<SelectListItem>();
            try
            {
                Foundation.Integration.Responses.ServiceResponse<GetInternshipHelpValuesResponse> CountryCoderesponse = IntershipServiceClient.GetHelpValues(new GetInternshipHelpValues { countrycodes = "X" }, RequestLanguage);
                if (CountryCoderesponse != null && CountryCoderesponse.Succeeded && CountryCoderesponse.Payload != null && CountryCoderesponse.Payload.@return != null && CountryCoderesponse.Payload.@return.countrycodes != null)
                {
                    countrycodelist = CountryCoderesponse.Payload.@return.countrycodes
                   .Select(p => new SelectListItem()
                   {
                       Text = p.countryname,
                       Value = p.countrytelephonecode,

                   }
                   ).ToList();
                    countrycodelist.Where(X => X.Value.Equals("971")).ToList().ForEach(y => y.Selected = true);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return countrycodelist;
        }
        /// <summary>
        /// The GetCountryMobilelist.
        /// </summary>
        /// <returns>The <see cref="List{SelectListItem}"/>.</returns>
        public List<SelectListItem> GetCountryMobilelist()
        {
            List<SelectListItem> countrycodelist = new List<SelectListItem>();
            try
            {
                Foundation.Integration.Responses.ServiceResponse<GetInternshipHelpValuesResponse> CountryCoderesponse = IntershipServiceClient.GetHelpValues(new GetInternshipHelpValues { countrycodes = "X" }, RequestLanguage);
                if (CountryCoderesponse != null && CountryCoderesponse.Succeeded && CountryCoderesponse.Payload != null && CountryCoderesponse.Payload.@return != null && CountryCoderesponse.Payload.@return.countrycodes != null)
                {
                    countrycodelist = CountryCoderesponse.Payload.@return.countrycodes
                   .Select(p => new SelectListItem()
                   {
                       Text = "+" + p.countrytelephonecode + " (" + p.countryname + ")",
                       Value = p.countrytelephonecode,

                   }
                   ).ToList();
                    countrycodelist.Where(X => X.Value.Equals("971")).ToList().ForEach(y => y.Selected = true);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return countrycodelist;
        }

        /// <summary>
        /// The ManufacturerCode.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string ManufacturerCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "MANUF", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        /// <summary>
        /// The FactoryCode.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string FactoryCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "FACTORY", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        public string PVModuleCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "PV", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        public string InverterModuleCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "INV", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        public string InterfaceModuleCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "IP", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        public string EvaluatorCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "EVR", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        public string RejectedCode()
        {
            lock (_lock)
            {
                AlphaNumericStringGenerator al = new AlphaNumericStringGenerator();
                return string.Format("{0}{1}{2}", "REJECT", DateTime.Now.ToString("MMdd"), al.GetRandomUppercaseAlphaNumericValue(8));
            }
        }

        /// <summary>
        /// The GetDRRGErrormessage.
        /// </summary>
        /// <param name="msg">The msg<see cref="string"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public string GetDRRGErrormessage(string msg)
        {
            string message = string.Empty;
            switch (msg)
            {
                case "Useremailalreadyexist":
                    message = DRRGERRORCODE.Useremailalreadyexist;
                    break;
                case "UserdoesntExists":
                    message = DRRGERRORCODE.UserdoesntExists;
                    break;
                case "Incorrect password":
                    message = Translate.Text("InvalidCredential_ErrorMessage");
                    break;
            }
            return message;
        }

        public string GetDRRGSetPasswordmessage(string msg)
        {
            string message = Translate.Text("AR.Password_reset_successful_Success");
            switch (msg)
            {
                case "Registration":
                    message = DRRGERRORCODE.SetPasswordRegistration;
                    break;
                case "ForgotPassword":
                    message = DRRGERRORCODE.SetPasswordForgot;
                    break;
            }
            return message;
        }

        /// <summary>
        /// The Addfile.
        /// </summary>
        /// <param name="file">The file<see cref="HttpPostedFileBase"/>.</param>
        /// <param name="filetype">The filetype<see cref="string"/>.</param>
        /// <param name="entity">The entity<see cref="string"/>.</param>
        /// <param name="code">The code<see cref="string"/>.</param>
        /// <param name="error">The error<see cref="string"/>.</param>
        /// <param name="FileSizeLimit">The FileSizeLimit<see cref="long"/>.</param>
        /// <param name="supportedTypes">The supportedTypes<see cref="string[]"/>.</param>
        /// <param name="dRRG_Files_TYlist">The dRRG_Files_TYlist<see cref="List{DRRG_Files_TY}"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        protected string Addfile(HttpPostedFileBase file, string filetype, string entity, string code, string error, long FileSizeLimit, string[] supportedTypes, string ManufacturerCode, List<DRRG_Files_TY> dRRG_Files_TYlist)
        {
            try
            {
                if (file != null && file.ContentLength > 0)
                {
                    error = string.Empty;
                    if (CustomeAttachmentIsValid(file, FileSizeLimit, out error, supportedTypes))
                    {
                        using (MemoryStream memoryStream_8 = new MemoryStream())
                        {
                            file.InputStream.CopyTo(memoryStream_8);
                            dRRG_Files_TYlist.Add(new DRRG_Files_TY
                            {
                                Name = file.FileName,
                                ContentType = file.ContentType,
                                Extension = file.GetTrimmedFileExtension(),
                                File_Type = filetype,
                                Entity = entity,
                                Content = memoryStream_8.ToArray() ?? new byte[0],
                                Size = file.ContentLength.ToString(),
                                Reference_ID = code,
                                Manufacturercode = ManufacturerCode
                            });
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, filetype + " - " + error);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return error;
        }
        public byte[] GenerateDecalartionLetter(string regNumber, PVModule pvModel, InverterModule inverterModel, InterfaceModule interfaceModel)
        {
            byte[] declarationLetterBytes = null;
            string body = string.Empty;
            string DecalartionLetterText = string.Empty;

            try
            {
                if (!string.IsNullOrWhiteSpace(regNumber) && regNumber.ToLower().StartsWith("pv"))
                {
                    Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_PV_DECLARATION_LETTER);
                    if (item != null)
                    {
                        DecalartionLetterText = @"<!DOCTYPE html>
                                    <html>
                                    <head>
                                        <meta charset='utf-8'>
                                        <title></title>
                                    </head>
                                    <body>" + item["Rich Text"] + "</body></html>";
                        DecalartionLetterText = DecalartionLetterText.Replace("{currentdate}", DateTime.Now.ToString())
                                                                     .Replace("{modelname}", pvModel.ModelName);
                        if (pvModel.NominalPower.Equals(NominalPowerType.SingleNominalPowerEntry))
                        {
                            DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", pvModel.Singlenominalpower);
                        }
                        else if (pvModel.NominalPower.Equals(NominalPowerType.NominalPowerRange) && pvModel.MultinominalPower != null && pvModel.MultinominalPower.Count > 0)
                        {
                            DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", "" + pvModel.MultinominalPower[0] + " Wp - " + pvModel.MultinominalPower[1] + " Wp , in step of " + pvModel.MultinominalPower[2] + " W");
                        }
                        else if (pvModel.NominalPower.Equals(NominalPowerType.MultipleNominalPowerEntries) && !string.IsNullOrWhiteSpace(pvModel.NumberofMultinominals))
                        {
                            var numberofnominal = Convert.ToInt32(pvModel.NumberofMultinominals);
                            switch (numberofnominal)
                            {
                                case 1:
                                    DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", "" + pvModel.pVMultiNominals[0].wp1 + " Wp");
                                    break;
                                case 2:
                                    DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", "" + pvModel.pVMultiNominals[0].wp1 + " Wp, " + pvModel.pVMultiNominals[1].wp1 + " Wp");
                                    break;
                                case 3:
                                    DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", "" + pvModel.pVMultiNominals[0].wp1 + " Wp, " + pvModel.pVMultiNominals[1].wp1 + " Wp, " + pvModel.pVMultiNominals[2].wp1 + " Wp");
                                    break;
                                case 4:
                                    DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", "" + pvModel.pVMultiNominals[0].wp1 + " Wp, " + pvModel.pVMultiNominals[1].wp1 + " Wp, " + pvModel.pVMultiNominals[2].wp1 + " Wp, " + pvModel.pVMultiNominals[3].wp1 + " Wp");
                                    break;
                                case 5:
                                    DecalartionLetterText = DecalartionLetterText.Replace("{nominalpower}", "" + pvModel.pVMultiNominals[0].wp1 + " Wp, " + pvModel.pVMultiNominals[1].wp1 + " Wp, " + pvModel.pVMultiNominals[2].wp1 + " Wp, " + pvModel.pVMultiNominals[3].wp1 + " Wp, " + pvModel.pVMultiNominals[4].wp1 + " Wp");
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(pvModel.CellTechnology) && !pvModel.CellTechnology.ToLower().Equals("others"))
                            DecalartionLetterText = DecalartionLetterText.Replace("{celltechnology}", pvModel.CellTechnology);
                        else
                            DecalartionLetterText = DecalartionLetterText.Replace("{celltechnology}", pvModel.OtherCellTechnology);

                        var strCellStructure = string.Empty;
                        foreach (var cellstructure in pvModel.CellStructure)
                        {
                            if (!string.IsNullOrWhiteSpace(cellstructure) && !cellstructure.ToLower().Equals("others"))
                                strCellStructure += cellstructure + ", ";
                            else
                                strCellStructure += pvModel.OtherCellStructure + ", ";
                        }

                        DecalartionLetterText = DecalartionLetterText.Replace("{cellstructure}", strCellStructure.TrimEnd().TrimEnd(','));

                        DecalartionLetterText = DecalartionLetterText.Replace("{sign}", "data:image;base64," + pvModel.signatureCopy + "");

                    }
                }
                else if (!string.IsNullOrWhiteSpace(regNumber) && regNumber.ToLower().StartsWith("inv"))
                {
                    Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_INV_DECLARATION_LETTER);
                    var ratedPower = string.Empty;
                    var maximumAcApparentPower = string.Empty;
                    var maximumActivePower = string.Empty;
                    if (item != null)
                    {
                        DecalartionLetterText = @"<!DOCTYPE html>
                                    <html>
                                    <head>
                                        <meta charset='utf-8'>
                                        <title></title>
                                    </head>
                                    <body>" + item["Rich Text"] + "</body></html>";
                        DecalartionLetterText = DecalartionLetterText.Replace("{currentdate}", DateTime.Now.ToString())
                                                                     .Replace("{modelname}", inverterModel.ModelName);

                        #region Rated Power
                        foreach (var rpItem in inverterModel.RatedPower)
                        {
                            ratedPower += rpItem + " " + Translate.Text("kW") + ", ";
                        }
                        #endregion

                        #region Maximum Ac Apparent Power
                        foreach (var mapItem in inverterModel.MaximumAcApparentPower)
                        {
                            maximumAcApparentPower += mapItem + " " + Translate.Text("kVA") + ", ";
                        }
                        #endregion

                        #region Maximum Active Power
                        foreach (var mpItem in inverterModel.MaximumActivePower)
                        {
                            maximumActivePower += mpItem + " " + Translate.Text("kW") + ", ";
                        }
                        #endregion
                        DecalartionLetterText = DecalartionLetterText.Replace("{usagecategory}", inverterModel.UsageCategory)
                                                                     .Replace("{ratedpower}", ratedPower.TrimEnd().TrimEnd(','))
                                                                     .Replace("{maxapparentpower}", maximumAcApparentPower.TrimEnd().TrimEnd(','))
                                                                     .Replace("{maxactivepower}", maximumActivePower.TrimEnd().TrimEnd(','));

                        DecalartionLetterText = DecalartionLetterText.Replace("{sign}", "data:image;base64," + inverterModel.signatureCopy + "");

                    }
                }
                else if (!string.IsNullOrWhiteSpace(regNumber) && regNumber.ToLower().StartsWith("ip"))
                {
                    Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_IP_DECLARATION_LETTER);
                    if (item != null)
                    {
                        DecalartionLetterText = @"<!DOCTYPE html>
                                    <html>
                                    <head>
                                        <meta charset='utf-8'>
                                        <title></title>
                                    </head>
                                    <body>" + item["Rich Text"] + "</body></html>";
                        DecalartionLetterText = DecalartionLetterText.Replace("{currentdate}", DateTime.Now.ToString())
                                                                     .Replace("{modelname}", interfaceModel.ModelName);
                        DecalartionLetterText = DecalartionLetterText.Replace("{sign}", "data:image;base64," + interfaceModel.signatureCopy + "");
                    }
                }

                declarationLetterBytes = AsposePDFObject.HtmlToPdf(DecalartionLetterText);

            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return declarationLetterBytes;
        }
        public List<Tuple<string, byte[]>> GenerateAuthorizeLetter(string ManufacturerFullName, string manufacturercode)
        {
            Item authorizedTemplate = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_AUTHORIZED_LETTER_TEMPLATE);
            Item factoryTemplate = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_FACTORY_TEMPLATE);
            byte[] authorizedLetterBytes = null;
            List<Tuple<string, byte[]>> authorizedfile = new List<Tuple<string, byte[]>>();

            try
            {
                if (authorizedTemplate != null)
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var manfacturerItem = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower() == manufacturercode.ToLower()).FirstOrDefault();
                        if (manfacturerItem != null)
                        {
                            if (!manfacturerItem.Local_Representative)
                            {
                                authorizedTemplate = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_AUTHORIZED_LETTER_MANUFAC_TEMPLATE);
                            }
                            var factoryItems = context.DRRG_Factory_Details.Where(x => x.Manufacturer_Code.ToLower() == manufacturercode.ToLower()).ToList();
                            var supportDocItems = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == manufacturercode.ToLower() && x.Entity == FileEntity.Manufacturer && x.File_Type.ToLower() == FileType.SupportingDocument.ToLower() && x.Manufacturer_Code.ToLower() == DRRGStandardValues.Registration.ToLower()).ToList();
                            var signatureDoc = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == manufacturercode.ToLower() && x.Entity == FileEntity.Manufacturer && x.File_Type.ToLower() == FileType.SignatureCopy.ToLower()).FirstOrDefault();
                            string authorizedLetterText = @"<!DOCTYPE html>
                                                            <html>
                                                            <head>
                                                                <meta charset='utf-8'>
                                                                <title></title>
                                                            </head>
                                                            <body>" + authorizedTemplate["Rich Text"] + "</body></html>";
                            authorizedLetterText = authorizedLetterText.Replace("{DD/MM/YYYY hh:mm}", DateTime.Now.ToString())
                                                                       .Replace("{Ref No}", ManufacturerFullName + " / " + DateTime.Now.Year + " / " + manfacturerItem.Manufacturer_Code)
                                                                       .Replace("{manfacturercode}", manfacturerItem.Manufacturer_Code)
                                                                       .Replace("{manufacfullname}", manfacturerItem.Manufacturer_Name)
                                                                       .Replace("{MrMs}", manfacturerItem.User_Gender.ToLower() == "male" ? "Mr." : "Ms.")
                                                                       .Replace("{his/her}", manfacturerItem.User_Gender.ToLower() == "male" ? "his" : "her")
                                                                       .Replace("{him/her}", manfacturerItem.User_Gender.ToLower() == "male" ? "him" : "her")
                                                                       .Replace("{FirstName}", manfacturerItem.User_First_Name)
                                                                       .Replace("{LastName}", manfacturerItem.User_Last_Name)
                                                                       .Replace("{fullname}", manfacturerItem.User_First_Name + " " + manfacturerItem.User_Last_Name)
                                                                       .Replace("{country}", manfacturerItem.Manufacturer_Country)
                                                                       .Replace("{phonenumber}", !string.IsNullOrWhiteSpace(manfacturerItem.Corporate_Phone_Number) ? manfacturerItem.Corporate_Phone_Code + "-" + manfacturerItem.Corporate_Phone_Number : "-")
                                                                       .Replace("{brandname}", manfacturerItem.Brand_Name)
                                                                       .Replace("{corporateemail}", manfacturerItem.Corporate_Email)
                                                                       .Replace("{company}", manfacturerItem.Company_Full_Name)
                                                                       .Replace("{website}", !string.IsNullOrWhiteSpace(manfacturerItem.Website) ? manfacturerItem.Website : "-");


                            StringBuilder factoryStrBuilder = new StringBuilder();
                            var facCnt = 1;
                            foreach (var facItem in factoryItems)
                            {
                                var factoryHtml = factoryTemplate["Rich Text"];
                                factoryHtml = factoryHtml.Replace("{factoryname}", facItem.Factory_Name)
                                                               .Replace("{factoryno}", facCnt.ToString())
                                                               .Replace("{address}", facItem.Address)
                                                               .Replace("{country}", facItem.Country);


                                var facFiles = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == facItem.Factory_Code.ToLower()).ToList();
                                if (facFiles != null && facFiles.Count > 0)
                                {
                                    string fileName = string.Empty;
                                    foreach (var facFileItem in facFiles)
                                    {
                                        fileName += "<div>" + Translate.Text(facFileItem.File_Type) + "</div>";
                                    }

                                    if (!string.IsNullOrWhiteSpace(fileName))
                                        factoryHtml = factoryHtml.Replace("{filename}", fileName);
                                    else
                                        factoryHtml = factoryHtml.Replace("{filename}", string.Empty);

                                }
                                else
                                {
                                    factoryHtml = factoryHtml.Replace("{filename}", string.Empty);
                                }

                                factoryStrBuilder.Append(factoryHtml);
                                facCnt++;
                            }

                            authorizedLetterText = authorizedLetterText.Replace("{factorydetails}", factoryStrBuilder.ToString())
                                                                       .Replace("{manfacturercompanyname}", manfacturerItem.Local_Representative ? manfacturerItem.Manufacturer_Name + " and " + manfacturerItem.Company_Full_Name : "")
                                                                       .Replace("{role}", manfacturerItem.Local_Representative ? "Authorized Representative" : "Manfacturer")
                                                                       .Replace("{companyname}", manfacturerItem.Company_Full_Name)
                                                                       .Replace("{gender}", System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(manfacturerItem.User_Gender.ToLower()))
                                                                       .Replace("{designation}", manfacturerItem.User_Designation)
                                                                       .Replace("{email}", manfacturerItem.User_Email_Address)
                                                                       .Replace("{username}", manfacturerItem.User_First_Name + " " + manfacturerItem.User_Last_Name)
                                                                       .Replace("{nationality}", manfacturerItem.User_Nationality)
                                                                       .Replace("{fax}", manfacturerItem.Corporate_Fax_Code + "-" + manfacturerItem.Corporate_Fax_Number)
                                                                       .Replace("{mobile}", manfacturerItem.User_Mobile_Code + "-" + manfacturerItem.User_Mobile_Number);
                            string supportDocs = string.Empty;
                            var sdcnt = 1;
                            foreach (var docItem in supportDocItems)
                            {
                                if (!string.IsNullOrWhiteSpace(supportDocs))
                                    supportDocs = Translate.Text(docItem.File_Type) + sdcnt + ": " + Translate.Text(docItem.Name);
                                else
                                    supportDocs += "</br>" + Translate.Text(docItem.File_Type) + sdcnt + ": " + Translate.Text(docItem.Name);

                                sdcnt++;
                            }
                            if (signatureDoc != null)
                            {
                                authorizedLetterText = authorizedLetterText.Replace("{sign}", "<img alt='' style='max-width: 200px;' id='sign' src='data:image;base64," + Convert.ToBase64String(signatureDoc.Content) + "'>");
                            }

                            authorizedLetterText = authorizedLetterText.Replace("{supportingdocs}", supportDocs);
                            authorizedLetterBytes = AsposePDFObject.HtmlToPdf(authorizedLetterText);

                            authorizedfile.Add(new Tuple<string, byte[]>("AuthorizationLetter_" + manufacturercode + ".pdf", authorizedLetterBytes));
                        }

                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return authorizedfile;
        }
        public void SendDRRGRegistrationEmail(string name, string tomail, bool rep, string linkurl, string module, string ManufacturerFullName, string manufacturercode)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                var file_photo = rep ? new Sitecorex.Data.Fields.FileField(item.Fields["Registration file Local Rep"]) : new Sitecorex.Data.Fields.FileField(item.Fields["Registration file"]);
                Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.DRRG_SET_PASSWORD));
                string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + HttpUtility.UrlEncode(linkurl);
                UniqueURL = UniqueURL.Replace(":443", "");

                string subject = string.Empty;
                List<Tuple<string, byte[]>> authorizedfile = null;
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.Registration))
                    {
                        authorizedfile = GenerateAuthorizeLetter(ManufacturerFullName, manufacturercode);
                        body = item["Registration Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{setpasswordlink}", UniqueURL);
                        body = body.Replace("{username}", toEmail);
                        subject = item["Registration Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.UpdateProfile))
                    {
                        authorizedfile = GenerateAuthorizeLetter(ManufacturerFullName, manufacturercode);
                        body = item["Edit Profile Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{setpasswordlink}", UniqueURL);
                        body = body.Replace("{manufacturername}", ManufacturerFullName);
                        body = body.Replace("{username}", toEmail);
                        subject = item["Edit Profile Subject"].ToString();
                    }

                    if (file_photo != null && file_photo.MediaItem != null && !string.IsNullOrWhiteSpace(file_photo.Src))
                    {
                        string url = Sitecorex.StringUtil.EnsurePrefix('/', Sitecorex.Resources.Media.MediaManager.GetMediaUrl(file_photo.MediaItem));
                        url = Sitecorex.Links.LinkManager.GetItemUrl(Sitecorex.Context.Database.GetItem(Sitecorex.Context.Site.StartPath), new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Never }) + url;
                        body = body.Replace("{authorizedletterlink}", url);
                    }
                    else
                    {
                        body = body.Replace("{authorizedletterlink}", "#");
                    }

                    if (authorizedfile != null && authorizedfile.Count > 0)
                        EmailServiceClient.SendEmail(fromEmail, toEmail, string.Empty, string.Empty, subject, body, authorizedfile);
                    else
                        EmailServiceClient.SendEmail(fromEmail, toEmail, subject, body);

                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
        }
        public void SendDRRGForgotpasswordEmail(string name, string tomail, string linkurl)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.DRRG_SET_PASSWORD));
                string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always }) + "?code=" + HttpUtility.UrlEncode(linkurl);
                UniqueURL = UniqueURL.Replace(":443", "");
                
                if (item != null)
                {
                    body = item["Forgot Password Email"];
                    body = body.Replace("{user}", name);
                    body = body.Replace("{setpasswordlink}", UniqueURL);
                    EmailServiceClient.SendEmail(fromEmail, toEmail, item["Forgot Password Subject"].ToString(), body);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

        }
        public void SendDRRGSetpasswordEmail(string name, string tomail, string module)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.DRRG_HOME));
                string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always });
                UniqueURL = UniqueURL.Replace(":443", "");
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.Registration))
                    {
                        body = item["Set Password Registration Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{loginURL}", UniqueURL);
                        EmailServiceClient.SendEmail(fromEmail, toEmail, item["Set Password Registration Subject"].ToString(), body);
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.ForgotPassword))
                    {
                        body = item["Set Password Forgot Email"];
                        body = body.Replace("{user}", name);
                        EmailServiceClient.SendEmail(fromEmail, toEmail, item["Set Password Forgot Subject"].ToString(), body);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

        }
        public void SendDRRGModuleEmail(string name, string tomail, string referenceid, string module)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                string subject = string.Empty;
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.PVmodule))
                    {
                        body = item["PV Module Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        subject = item["PV Module Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.Interfacemodule))
                    {
                        body = item["Interface Module Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        subject = item["Interface Module Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.Invertermodule))
                    {
                        body = item["Inverter Module Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        subject = item["Inverter Module Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.AuthorizedLetterSubmitted))
                    {
                        body = item["Authorization Letter Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{Refno}", referenceid);
                        subject = item["Authorization Letter Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.EvaluatorRegistration))
                    {
                        body = item["Evaluator Registration Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        subject = item["Evaluator Registration Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.Registrationapproved))
                    {
                        Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.DRRG_HOME));
                        string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = false, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always });
                        UniqueURL = UniqueURL.Replace(":443", "");
                        UniqueURL = DRRG_PORTAL_WEBURL + UniqueURL;
                        body = item["Manufacturer Approval Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        body = body.Replace("{loginurl}", UniqueURL);
                        subject = item["Manufacturer Approval Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.EvaluatorRegistrationApproved))
                    {
                        if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                        {
                            Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.DRRG__EVALUATOR_Dashboard));
                            string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always });
                            UniqueURL = UniqueURL.Replace(":443", "");
                            body = item["New Evaluator Registration Approve Email"];
                            body = body.Replace("{user}", name);
                            body = body.Replace("{referenceid}", referenceid);
                            body = body.Replace("{emailid}", tomail);
                            body = body.Replace("{loginurl}", UniqueURL);
                            subject = item["New Evaluator Registration Approve Subject"].ToString();
                        }
                    }
                    EmailServiceClient.SendEmail(fromEmail, toEmail, subject, body);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        public void SendDRRGRemoveModuleEmail(string name, string tomail, string moduleName, string module)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                string subject = string.Empty;
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.PVmodule))
                    {
                        body = item["PV Module Remove Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{PVModule}", moduleName);
                        subject = item["PV Module Remove Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.Interfacemodule))
                    {
                        body = item["Interface Module Remove Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{InterfaceModule}", moduleName);
                        subject = item["Interface Module Remove Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.Invertermodule))
                    {
                        body = item["Inverter Module Remove Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{InverterModule}", moduleName);
                        subject = item["Inverter Module Remove Subject"].ToString();
                    }
                    EmailServiceClient.SendEmail(fromEmail, toEmail, subject, body);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        public void SendDRRGUpdateModuleEmail(string name, string tomail, string moduleName, string module)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                string subject = string.Empty;
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.PVmodule))
                    {
                        body = item["PV Module Update Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{PVModule}", moduleName);
                        subject = item["PV Module Update Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.Interfacemodule))
                    {
                        body = item["Interface Module Update Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{InterfaceModule}", moduleName);
                        subject = item["Interface Module Update Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(FileEntity.Invertermodule))
                    {
                        body = item["Inverter Module Update Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{InverterModule}", moduleName);
                        subject = item["Inverter Module Update Subject"].ToString();
                    }
                    EmailServiceClient.SendEmail(fromEmail, toEmail, subject, body);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }


        }

        public void SendDRRGRejectionEmail(string name, string tomail, string referenceid, string rejectedreason, string modelname, string module, List<Tuple<string, byte[]>> file)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                string subject = string.Empty;
                if (item != null)
                {
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.ManufacturerRejected))
                    {
                        Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.DRRG_HOME));
                        string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = false, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always });
                        UniqueURL = UniqueURL.Replace(":443", "");
                        UniqueURL = DRRG_PORTAL_WEBURL + UniqueURL;
                        body = item["Manufacturer Reject Email"];
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        body = body.Replace("{rejectedreason}", rejectedreason);
                        body = body.Replace("{loginurl}", UniqueURL);
                        subject = item["Manufacturer Reject Subject"].ToString();
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.PVModuleRejected))
                    {
                        if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                        {
                            body = item["PV Module Reject Email"];
                            subject = item["PV Module Reject Subject"].ToString();
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                        {
                            body = item["PV Module Manager Reject Email"];
                            subject = item["PV Module Manager Reject Subject"].ToString();
                        }
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        body = body.Replace("{rejectedreason}", rejectedreason);
                        body = body.Replace("{modelname}", modelname);
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.IVModuleRejected))
                    {
                        if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                        {
                            body = item["IV Module Reject Email"];
                            subject = item["IV Module Reject Subject"].ToString();
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                        {
                            body = item["IV Module Manager Reject Email"];
                            subject = item["IV Module Manager Reject Subject"].ToString();
                        }
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        body = body.Replace("{rejectedreason}", rejectedreason);
                        body = body.Replace("{modelname}", modelname);
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.IPModuleRejected))
                    {
                        if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                        {
                            body = item["IP Module Reject Email"];
                            subject = item["IP Module Reject Subject"].ToString();
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                        {
                            body = item["IP Module Manager Reject Email"];
                            subject = item["IP Module Manager Reject Subject"].ToString();
                        }
                        body = body.Replace("{user}", name);
                        body = body.Replace("{referenceid}", referenceid);
                        body = body.Replace("{rejectedreason}", rejectedreason);
                        body = body.Replace("{modelname}", modelname);
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.EvaluatorRegistrationRejected))
                    {
                        if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                        {
                            body = item["New Evaluator Registration Reject Email"];
                            subject = item["New Evaluator Registration Reject Subject"].ToString();
                            body = body.Replace("{user}", name);
                            body = body.Replace("{referenceid}", referenceid);
                            body = body.Replace("{rejectedreason}", rejectedreason);
                        }
                    }

                    if (file != null && file.Count > 0)
                    {
                        EmailServiceClient.SendEmail(fromEmail, toEmail, string.Empty, string.Empty, subject, body, file);
                    }
                    else
                    {
                        body = body.Replace("<p><strong>Screenshot:</strong> please find attached</p>", "");
                        EmailServiceClient.SendEmail(fromEmail, toEmail, subject, body);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
        }
        public void SendDRRGApproveEmail(string name, string tomail, string referenceid, string modelname, string module)
        {
            try
            {
                Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                string fromEmail = item["From Mail"].ToString();
                string toEmail = tomail;
                string body = string.Empty;
                string subject = string.Empty;
                if (item != null)
                {
                    Item pageItem = Sitecorex.Context.Database.GetItem(Sitecorex.Data.ID.Parse(SitecoreItemIdentifiers.Eligible_List_APPLICATION));
                    string UniqueURL = Sitecorex.Links.LinkManager.GetItemUrl(pageItem, new Sitecorex.Links.UrlBuilders.ItemUrlBuilderOptions() { AlwaysIncludeServerUrl = true, LanguageEmbedding = Sitecorex.Links.LanguageEmbedding.Always });
                    UniqueURL = UniqueURL.Replace(":443", "");
                    UniqueURL = DRRG_PORTAL_WEBURL + UniqueURL;
                    body = item["Schema Manager Registration Approve Email"];
                    subject = item["Schema Manager Registration Approve Subject"].ToString();
                    body = body.Replace("{user}", name);
                    body = body.Replace("{referenceid}", referenceid);
                    body = body.Replace("{listURL}", UniqueURL);
                    body = body.Replace("{modelname}", modelname);
                    if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.PVModuleApproved))
                    {
                        body = body.Replace("{moduletype}", Translate.Text("PV Module"));
                        subject = subject.Replace("{moduletype}", Translate.Text("PV Module"));
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.IPModuleApproved))
                    {
                        body = body.Replace("{moduletype}", Translate.Text("DRRG_InterfaceProtection"));
                        subject = subject.Replace("{moduletype}", Translate.Text("DRRG_InterfaceProtection"));
                    }
                    else if (!string.IsNullOrWhiteSpace(module) && module.Equals(DRRGStandardValues.IVModuleApproved))
                    {
                        body = body.Replace("{moduletype}", Translate.Text("PV Inverter"));
                        subject = subject.Replace("{moduletype}", Translate.Text("PV Inverter"));
                    }
                    EmailServiceClient.SendEmail(fromEmail, toEmail, subject, body);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
        }
        protected bool GetDecryptedValues(string url, out string userid, out string errormessage, out string module)
        {
            bool valid = false;
            userid = string.Empty;
            module = string.Empty;
            errormessage = Translate.Text("Invalid Link"); ;
            try
            {
                byte[] Results;
                UTF8Encoding UTF8 = new UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.Zeros
                };
                byte[] DataToDecrypt = Convert.FromBase64String(url);
                try
                {
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                }
                string resulttext = UTF8.GetString(Results);
                string[] result = resulttext.Split('|');
                if (result.Length > 1 && !string.IsNullOrWhiteSpace(result[1]))
                {
                    if (DateTime.TryParse(result[1], out DateTime parsetime))
                    {
                        if (DateTime.Now.CompareTo(parsetime.AddDays(1)) < 0)
                        {
                            valid = true;
                            userid = result[0];
                            module = !string.IsNullOrWhiteSpace(result[2]) ? result[2].Replace("\0", string.Empty) : string.Empty;
                        }
                        else
                        {
                            errormessage = Translate.Text("Link has been expired");
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return valid;
        }

        protected string GetEncryptedLinkExpiryURL(string userid, string module = "")
        {
            byte[] Results;
            UTF8Encoding UTF8 = new UTF8Encoding();
            MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
            byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(passphrase));
            TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
            {
                Key = TDESKey,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.Zeros
            };
            byte[] DataToEncrypt = UTF8.GetBytes(string.Concat(userid, "|", DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"), "|", module));
            try
            {
                ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
            }
            finally
            {
                TDESAlgorithm.Clear();
                HashProvider.Clear();
            }
            string newlinkurl = Convert.ToBase64String(Results);
            return newlinkurl;
        }

        public ModuleDetail GetPVModuleDetail(string id, List<DRRG_PVMODULE> pvresult, List<DRRG_Files> pvfileslist = null, List<SP_DRRG_GETFilesbyIDAdmin_Result> pvresultfiles = null)
        {
            ModuleDetail moduleDetail = new ModuleDetail { ReferenceNumber = id };
            try
            {
                if (pvresult != null && pvresult.FirstOrDefault() != null)
                {
                    var pvitem = pvresult.FirstOrDefault();
                    moduleDetail.Status = pvitem.Status;

                    var nominalpower = string.Empty;
                    DRRG_Manufacturer_Details manufacDetail = null;
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        manufacDetail = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower() == pvitem.Manufacturer_Code.ToLower()).FirstOrDefault();
                        var nominalPowerLst = context.DRRG_PVModule_Nominal.Where(x => x.PV_ID.ToLower() == id.ToLower()).ToList();
                        if (!string.IsNullOrWhiteSpace(pvitem.Nominal_Power) && pvitem.Nominal_Power.Equals("Nominal Power Range"))
                        {
                            if (nominalPowerLst != null)
                            {
                                nominalpower = nominalPowerLst[0].wp1 + " Wp - " + nominalPowerLst[0].wp2 + " Wp , in step of " + nominalPowerLst[0].wp3 + " W";
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(pvitem.Nominal_Power) && pvitem.Nominal_Power.Equals("Multiple Nominal Power Entries"))
                        {
                            foreach (var item in nominalPowerLst)
                            {
                                if (item != null)
                                {
                                    nominalpower += item.wp1?.ToString() + " Wp, ";
                                }
                            }
                            if (!string.IsNullOrWhiteSpace(nominalpower))
                                nominalpower = nominalpower.Trim().TrimEnd(',');
                        }
                        else
                        {
                            if (nominalPowerLst != null)
                                nominalpower = nominalPowerLst[0].wp1 + " Wp";
                        }
                    }
                    ModuleSectionRow moduleSectionRow = new ModuleSectionRow { Title = Translate.Text("DRRG_EquipmentDetails") };
                    ModuleSectionRow moduleSectionRow3 = new ModuleSectionRow { Title = Translate.Text("Manufacturer Details") };

                    ModuleSection moduleSection = new ModuleSection();
                    ModuleSection moduleSection1 = new ModuleSection();
                    ModuleSection moduleSection2 = new ModuleSection();
                    ModuleSection moduleSection3 = new ModuleSection();

                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Full Legal Name of Manufacturer"), manufacDetail.Manufacturer_Name));
                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Country (As per Certificate Holder’s Address in Compliance Certificate)"), manufacDetail.Manufacturer_Country));
                    if (!string.IsNullOrWhiteSpace(manufacDetail.Corporate_Phone_Number))
                        moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Phone Number"), manufacDetail.Corporate_Phone_Code + manufacDetail.Corporate_Phone_Number));


                    var detailURL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_DETAILS);
                    if (CurrentPrincipal.Role.Equals("drrgevaluator") || CurrentPrincipal.Role.Equals("drrgschemamanager"))
                    {
                        detailURL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG__EVALUATOR_Details);
                    }
                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem("", "<a href='" + detailURL + "?id=" + manufacDetail.Manufacturer_Code + "&strstatus=approved' style='display: inline-block;' target='_blank' class='link tooltipstered'>" + Translate.Text("DRRG_Showmore") + "</a>"));
                    moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Brand/Trade Name of Manufacturer (If Any)"), manufacDetail.Brand_Name));
                    moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate E-mail Address"), manufacDetail.Corporate_Email));
                    //moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Fax Number"), manufacDetail.Corporate_Fax_Code + manufacDetail.Corporate_Fax_Number));
                    if (!string.IsNullOrWhiteSpace(manufacDetail.Website))
                        moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Website"), manufacDetail.Website, linkvalue: false));



                    moduleSectionRow3.moduleSections.Add(moduleSection2);
                    moduleSectionRow3.moduleSections.Add(moduleSection3);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow3);

                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Model of PV Module"), pvitem.Model_Name));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Nominal Power Entry Type"), pvitem.Nominal_Power));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Nominal Power"), nominalpower));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Module Length"), pvitem.Module_Length.ToString()));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Module Width"), pvitem.Module_Width.ToString()));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Cell Technology"), pvitem.Cell_Technology?.ToString()));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Framed"), pvitem.Framed?.ToString()));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Position of the Junction Box"), pvitem.Position_JB?.ToString()));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Terminations"), pvitem.Terminations?.ToString()));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Features of the Junction Box"), pvitem.Features_JB?.ToString()));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Front Superstrate"), pvitem.Front_Superstrate?.ToString()));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Back Superstrate"), pvitem.Back_Superstrate?.ToString()));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("PV Cell Structure"), pvitem.PV_Cell_Structure?.ToString()));

                    if (!string.IsNullOrWhiteSpace(pvitem.Othe_Front_Superstrate))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Front Superstrate"), pvitem.Othe_Front_Superstrate?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Other_Back_Superstrate))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Back Superstrate"), pvitem.Other_Back_Superstrate?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Salt_Mist_Test_Method))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Test Method"), pvitem.Salt_Mist_Test_Method?.ToString()));


                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Encapsulant"), pvitem.Encapsulant?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Other_Cell_Structure))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other PV Cell Structure"), pvitem.Other_Cell_Structure?.ToString()));

                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("DC System Grounding if mandatory?"), pvitem.DC_System_Grounding_Mandatory?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.DC_System_Grounding_Mandatory) && pvitem.DC_System_Grounding_Mandatory.ToLower().Equals("yes"))
                        moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("DC System Grounding"), pvitem.DC_System_Grounding?.ToString()));

                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Material of the Junction Box Enclosure"), pvitem.Material_JB?.ToString()));

                    if (!string.IsNullOrWhiteSpace(pvitem.Other_Material_JB))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Material of the Junction Box Enclosure"), pvitem.Other_Material_JB?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Other_Terminations))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Terminations"), pvitem.Other_Terminations?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Other_Features_JB))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Features of the Junction Box"), pvitem.Other_Features_JB?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Other_Encapsulant))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Encapsulant"), pvitem.Other_Encapsulant?.ToString()));
                    if (!string.IsNullOrWhiteSpace(pvitem.Othe_Front_Superstrate))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Other Front Superstrate"), pvitem.Othe_Front_Superstrate?.ToString()));

                    moduleSectionRow.moduleSections.Add(moduleSection);
                    moduleSectionRow.moduleSections.Add(moduleSection1);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow);

                    if (pvfileslist != null && pvfileslist.Count() > 0)
                    {
                        ExtractFilelist(pvfileslist, moduleDetail);
                    }
                    if (pvresultfiles != null && pvresultfiles.Count() > 0)
                    {
                        ExtractFileAdminlist(pvresultfiles, moduleDetail);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return moduleDetail;
        }

        private static void ExtractFilelist(List<DRRG_Files> pvfileslist, ModuleDetail moduleDetail)
        {
            ModuleSectionRow moduleSectionRow2 = new ModuleSectionRow { Title = Translate.Text("Compliance Certificates") };
            ModuleSection moduleSectionfilelist = new ModuleSection();
            ModuleSection moduleSectionfilelist1 = new ModuleSection();
            pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Take(pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Count() % 2 == 0 ? pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Count() / 2 : (pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Count() / 2) + 1).ForEach(x =>
            moduleSectionfilelist.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true))
            );
            pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Skip(moduleSectionfilelist.moduleItems.Count).ForEach(x => moduleSectionfilelist1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
            moduleSectionRow2.moduleSections.Add(moduleSectionfilelist);
            moduleSectionRow2.moduleSections.Add(moduleSectionfilelist1);
            moduleDetail.moduleSectionRows.Add(moduleSectionRow2);


            ModuleSectionRow moduleSectionRow3 = new ModuleSectionRow { Title = Translate.Text("Declaration") };
            ModuleSection moduleSectionfilelist2 = new ModuleSection();
            ModuleSection moduleSectionfilelist3 = new ModuleSection();
            pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Take(pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Count() % 2 == 0 ? pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Count() / 2 : (pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Count() / 2) + 1).ForEach(x =>
              moduleSectionfilelist2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true))
            );
            pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Skip(moduleSectionfilelist2.moduleItems.Count).ForEach(x => moduleSectionfilelist3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
            moduleSectionRow3.moduleSections.Add(moduleSectionfilelist2);
            moduleSectionRow3.moduleSections.Add(moduleSectionfilelist3);
            moduleDetail.moduleSectionRows.Add(moduleSectionRow3);
        }
        private static void ExtractFileAdminlist(List<SP_DRRG_GETFilesbyIDAdmin_Result> pvfileslist, ModuleDetail moduleDetail)
        {
            ModuleSectionRow moduleSectionRow2 = new ModuleSectionRow { Title = Translate.Text("Compliance Certificates") };
            ModuleSection moduleSectionfilelist = new ModuleSection();
            ModuleSection moduleSectionfilelist1 = new ModuleSection();
            pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Take(pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Count() % 2 == 0 ? pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Count() / 2 : (pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Count() / 2) + 1).ForEach(x =>
            moduleSectionfilelist.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true))
            );
            pvfileslist.Where(x => x.File_Type != FileType.SignatureCopy).Skip(moduleSectionfilelist.moduleItems.Count).ForEach(x => moduleSectionfilelist1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
            moduleSectionRow2.moduleSections.Add(moduleSectionfilelist);
            moduleSectionRow2.moduleSections.Add(moduleSectionfilelist1);
            moduleDetail.moduleSectionRows.Add(moduleSectionRow2);


            ModuleSectionRow moduleSectionRow3 = new ModuleSectionRow { Title = Translate.Text("Declaration") };
            ModuleSection moduleSectionfilelist2 = new ModuleSection();
            ModuleSection moduleSectionfilelist3 = new ModuleSection();
            pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Take(pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Count() % 2 == 0 ? pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Count() / 2 : (pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Count() / 2) + 1).ForEach(x =>
              moduleSectionfilelist2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true))
            );
            pvfileslist.Where(x => x.File_Type == FileType.SignatureCopy).Skip(moduleSectionfilelist2.moduleItems.Count).ForEach(x => moduleSectionfilelist3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
            moduleSectionRow3.moduleSections.Add(moduleSectionfilelist2);
            moduleSectionRow3.moduleSections.Add(moduleSectionfilelist3);
            moduleDetail.moduleSectionRows.Add(moduleSectionRow3);
        }

        public ModuleDetail GetInterfaceModuleDetail(string id, List<DRRG_InterfaceModule> ipresult, List<DRRG_Files> pvfileslist = null, List<SP_DRRG_GETFilesbyIDAdmin_Result> pvresultfiles = null)
        {
            ModuleDetail moduleDetail = new ModuleDetail { ReferenceNumber = id };
            try
            {
                if (ipresult != null && ipresult.FirstOrDefault() != null)
                {
                    var pvitem = ipresult.FirstOrDefault();
                    moduleDetail.Status = pvitem.Status;
                    DRRG_Manufacturer_Details manufacDetail = null;
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        manufacDetail = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower() == pvitem.Manufacturer_Code.ToLower()).FirstOrDefault();
                    }

                    ModuleSectionRow moduleSectionRow = new ModuleSectionRow { Title = Translate.Text("DRRG_EquipmentDetails") };
                    ModuleSectionRow moduleSectionRow1 = new ModuleSectionRow { Title = Translate.Text("Manufacturer Details") };

                    ModuleSection moduleSection2 = new ModuleSection();
                    ModuleSection moduleSection3 = new ModuleSection();

                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Full Legal Name of Manufacturer"), manufacDetail.Manufacturer_Name));
                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Country (As per Certificate Holder’s Address in Compliance Certificate)"), manufacDetail.Manufacturer_Country));
                    if (!string.IsNullOrWhiteSpace(manufacDetail.Corporate_Phone_Number))
                        moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Phone Number"), manufacDetail.Corporate_Phone_Code + manufacDetail.Corporate_Phone_Number));

                    var detailURL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_DETAILS);
                    if (CurrentPrincipal.Role.Equals("drrgevaluator") || CurrentPrincipal.Role.Equals("drrgschemamanager"))
                    {
                        detailURL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG__EVALUATOR_Details);
                    }
                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem("", "<a href='" + detailURL + "?id=" + manufacDetail.Manufacturer_Code + "&strstatus=approved' style='display: inline-block;' target='_blank' class='link tooltipstered'>" + Translate.Text("DRRG_Showmore") + "</a>"));
                    moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Brand/Trade Name of Manufacturer (If Any)"), manufacDetail.Brand_Name));
                    moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate E-mail Address"), manufacDetail.Corporate_Email));

                    //moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Fax Number"), manufacDetail.Corporate_Fax_Code + manufacDetail.Corporate_Fax_Number));
                    if (!string.IsNullOrWhiteSpace(manufacDetail.Website))
                        moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Website"), manufacDetail.Website, linkvalue: false));
                    moduleSectionRow1.moduleSections.Add(moduleSection2);
                    moduleSectionRow1.moduleSections.Add(moduleSection3);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow1);

                    ModuleSection moduleSection = new ModuleSection();
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Model of Interface protection"), pvitem.Model_Name));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Application"), pvitem.Application));
                    ModuleSection moduleSection1 = new ModuleSection();
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Communication Protocol"), pvitem.CommunicationProtocol));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Complaiance to Other National"), pvitem.Compliance));
                    moduleSectionRow.moduleSections.Add(moduleSection);
                    moduleSectionRow.moduleSections.Add(moduleSection1);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow);
                    if (pvfileslist != null && pvfileslist.Count() > 0)
                    {
                        ExtractFilelist(pvfileslist, moduleDetail);
                    }
                    if (pvresultfiles != null && pvresultfiles.Count() > 0)
                    {
                        ExtractFileAdminlist(pvresultfiles, moduleDetail);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return moduleDetail;
        }

        public ModuleDetail GetInverterModuleDetail(string id, List<DRRG_InverterModule> ivresult, List<DRRG_Files> pvfileslist = null, List<SP_DRRG_GETFilesbyIDAdmin_Result> pvresultfiles = null)
        {
            ModuleDetail moduleDetail = new ModuleDetail { ReferenceNumber = id };
            try
            {
                if (ivresult != null && ivresult.FirstOrDefault() != null)
                {
                    DRRG_Manufacturer_Details manufacDetail = null;
                    var pvitem = ivresult.FirstOrDefault();
                    moduleDetail.Status = pvitem.Status;
                    ModuleSectionRow moduleSectionRow = new ModuleSectionRow { Title = Translate.Text("DRRG_EquipmentDetails") };
                    ModuleSectionRow moduleSectionRow3 = new ModuleSectionRow { Title = Translate.Text("Manufacturer Details") };

                    ModuleSection moduleSection = new ModuleSection();
                    ModuleSection moduleSection1 = new ModuleSection();
                    var ACParentPower = string.Empty;
                    var RatedPower = string.Empty;
                    var MaxPower = string.Empty;

                    using (DRRGEntities context = new DRRGEntities())
                    {
                        manufacDetail = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code.ToLower() == pvitem.Manufacturer_Code.ToLower()).FirstOrDefault();
                        var resultAP = context.DRRG_InverterModule_AP.Where(x => x.Inverter_ID.ToLower() == id.ToLower()).ToList();

                        foreach (var item in resultAP)
                        {
                            if (!string.IsNullOrWhiteSpace(ACParentPower))
                                ACParentPower = ACParentPower + ", " + item.AC_Apparent_Power.ToString() + " kVA";
                            else
                                ACParentPower = item.AC_Apparent_Power.ToString() + " kVA";
                        }

                        var resultRP = context.DRRG_InverterModule_RP.Where(x => x.Inverter_ID.ToLower() == id.ToLower()).ToList();

                        foreach (var item in resultRP)
                        {
                            if (!string.IsNullOrWhiteSpace(RatedPower))
                                RatedPower = RatedPower + ", " + item.Rated_Power.ToString() + " kW";
                            else
                                RatedPower = item.Rated_Power.ToString() + " kW";
                        }
                        var resultMAP = context.DRRG_InverterModule_MAP.Where(x => x.Inverter_ID.ToLower() == id.ToLower()).ToList();

                        foreach (var item in resultMAP)
                        {
                            if (!string.IsNullOrWhiteSpace(MaxPower))
                                MaxPower = MaxPower + ", " + item.Max_Active_Power.ToString() + " kW";
                            else
                                MaxPower = item.Max_Active_Power.ToString() + " kW";
                        }
                    }
                    ModuleSection moduleSection2 = new ModuleSection();
                    ModuleSection moduleSection3 = new ModuleSection();

                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Full Legal Name of Manufacturer"), manufacDetail.Manufacturer_Name));
                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Country (As per Certificate Holder’s Address in Compliance Certificate)"), manufacDetail.Manufacturer_Country));
                    if (!string.IsNullOrWhiteSpace(manufacDetail.Corporate_Phone_Number))
                        moduleSection2.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Phone Number"), manufacDetail.Corporate_Phone_Code + manufacDetail.Corporate_Phone_Number));

                    var detailURL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG_DETAILS);
                    if (CurrentPrincipal.Role.Equals("drrgevaluator") || CurrentPrincipal.Role.Equals("drrgschemamanager"))
                    {
                        detailURL = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.DRRG__EVALUATOR_Details);
                    }
                    moduleSection2.moduleItems.Add(new ModuleKeyValueItem("", "<a href='" + detailURL + "?id=" + manufacDetail.Manufacturer_Code + "&strstatus=approved' style='display: inline-block;' target='_blank' class='link tooltipstered'>" + Translate.Text("DRRG_Showmore") + "</a>"));

                    moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Brand/Trade Name of Manufacturer (If Any)"), manufacDetail.Brand_Name));
                    //moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Fax Number"), manufacDetail.Corporate_Fax_Code + manufacDetail.Corporate_Fax_Number));
                    moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate E-mail Address"), manufacDetail.Corporate_Email));
                    if (!string.IsNullOrWhiteSpace(manufacDetail.Website))
                        moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Website"), manufacDetail.Website, linkvalue: false));
                    moduleSectionRow3.moduleSections.Add(moduleSection2);
                    moduleSectionRow3.moduleSections.Add(moduleSection3);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow3);

                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Model of PV Inverter"), pvitem.Model_Name));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Rate Power"), RatedPower));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Power Factor Range (Lag/lead)"), pvitem.Power_Factor_Range));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Remote Control"), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pvitem.Remote_Control)));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("LVRT"), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pvitem.LVRT)));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("DRRG.DegreeProtectionIP"), pvitem.Protection_Degree.ToString()));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Internal Interface Protection"), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pvitem.Internal_Interface)));
                    //moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("multimasterfeature"), pvitem.Multi_Master_Feature));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Power derating at 50 oC and above"), pvitem.Power_Derating.ToString()));


                    //moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Function of String"), pvitem.Function_String));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Max apparent power (kVA)"), ACParentPower));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Number of Phases"), pvitem.Number_Of_Phases));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Remote Monitoring"), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pvitem.Remote_Monitoring)));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Max active power (kW @ cosθ = 1)"), MaxPower));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Possibility of Earthing DC Conductors"), pvitem.Possibility_DC_Conductors));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Total no of string-parallel"), pvitem.Number_String));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Multi MPPT Section"), System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(pvitem.MPPT_Section)));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Number of Section"), pvitem.Number_Section));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("DC/AC galvanic isolation"), pvitem.AC_DC_Section));

                    moduleSectionRow.moduleSections.Add(moduleSection);
                    moduleSectionRow.moduleSections.Add(moduleSection1);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow);

                    if (pvfileslist != null && pvfileslist.Count() > 0)
                    {
                        ExtractFilelist(pvfileslist, moduleDetail);
                    }
                    if (pvresultfiles != null && pvresultfiles.Count() > 0)
                    {
                        ExtractFileAdminlist(pvresultfiles, moduleDetail);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return moduleDetail;
        }

        public ModuleDetail GetManufacturerDetail(string id, List<DRRG_Manufacturer_Details> manuresult, List<SP_DRRG_GETFactorybyID_Result> factresult, List<SP_DRRG_GETFilesbyIDAdmin_Result> fileslist, List<SP_DRRG_GETManuRejectedComments_Result> rejectcomments, List<SP_DRRG_GETManuRejectedFilesbyIDAdmin_Result> rejectfiles)
        {
            ModuleDetail moduleDetail = new ModuleDetail { ReferenceNumber = id };
            try
            {
                if (manuresult != null && manuresult.FirstOrDefault() != null)
                {
                    var item = manuresult.FirstOrDefault();
                    moduleDetail.Status = item.Status;
                    ModuleSectionRow moduleSectionRow = new ModuleSectionRow { Title = Translate.Text("Manufacturer Details") };
                    ModuleSection moduleSection = new ModuleSection();
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Full Legal Name of Manufacturer"), item.Manufacturer_Name));
                    moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Country (As per Certificate Holder’s Address in Compliance Certificate)"), item.Manufacturer_Country));
                    if (!string.IsNullOrWhiteSpace(item.Corporate_Phone_Number))
                        moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Phone Number"), item.Corporate_Phone_Code + item.Corporate_Phone_Number));
                    if (fileslist != null && fileslist.Count > 0 && fileslist.Any(x => x.Reference_ID.Equals(item.Manufacturer_Code)))
                    {
                        fileslist.Where(x => x.Reference_ID.Equals(item.Manufacturer_Code) && x.File_Type == FileType.TrademarkLogo).OrderBy(x => x.File_Type).ForEach(x => moduleSection.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
                    }
                    ModuleSection moduleSection1 = new ModuleSection();
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Brand/Trade Name of Manufacturer (If Any)"), item.Brand_Name));
                    moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate E-mail Address"), item.Corporate_Email));

                    //moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate Fax Number"), item.Corporate_Fax_Code + item.Corporate_Fax_Number));
                    if (!string.IsNullOrWhiteSpace(item.Website))
                        moduleSection1.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Website"), item.Website, linkvalue: false));
                    moduleSectionRow.moduleSections.Add(moduleSection);
                    moduleSectionRow.moduleSections.Add(moduleSection1);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow);

                    if (factresult != null && factresult.Count() > 0)
                    {
                        ModuleSectionRow moduleSectionRow2 = new ModuleSectionRow { Title = Translate.Text("Factory Details") };
                        var i = 1;
                        foreach (var factory in factresult.OrderBy(x => x.CreatedDate))
                        {
                            ModuleSection moduleSection3 = new ModuleSection();
                            //
                            moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Full Legal Name of Factory"), factory.Factory_Name));
                            moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Country (As per Certificate Holder’s Address in Compliance Certificate)"), factory.Country));
                            moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Address"), factory.Address));
                            moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("End-of-Life (EOL) PV Module Management (If Any)"), factory.EOL_PV_Module));
                            using (DRRGEntities context = new DRRGEntities())
                            {
                                var facFiles = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == factory.Factory_Code.ToLower()).ToList();
                                if (facFiles != null && facFiles.Count > 0)
                                {
                                    facFiles.ForEach(x => moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
                                }
                            }
                            moduleSectionRow2.moduleSections.Add(moduleSection3);
                            i++;
                        }
                        moduleDetail.moduleSectionRows.Add(moduleSectionRow2);
                    }

                    ModuleSectionRow moduleSectionRow3 = new ModuleSectionRow { Title = Translate.Text("User Details") };
                    ModuleSection moduleSection4 = new ModuleSection();
                    if (item.Local_Representative)
                        moduleSection4.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Full Legal Name of Company"), item.Company_Full_Name));
                    moduleSection4.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("First Name"), item.User_First_Name));
                    moduleSection4.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Last Name"), item.User_Last_Name));
                    moduleSection4.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Gender"), item.User_Gender));
                    moduleSection4.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Nationality"), item.User_Nationality));

                    ModuleSection moduleSection5 = new ModuleSection();
                    moduleSection5.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("DRRG.ApplicantCompanyRole"), item.Local_Representative ? Translate.Text("DRRG.AuthorizedLocalRepresentative.Label") : Translate.Text("DRRG.Manufacturer.Label")));
                    moduleSection5.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("DRRG.Designation.Label"), item.User_Designation));
                    moduleSection5.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Mobile Number"), item.User_Mobile_Code + item.User_Mobile_Number));
                    moduleSection5.moduleItems.Add(new ModuleKeyValueItem(Translate.Text("Corporate E-mail Address"), item.User_Email_Address));
                    if (fileslist != null && fileslist.Count > 0 && fileslist.Any(x => x.Reference_ID.Equals(item.Manufacturer_Code)))
                    {
                        fileslist.Where(x => x.Reference_ID.Equals(item.Manufacturer_Code) && x.File_Type != FileType.TrademarkLogo && x.File_Type != FileType.AuthorizationLetter).OrderBy(x => x.File_Type).ForEach(x => moduleSection5.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
                    }
                    moduleSectionRow3.moduleSections.Add(moduleSection4);
                    moduleSectionRow3.moduleSections.Add(moduleSection5);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow3);
                    ModuleSectionRow moduleSectionRow4 = new ModuleSectionRow { Title = Translate.Text("Authorization letter") };
                    ModuleSection moduleSection6 = new ModuleSection();
                    if (fileslist != null && fileslist.Count > 0 && fileslist.Any(x => x.Reference_ID.Equals(item.Manufacturer_Code)))
                    {
                        fileslist.Where(x => x.Reference_ID.Equals(item.Manufacturer_Code) && x.File_Type == FileType.AuthorizationLetter).OrderBy(x => x.File_Type).ForEach(x => moduleSection6.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
                    }
                    moduleSectionRow4.moduleSections.Add(moduleSection6);
                    moduleDetail.moduleSectionRows.Add(moduleSectionRow4);
                    if (rejectcomments != null && rejectcomments.Count() > 0)
                    {
                        ModuleSectionRow moduleSectionRow5 = new ModuleSectionRow { Title = Translate.Text("Rejected Remarks") };
                        ModuleSection moduleSection3 = new ModuleSection();
                        foreach (var comments in rejectcomments)
                        {
                            moduleSection3.moduleItems.Add(new ModuleKeyValueItem(string.Empty, comments.Rejected_Reason));
                        }

                        if (rejectfiles != null && rejectfiles.Count > 0)
                        {
                            rejectfiles.ForEach(x => moduleSection3.moduleItems.Add(new ModuleKeyValueItem(Translate.Text(x.File_Type), x.File_ID.ToString(), filelink: true)));
                        }
                        moduleSectionRow5.moduleSections.Add(moduleSection3);
                        moduleDetail.moduleSectionRows.Add(moduleSectionRow5);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return moduleDetail;
        }

        public bool fnValidateUser(string UserName, string UserPassword, string code, string role)
        {
            bool validation;
            try
            {
                LdapConnection ldapConn = new LdapConnection
                        (new LdapDirectoryIdentifier((string)null, false, false));
                NetworkCredential nc = new NetworkCredential(UserName,
                                       UserPassword, "DEWA");
                ldapConn.Credential = nc;
                ldapConn.AuthType = AuthType.Negotiate;
                ldapConn.Bind(nc);
                validation = true;
                if (validation)
                {
                    using (PrincipalContext prcontext = new PrincipalContext(ContextType.Domain))
                    {
                        UserPrincipal usr = UserPrincipal.FindByIdentity(prcontext, UserName);
                        if (usr != null)
                        {
                            using (DRRGEntities context = new DRRGEntities())
                            {
                                ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                                ObjectParameter SessionParamresponse = new ObjectParameter(DRRGStandardValues.sessionout, typeof(string));
                                context.SP_DRRG_AdminLogin(UserName, myOutputParamresponse, SessionParamresponse);
                                string myString = Convert.ToString(myOutputParamresponse.Value);
                                string session = Convert.ToString(SessionParamresponse.Value);
                                if (!string.IsNullOrWhiteSpace(myString) && !string.IsNullOrWhiteSpace(session) && myString.Equals(DRRGStandardValues.Success))
                                {
                                    AuthStateService.Save(new DewaProfile(UserName, session, role)
                                    {
                                        IsContactUpdated = true,
                                        Name = usr.DisplayName,
                                        EmailAddress = usr.EmailAddress,
                                        BusinessPartner = code
                                    });
                                    if (Request.Cookies[GenericConstants.AntiHijackCookieName] != null)
                                    {
                                        Response.Cookies[GenericConstants.AntiHijackCookieName].Expires = DateTime.UtcNow.AddMinutes(60);
                                    }
                                    System.Web.HttpContext.Current.Session.Timeout = 60;
                                    return validation;
                                }
                                else
                                {
                                    LogService.Error(new System.Exception(myString), this);
                                    ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                                }
                            }
                        }
                        validation = false;
                    }
                }
            }
            catch (LdapException ex)
            {
                LogService.Fatal(ex, this);
                validation = false;
            }
            return validation;
        }
        public string GetRemarks(string status, string refno)
        {
            string strRemarks = string.Empty;
            string strFiles = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(status) && (status.Equals("Submitted") || status.Equals("ReviewerApproved") || status.Equals("Updated")))
                {
                    strRemarks = Translate.Text("DRRG_UnderEvaluationRemarks");
                }
                else if (!string.IsNullOrWhiteSpace(status) && (status.Equals("ReviewerRejected") || status.Equals("Rejected")))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var resultRejected = context.DRRG_Rejected.Where(x => x.Reference_ID.ToLower() == refno.ToLower()).ToList();
                        foreach (var itemRejected in resultRejected)
                        {
                            if (!string.IsNullOrWhiteSpace(strRemarks))
                            {
                                strRemarks += @"</br> " + itemRejected.Rejected_Reason;
                            }
                            else
                                strRemarks = @"<strong>Remarks:</strong> </br> " + itemRejected.Rejected_Reason;

                            var rejectedFiles = context.DRRG_Files.Where(x => x.Reference_ID.ToLower() == itemRejected.Rejected_File_id.ToLower()).ToList();

                            foreach (var item in rejectedFiles)
                            {
                                if (!string.IsNullOrWhiteSpace(strFiles))
                                {
                                    strFiles += @"</br> <a data-fileid='" + item.File_ID + "' href='#' target='_blank' class='link filelink tooltipstered'>View</a>";
                                }
                                else
                                {
                                    strFiles = @"<strong>Screenshot:</strong> </br> <a data-fileid='" + item.File_ID + "' href='#' target='_blank' class='link filelink tooltipstered'>View</a>";
                                }
                            }
                        }
                    }
                }
                else if (!string.IsNullOrWhiteSpace(status) && status.Equals("Deleted"))
                {
                    strRemarks = Translate.Text("Deleted");
                }
                else if (!string.IsNullOrWhiteSpace(status) && status.Equals("Approved"))
                {
                    strRemarks = Translate.Text("DRRG_ApprovedRemarks").Replace("{clickhere}", "<a href='" + @LinkHelper.GetItemUrl(SitecoreItemIdentifiers.Eligible_List_APPLICATION) + "'><span class='green'>Click here</span></a>");
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return strRemarks + "</br>" + strFiles;
        }
        public string GetEvaluator(string email)
        {
            string fullName = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var evaluatorDetails = context.DRRG_EvaluatorLogin.Where(x => x.loginid == email).FirstOrDefault();
                    if (evaluatorDetails != null)
                        fullName = evaluatorDetails.Name;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return fullName;
        }
        public string getExtraCompliance(string refId, string type, DRRGEntities context)
        {
            string compliance = string.Empty;
            try
            {
                //PV - IEC62716,IEC60068-2-68
                if (type.ToLower().Trim() == "pv")
                {
                    if (context.DRRG_Files.Any(x => x.File_Type == FileType.IEC62716 && x.Reference_ID == refId))
                    {
                        compliance = Translate.Text("DRRG_IEC62716");
                    }
                    if (context.DRRG_Files.Any(x => x.File_Type == FileType.IEC60068 && x.Reference_ID == refId))
                    {
                        if (!string.IsNullOrEmpty(compliance))
                            compliance += ", " + Translate.Text("DRRG_IEC60068");
                        else
                            compliance = Translate.Text("DRRG_IEC60068");
                    }
                }
                else if (type.ToLower().Trim() == "ip")
                {
                    if (context.DRRG_Files.Any(x => x.File_Type == FileType.IEC610101 && x.Reference_ID == refId))
                    {
                        compliance = Translate.Text("DRRG_IEC610101");
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return compliance;
        }
        public string GetNominalPowerDetails(DRRG_PVMODULE pvitem)
        {
            var nominalpower = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var nominalPowerLst = context.DRRG_PVModule_Nominal.Where(x => x.PV_ID.ToLower() == pvitem.PV_ID.ToLower()).ToList();
                    if (!string.IsNullOrWhiteSpace(pvitem.Nominal_Power) && pvitem.Nominal_Power.Equals(NominalPowerType.NominalPowerRange))
                    {
                        if (nominalPowerLst != null && nominalPowerLst.Count > 0)
                        {
                            nominalpower = nominalPowerLst[0].wp1 + " Wp - " + nominalPowerLst[0].wp2 + " Wp , in step of " + nominalPowerLst[0].wp3 + " W";
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(pvitem.Nominal_Power) && pvitem.Nominal_Power.Equals(NominalPowerType.MultipleNominalPowerEntries))
                    {
                        foreach (var item in nominalPowerLst)
                        {
                            if (!string.IsNullOrWhiteSpace(nominalpower))
                            {
                                nominalpower = nominalpower + ", " + item.wp1.ToString() + " Wp";
                            }
                            else
                            {
                                nominalpower = item.wp1 + " Wp";
                            }
                        }
                    }
                    else
                    {
                        if (nominalPowerLst != null && nominalPowerLst.Count > 0)
                            nominalpower = nominalPowerLst[0].wp1 + " Wp";
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return nominalpower;
        }
        public string GetRatedPower(DRRG_InverterModule invmodule)
        {
            var RatedPower = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var resultRP = context.DRRG_InverterModule_RP.Where(x => x.Inverter_ID.ToLower() == invmodule.Inverter_ID.ToLower()).ToList();

                    foreach (var item in resultRP)
                    {
                        if (!string.IsNullOrWhiteSpace(RatedPower))
                            RatedPower = RatedPower + ", " + item.Rated_Power.ToString() + " kW";
                        else
                            RatedPower = item.Rated_Power.ToString() + " kW";
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return RatedPower;
        }
        public string GetACParentPower(DRRG_InverterModule invmodule)
        {
            var ACParentPower = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var resultAP = context.DRRG_InverterModule_AP.Where(x => x.Inverter_ID.ToLower() == invmodule.Inverter_ID.ToLower()).ToList();

                    foreach (var item in resultAP)
                    {
                        if (!string.IsNullOrWhiteSpace(ACParentPower))
                            ACParentPower = ACParentPower + ", " + item.AC_Apparent_Power.ToString() + " kVA";
                        else
                            ACParentPower = item.AC_Apparent_Power.ToString() + " kVA";
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return ACParentPower;
        }
    }
}
