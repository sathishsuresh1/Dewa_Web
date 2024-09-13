using DEWAXP.Feature.Builder.Models.ProjectGeneration;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.AccountModel;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Logger;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.SecurityModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;
using Roles = DEWAXP.Foundation.Content.Roles;

namespace DEWAXP.Feature.Builder.Controllers
{
    public class ProjectGenerationController : BaseController
    {
        [HttpGet]
        public ActionResult LoginMain(string returnUrl)
        {
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.Consultant))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_DOCUMENTUPLOADPAGE);
            }

            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            ViewBag.ReturnUrl = returnUrl;
            return PartialView("~/Views/Feature/Builder/ProjectGeneration/_LoginFormMain.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult LoginMain(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error;
                    if (TryLogin(model, out error))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_DOCUMENTUPLOADPAGE);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                }
                catch (Exception)
                {
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
        }

        private bool TryLogin(LoginModel model, out string error)
        {
            error = null;
            var projectUsers = ContentRepository.GetItem<ProjectUsers>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.PROJECTGENERATION_USERS) { Language= Language.Parse("en") });

            var currentUser =
                projectUsers.Users.FirstOrDefault(
                    c =>
                        c.UserName.ToLower() == model.Username.ToLower() &&
                        string.Equals(c.Password, model.Password.Trim(), StringComparison.InvariantCulture));

            if (currentUser != null)
            {
                CacheProvider.Store(CacheKeys.PROJECTGENERATION_USERDATA, new AccessCountingCacheItem<ProjectUser>(currentUser, Times.Max));
                AuthStateService.Save(new DewaProfile(model.Username, string.Empty, Roles.Consultant));
                RemoveSecurityCode(currentUser.Id);
                return true;
            }
            error = Translate.Text(DictionaryKeys.ProjectGeneration.InvalidAccount);
            return false;
        }

        private bool CheckUserSendResetMail(ForgotPasswordModel model, out string error, SupportedLanguage language = SupportedLanguage.English, RequestSegment segment = RequestSegment.Desktop)
        {
            error = null;
            var projectUsers = ContentRepository.GetItem<ProjectUsers>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.PROJECTGENERATION_USERS) { Language = Language.Parse("en") });

            var currentUser =
                projectUsers.Users.FirstOrDefault(
                    c =>
                        c.UserName.ToLower() == model.Username.ToLower() &&
                        string.Equals(c.Emailaddress, model.Email.Trim(), StringComparison.InvariantCulture));

            if (currentUser != null)
            {
                var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();
                //currentUser.Id

                Item myitem = Database.GetDatabase("master").GetItem(new Sitecore.Data.ID(Guid.Parse(currentUser.Id.ToString())));
                var securitycode = Guid.NewGuid().ToString();
                using (new SecurityDisabler())
                {
                    myitem.Editing.BeginEdit();
                    myitem["SecurityCode"] = securitycode;
                    myitem.Editing.EndEdit();
                }
                UtilExtensions.PublishItem(myitem, "web");
                string from = !string.IsNullOrEmpty(ConfigurationManager.AppSettings["DEWA_FROMEMAIL"]) ? ConfigurationManager.AppSettings["DEWA_FROMEMAIL"] : "no-reply@dewa.gov.ae";
                var emailsubject = ContentRepository.GetItem<FormattedText>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.J86_EMAILSUBJECT)));
                string subject = emailsubject != null ? emailsubject.RichText : "Reset Password Details";
                var emailtemplate = ContentRepository.GetItem<FormattedText>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.J86_EMAILTEMPlATE)));
                string body = emailtemplate != null ? emailtemplate.RichText : @"Dear Sir / Madam,
                        You Can Reset the password from the following link.
                        Your User ID: *USERNAME*
                    Please reset you password at *LOGINLINK* .
                    Note: You need Internet Explorer version 8 or above to use the system.";

                var link = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.J86_SET_NEW_PASSWORD, false);
                var LOGINLINK = link + "?userid=" + currentUser.Id.ToString() + "&dynli=" + securitycode;
                body = body.Replace("*USERNAME*", currentUser.UserName);
                body = body.Replace("*LOGINLINK*", LOGINLINK);
                string useEmail = currentUser.Emailaddress;
                emailserviceclient.SendEmail(from, useEmail, subject, body);

                return true;
            }
            error = Translate.Text(DictionaryKeys.ProjectGeneration.InvalidAccount);
            return false;
        }

        [HttpGet]
        public ActionResult SubmitDocument()
        {
            var projectDcumentSubmissionModel = new ProjectDcumentSubmissionModel();
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Consultant))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }

            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            ProjectUser currentuser;
            if (!CacheProvider.TryGet(CacheKeys.PROJECTGENERATION_USERDATA, out currentuser))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }
            if (currentuser != null)
            {
                if (!currentuser.Projects.Any())
                {
                    return PartialView("~/Views/Feature/Builder/ProjectGeneration/DocumentSubmissionNoProjects.cshtml", projectDcumentSubmissionModel);
                }
                projectDcumentSubmissionModel.Projects = currentuser.Projects;
                return PartialView("~/Views/Feature/Builder/ProjectGeneration/DocumentSubmission.cshtml", projectDcumentSubmissionModel);
            }
            else
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitDocument(ProjectDcumentSubmissionModel model)
        {
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.Consultant))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }

            string error;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }
            ProjectUser currentuser;
            if (!CacheProvider.TryGet(CacheKeys.PROJECTGENERATION_USERDATA, out currentuser))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }
            var projectconfig = ContentRepository.GetItem<ProjectConfiguration>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.J86_PPGCONFIG)));

            if (ModelState.IsValid)
            {
                var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();

                try
                {
                    var fileobjectid = string.Empty;
                    var selectedproject = ContentRepository.GetItem<Project>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(model.ProjectId)));
                    var SessionId = this.ProjectGenerationClient.ConnectToDocbase(DocumentumConfig.DMS_CONNECTUSER,
                        DocumentumConfig.DMS_CONNECTPASSWORD, DocumentumConfig.DMS_DOCBASE);
                    if (SessionId != null && SessionId.Succeeded)
                    {
                        var folderobjectid = this.ProjectGenerationClient.CreateFolder(SessionId.Payload,
                            DocumentumConfig.DMS_CONNECTUSER, selectedproject.DMSFolder);
                        if (folderobjectid != null && folderobjectid.Succeeded)
                        {
                            if (folderobjectid.Payload.Length == 16)
                            {
                                if (Request.Files != null && Request.Files.Count > 0)
                                {
                                    //if (this.AttachmentIsValid(General.MaxPGPAttachmentSize, out error,
                                    // General.AcceptedFileTypes))

                                    //Update By Syed Shujaat Ali
                                    //Get attachment size from sitecore

                                    if (this.AttachmentIsValid(projectconfig.AttachmentSize, out error,
                                        General.AcceptedFileTypes))

                                    {
                                        string DMS_Attributes = this.CreateAttributeSet(model, selectedproject,
                                           currentuser.UserName);
                                        List<string> Successfiles = new List<string>();
                                        foreach (string fileName in Request.Files)
                                        {
                                            HttpPostedFileBase myfile = Request.Files[fileName];
                                            if (myfile != null && !string.IsNullOrEmpty(myfile.FileName) &&
                                                myfile.ContentLength > 0)
                                            {
                                                var filebytes = myfile.ToArray();
                                                var filetype = myfile.FileName.GetFileExtensionTrimmed();
                                                var response = this.ProjectGenerationClient.UploadFileToDMS
                                                    (SessionId.Payload, DocumentumConfig.DMS_CONNECTUSER, DMS_Attributes,
                                                        selectedproject.DMSFolder, DocumentumConfig.DMS_DOCUMENTTYPE,
                                                        filetype.ToLower(), filebytes);
                                                if (response.Succeeded)
                                                {
                                                    if (response.Payload != null && response.Payload.Length == 16)
                                                    {
                                                        Successfiles.Add(myfile.FileName.GetFileNameWithoutPath());
                                                        fileobjectid = response.Payload;
                                                    }
                                                    else
                                                    {
                                                        ModelState.AddModelError(string.Empty, Translate.Text("Error while uploading the File"));
                                                    }
                                                }
                                            }
                                        }

                                        this.ProjectGenerationClient.DisconnectDocumentum(SessionId.Payload,
                                            DocumentumConfig.DMS_CONNECTUSER);

                                        var successmodel = new ProjectDcumentSubmissionSuccessModel
                                        {
                                            ProjectName = selectedproject.ProjectName,
                                            Subject = model.Subject,
                                            DocumentReference = model.DocumentReference,
                                            ReferenceDate = model.ReferenceDate,
                                            FileName = Successfiles,
                                            FileObjectID = fileobjectid
                                        };
                                        CacheProvider.Store(CacheKeys.PROJECTGENERATION_SUCCESSDATA,
                                            new AccessCountingCacheItem<ProjectDcumentSubmissionSuccessModel>(successmodel,
                                                Times.Max));

                                        if (!string.IsNullOrEmpty(fileobjectid))
                                        {
                                            if (fileobjectid.Length == 16)
                                            {
                                                emailserviceclient.SendEmail(projectconfig.EmailFrom, currentuser.Emailaddress, projectconfig.Subject, this.CreateEmailBody(successmodel, selectedproject));

                                                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_DOCUMENTSUCCESSPAGE);
                                            }
                                        }
                                        else
                                        {
                                            ModelState.AddModelError(string.Empty, projectconfig.ErrorMessage);
                                        }
                                    }
                                    else
                                    {
                                        ModelState.AddModelError(string.Empty, error);
                                    }
                                }
                            }
                            else
                            {
                                ModelState.AddModelError(string.Empty, folderobjectid.Payload);
                            }
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("Create folderobjectid is null"));
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("ConnectToDocbase is not working"));
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, new object());
                    ModelState.AddModelError(string.Empty, ex.Message);
                    //emailserviceclient.Send_Mail("no-reply@dewa.gov.ae", "syed.shujaat@dewa.gov.ae", "Project Generation Document Submission ! Error",ex.InnerException.ToString());
                }
            }
            model.Projects = currentuser.Projects;
            return View("~/Views/Feature/Builder/ProjectGeneration/DocumentSubmission.cshtml", model);
        }

        private string CreateAttributeSet(ProjectDcumentSubmissionModel model, Project selectedproject, string UserName)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement element1 = doc.CreateElement(string.Empty, "DMS_Attributes", string.Empty);
            doc.AppendChild(element1);
            this.AppendAttribute(doc, element1, "object_name", model.Subject + " - " + model.DocumentReference + " - " + model.ReferenceDate.ToString("dd-MMM-yyyy"));
            this.AppendAttribute(doc, element1, "subject", model.Subject);
            this.AppendAttribute(doc, element1, "title", selectedproject.ProjectName);
            this.AppendAttribute(doc, element1, "b_department", "P&P(G)");
            this.AppendAttribute(doc, element1, "d_project_no", selectedproject.ContractNumber);
            this.AppendAttribute(doc, element1, "d_sender_name", UserName);
            this.AppendAttribute(doc, element1, "f_reference_no", model.DocumentReference);
            this.AppendAttribute(doc, element1, "h_ref_dt", model.ReferenceDate.ToString("dd-MMM-yyyy"));
            this.AppendAttribute(doc, element1, "c_scanned_by", "eService System");
            this.AppendAttribute(doc, element1, "g_send_date", DateTime.Now.ToString("MM/dd/yyyy"));
            this.AppendAttribute(doc, element1, "acl_domain", selectedproject.AclDomain);
            this.AppendAttribute(doc, element1, "acl_name", selectedproject.AclName);
            return doc.InnerXml.Replace("&amp;", "&");
        }

        private void AppendAttribute(XmlDocument doc, XmlElement element, string name, string value)
        {
            XmlElement myelement = doc.CreateElement(string.Empty, "Attribute", string.Empty);
            myelement.SetAttribute("Name", name);
            myelement.SetAttribute("Value", value);
            element.AppendChild(myelement);
        }

        [HttpGet]
        public ActionResult SubmitDocumentSuccess()
        {
            ProjectDcumentSubmissionSuccessModel model;
            if (!CacheProvider.TryGet(CacheKeys.PROJECTGENERATION_SUCCESSDATA, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }

            return PartialView("~/Views/Feature/Builder/ProjectGeneration/DocumentSubmissionSuccess.cshtml", model);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return PartialView("~/Views/Feature/Account/Forms/_ForgotPassword.cshtml", new ForgotPasswordModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string error;
                    if (CheckUserSendResetMail(model, out error, RequestLanguage, Request.Segment()))
                    {
                        var recoveryEmailModel = new RecoveryEmailSentModel
                        {
                            EmailAddress = model.Email,
                            Context = RecoveryContext.Password
                        };

                        CacheProvider.Store(CacheKeys.RECOVERY_EMAIL_STATE, new CacheItem<RecoveryEmailSentModel>(recoveryEmailModel));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_RECOVERY_EMAIL_SENT);
                    }
                    ModelState.AddModelError(string.Empty, error);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            return PartialView("~/Views/Feature/Account/Forms/_ForgotPassword.cshtml", model);
        }

        [HttpGet]
        public ActionResult SetNewPassword(string userid, string dynli)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(dynli))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }

            try
            {
                Guid ownerIdGuid = Guid.Empty;
                ownerIdGuid = new Guid(userid);
                if (!CheckSecurityCode(ownerIdGuid, dynli))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/Account/Forms/_SetNewPassword.cshtml", new SetNewPasswordModel
            {
                SessionToken = dynli,
                Username = userid
            });
        }

        private bool CheckSecurityCode(Guid userid, string dynli)
        {
            var projectUsers = ContentRepository.GetItem<ProjectUsers>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.PROJECTGENERATION_USERS) { Language = Language.Parse("en") });

            var currentUser =
                projectUsers.Users.FirstOrDefault(
                    c =>
                        c.Id == userid &&
                        string.Equals(c.SecurityCode, dynli.Trim(), StringComparison.InvariantCulture));

            if (currentUser != null)
            {
                return true;
            }
            return false;
        }

        private void RemoveSecurityCode(Guid userid)
        {
            var projectUsers = ContentRepository.GetItem<ProjectUsers>(new Glass.Mapper.Sc.GetItemByPathOptions(SitecoreItemPaths.PROJECTGENERATION_USERS) { Language = Language.Parse("en") });

            var currentUser =
                projectUsers.Users.FirstOrDefault(
                    c =>
                        c.Id == userid);

            if (currentUser != null)
            {
                Item myitem = Database.GetDatabase("master").GetItem(new Sitecore.Data.ID(Guid.Parse(currentUser.Id.ToString())));

                using (new SecurityDisabler())
                {
                    myitem.Editing.BeginEdit();
                    myitem["SecurityCode"] = "";
                    myitem.Editing.EndEdit();
                }
                UtilExtensions.PublishItem(myitem, "web");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(SetNewPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Guid ownerIdGuid = Guid.Empty;
                    ownerIdGuid = new Guid(model.Username);
                    ChangePassword(ownerIdGuid, model.Password);
                    RemoveSecurityCode(ownerIdGuid);
                    //return RedirectToSitecoreItem(SitecoreItemIdentifiers.J60_CHANGE_PASSWORD_SUCCESSFUL);
                    return PartialView("~/Views/Feature/Builder/ProjectGeneration/_ChangePasswordSuccessful.cshtml");
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            return PartialView("~/Views/Feature/Account/Forms/_SetNewPassword.cshtml", model);
        }

        protected bool AttachmentIsValid(long maxBytes, out string error, params string[] allowedExtensions)
        {
            error = string.Empty;
            long totalcontentlength = 0;
            if (Request.Files != null && Request.Files.Count > 0)
            {
                foreach (string fileName in Request.Files)
                {
                    HttpPostedFileBase file = Request.Files[fileName];
                    if (file != null && !string.IsNullOrEmpty(file.FileName) && file.ContentLength > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        if (string.IsNullOrWhiteSpace(ext) && allowedExtensions.Any())
                        {
                            error = Translate.Text("invalid file type validation message");
                            return false;
                        }
                        if (allowedExtensions.Any() && !allowedExtensions.Any(e => e.Equals(ext, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            error = Translate.Text("invalid file type validation message");
                            return false;
                        }
                        totalcontentlength += file.ContentLength;
                    }
                }
            }

            if (totalcontentlength > maxBytes)
            {
                error = Translate.Text("file too large validation message");
                return false;
            }

            return true;
        }

        public ActionResult ChangePassword()
        {
            ProjectUser currentuser;
            if (!CacheProvider.TryGet(CacheKeys.PROJECTGENERATION_USERDATA, out currentuser))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }

            return View("~/Views/Feature/Builder/ProjectGeneration/_ChangePassword.cshtml", new ChangePasswordModel
            {
                UserId = CurrentPrincipal.Username
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ProjectUser currentuser;
                    if (!CacheProvider.TryGet(CacheKeys.PROJECTGENERATION_USERDATA, out currentuser))
                    {
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
                    }
                    if (String.CompareOrdinal(model.OldPassword, model.NewPassword) == 0)
                    {
                        return PartialView("~/Views/Feature/Builder/ProjectGeneration/_ChangePasswordSuccessful.cshtml", model);
                    }
                    if (String.CompareOrdinal(model.OldPassword, currentuser.Password) != 0)
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("OldPasswordInvalid"));
                        return PartialView("~/Views/Feature/Builder/ProjectGeneration/_ChangePassword.cshtml", model);
                    }
                    this.ChangePassword(currentuser.Id, model.NewPassword);
                    return PartialView("~/Views/Feature/Builder/ProjectGeneration/_ChangePasswordSuccessful.cshtml", model);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            // Reset password fields
            var emptyValue = new ValueProviderResult(string.Empty, string.Empty, CultureInfo.CurrentCulture);
            ModelState.SetModelValue("OldPassword", emptyValue);
            model.OldPassword = string.Empty;

            ModelState.SetModelValue("NewPassword", emptyValue);
            model.NewPassword = string.Empty;

            ModelState.SetModelValue("ConfirmPassword", emptyValue);
            model.ConfirmPassword = string.Empty;

            return PartialView("~/Views/Feature/Builder/ProjectGeneration/_ChangePassword.cshtml", model);
        }

        private void ChangePassword(Guid UserId, string newpassword)
        {
            Database masterDB = Factory.GetDatabase("master");
            Item myitem = masterDB.GetItem(new ID((UserId)));
            using (new SecurityDisabler())
            {
                myitem.Editing.BeginEdit();
                myitem["Password"] = newpassword;
                myitem.Editing.EndEdit();
            }
            UtilExtensions.PublishItem(myitem, "web");
        }

        [HttpGet]
        public ActionResult RecoveryEmailSent()
        {
            RecoveryEmailSentModel model;
            if (!CacheProvider.TryGet(CacheKeys.RECOVERY_EMAIL_STATE, out model))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }
            return View("~/Views/Feature/Account/Forms/_RecoveryEmailSent.cshtml", model);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            if (IsLoggedIn)
            {
                CacheProvider.Remove(CacheKeys.PROJECTGENERATION_USERDATA);
                FormsAuthentication.SignOut();
                Session.Abandon();
                Session.Clear();

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J86_LOGIN_PAGE);
        }

        private string CreateEmailBody(ProjectDcumentSubmissionSuccessModel ProjectObject, Project selectedproject)
        {
            string emailTemplate = string.Empty;

            emailTemplate = "Dear Sir/Madam <br/><br/>Your Document has been received.<br/>Please Note Down the Following:";

            emailTemplate += "<br/><br/>Document:";
            emailTemplate += "<br/>---------------";
            emailTemplate += "<br/>Contract No : " + selectedproject.ContractNumber;
            emailTemplate += "<br/>Subject : " + ProjectObject.Subject;
            emailTemplate += "<br/>Reference No :" + ProjectObject.DocumentReference;
            emailTemplate += "<br/>Reference Date :" + ProjectObject.ReferenceDate.ToShortDateString();
            emailTemplate += "<br/>Object ID :" + ProjectObject.FileObjectID;

            foreach (var filenname in ProjectObject.FileName)
            {
                emailTemplate += "<br/><br/>Document Name :" + filenname;
            }

            emailTemplate += "<br/><br/>Regards,";
            emailTemplate += "<br/>Project Generation";
            emailTemplate += "<br/>DEWA";

            return emailTemplate;
        }
    }
}