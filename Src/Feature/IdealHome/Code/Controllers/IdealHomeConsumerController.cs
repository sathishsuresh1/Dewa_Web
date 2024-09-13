using DEWAXP.Feature.IdealHome.Models.Configurations.Email;
using DEWAXP.Feature.IdealHome.Models.Configurations.Html_Template;
using DEWAXP.Feature.IdealHome.Models.IdealHomeConsumer;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Models.DewaStore;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Content.Services;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Logger;
using Glass.Mapper;
using Glass.Mapper.Sc;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.StyledXmlParser.Jsoup.Nodes;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Extensions;
using Sitecore.Globalization;
using Sitecore.Links;
using Sitecore.Links.UrlBuilders;
using Sitecore.Mvc.Presentation;
using Sitecore.Publishing;
using Sitecore.SecurityModel;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using X.PagedList;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using Context = Sitecore.Context;
using Document = iText.Layout.Document;
using ID = Sitecore.Data.ID;
using Roles = DEWAXP.Foundation.Content.Roles;
using Sitecorex = Sitecore;

namespace DEWAXP.Feature.IdealHome.Controllers
{
    public class MyModel
    {
        public string UserName { get; set; }
        public string to { get; set; }

        public string cc { get; set; }
        public string bcc { get; set; }
        public string subject { get; set; }
        public string body { get; set; }

        public string ImgPath { get; set; }
        public string base64string { get; set; }
    }

    public class IdealHomeConsumerController : BaseController
    {
        #region Properties and Variables

        private enum EnumVideoStatus
        { NotWatched, Watched };

        //ISitecoreService sitecoreService = null;

        #endregion Properties and Variables
        //private readonly ISitecoreService sitecoreService;

        //public IdealHomeConsumerController()
        //{
        //    sitecoreService = DependencyResolver.Current.GetService<ISitecoreService>();
        //}
        #region Actions


        public void SendEmailAttachment(string imgPath, string userName, string to, string cc, string bcc, string subject, string body, string filename)
        {
            var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();

            byte[] filebytearray = ExportPdf(imgPath, userName);//Convert.FromBase64String(model.base64string);
            List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
            attList.Add(new Tuple<string, byte[]>(userName + ".pdf", filebytearray));
            var response = emailserviceclient.SendEmail("no-reply@dewa.gov.ae", to, cc, bcc, subject, body, attList);
        }

        public byte[] ExportPdf(string imgPath, string overlayText)
        {
            string html = "<img src='" + imgPath + "' />"; //File.ReadAllText(Server.MapPath("/html/index2.html"));

            StringReader sr = new StringReader("");
            //PdfDocument pdfDoc = new PdfDocument(PageSize.A4.Rotate(), 10f, 10f, 10f, 0f);

            using (MemoryStream ms = new MemoryStream())
            {
                PdfDocument pdfDoc = new PdfDocument(new PdfWriter(ms));
                Document doc = new Document(pdfDoc, PageSize.A4.Rotate());
                doc.SetMargins(10f, 10f, 10f, 0f);
                Table table = new Table(UnitValue.CreatePercentArray(new float[] { 3, 3, 3 })).UseAllAvailableWidth();
                table.SetWidth(new UnitValue(2, 100));
                table.SetBaseDirection(BaseDirection.RIGHT_TO_LEFT);
                Paragraph paragraph = new Paragraph(overlayText).SetTextAlignment(TextAlignment.CENTER);
                Cell cell = new Cell();
                cell.Add(paragraph);
                table.AddCell(cell);
                Paragraph p = new Paragraph(overlayText).SetTextAlignment(TextAlignment.CENTER);
                p.SetFixedPosition(100, 800, 200);
                Image image_1 = new Image(ImageDataFactory.Create(imgPath));
                PdfFont pdfFont = PdfFontFactory.CreateFont(Server.MapPath("~/fonts/ARIALUNI.ttf"), "Identity-H", true);
                doc.Add(getWatermarkedImage(pdfDoc, image_1, overlayText, pdfFont));
                doc.Add(p);
                doc.Close();
                byte[] bytes = ms.ToArray();
                ms.Close();
                return bytes;
            }
        }
        private static Image getWatermarkedImage(PdfDocument pdfDoc, Image img, String watermark, PdfFont pdfFont)
        {
            float width = img.GetImageScaledWidth();
            float height = img.GetImageScaledHeight();
            float coordX = width / 2;
            float coordY = height / 2;
            //float angle = (float)Math.PI * 30f / 180f;
            PdfFormXObject template = new PdfFormXObject(new Rectangle(width, height));
            new Canvas(template, pdfDoc)
                    .Add(img)
                    .SetFont(pdfFont)
                    .SetFontColor(DeviceGray.BLACK)
                    .SetFontSize(35)
                    //.SetFontFamily(Server.MapPath("~/fonts/ARIALUNI.ttf"))
                    .ShowTextAligned(watermark, coordX, coordY, TextAlignment.CENTER)
                    .Close();
            return new Image(template);
        }

        [HttpGet]
        public ActionResult EmailPDF()
        {
            string _path = string.Empty;
            string body = string.Empty;
            string subject = "Ideal Home Certificate";
            string filename = CurrentPrincipal.Name;
            string UserName = CurrentPrincipal.Name;
            string emailaddress = CurrentPrincipal.EmailAddress;
            if (Sitecorex.Context.Language.Name == "ar-AE")
            {
                _path = Server.MapPath("/images/idealhome/certificate_ar.jpg"); //Server.MapPath("/images/idealhome/certificate_ar.jpg");
            }
            else
            {
                _path = Server.MapPath("/images/idealhome/certificate_en.jpg");
            }
            var _htmlConfig = ContentRepository.GetItem<HtmlTemplateConfigurations>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.IDEALHOMECONSUMER_HTML_TEMPLATE_CONFIG)));

            if (_htmlConfig.HtmlImage != null)
            {
                ViewBag.HtmlTemplate = _htmlConfig.HtmlImage.Src;//_htmlConfig.HtmlText.Replace("$Name", _surveyResponseModel.FullName.ToUpper()).Replace("urltoken", ContextUrl());
                body = _htmlConfig.HtmlEmail;
                body = body.Replace("\n", "<br/>");
            }
            // return resulted pdf document
            SendEmailAttachment(_path, UserName, emailaddress, emailaddress, emailaddress, subject, body, filename);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_DASHBOARD);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DownloadPDF()
        {
            string _path = string.Empty;
            var fileName = CurrentPrincipal.Name;
            if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
            {
                return null;
            }
            if (Sitecorex.Context.Language.Name == "ar-AE")
            {
                _path = Server.MapPath("/images/idealhome/certificate_ar.jpg");
            }
            else
            {
                _path = Server.MapPath("/images/idealhome/certificate_en.jpg");
            }
            if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(_path))
            {
                return File(ExportPdf(_path, fileName), "application/pdf", fileName + "-" + DateTime.Now.Ticks + ".pdf");
                // return resulted pdf document
                //FileResult fileResult = new FileContentResult(ExportPdf(_path, fileName), "application/pdf");
                //fileResult.FileDownloadName = fileName + "-" + DateTime.Now.Ticks + ".pdf";
                //return fileResult;
            }
            return null;
        }

        [HttpGet]
        public ActionResult Score()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }
                var _surveyResponseModel = DisplayScoreStats();
                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/Module/_Score.cshtml", _surveyResponseModel);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult DisplaySubHeader()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                var _userModel = DisplayDashBoard();
                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/Module/_SubHeader.cshtml", _userModel);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult LogOut()
        {
            if (IsLoggedIn)
            {
                CacheProvider.Remove(CacheKeys.IDEAL_HOME_USER_DATA);
                FormsAuthentication.SignOut();
                Session.Abandon();
                Session.Clear();

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
            }
            Session["Admin"] = false;
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            var x = FetchFutureCenterValues();

            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.IdealHome))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_DASHBOARD);
            }
            string error;

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            ViewBag.ReturnUrl = returnUrl;

            return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_Login.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(User model)
        {
            if (ModelState.Values != null)
            {
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string error;
                    bool isAdmin;
                    if (TryLogin(model, out error, out isAdmin))
                    {
                        if (isAdmin)
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_USER_DATA);
                        }
                        else
                        {
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_DASHBOARD);
                        }
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            string error;

            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
            {
                ModelState.AddModelError(string.Empty, error);
            }

            return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_ForgotPassword.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(User model)
        {
            if (ModelState.Values != null)
            {
                foreach (var modelValue in ModelState.Values)
                {
                    modelValue.Errors.Clear();
                }
            }
            if (ModelState.IsValid)
            {
                try
                {
                    string error;
                    User refModel;

                    if (TryLogin(model, out error, out refModel, true))
                    {
                        model.FullName = refModel.FullName;
                        model.Item = refModel.Item;
                        SendPasswordEmail(model);
                    }
                    else
                    {
                        LogService.Error(new Exception(error), this);
                    }
                    ///CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                    //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Unexpected error"), Times.Once));
                }
            }
            else
            {
                LogService.Error(new Exception("Invalid details"), this);
                //CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_EMAIL_SUCCESS);
        }

        [HttpGet]
        public ActionResult SetNewPassword(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                try
                {
                    string error;

                    if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                    var _User = ContentRepository.GetItem<User>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(id)));
                    if (_User != null && !string.IsNullOrWhiteSpace(_User.ResetPasswordDate))
                    {
                        DateTime resetdateTime = DateTime.ParseExact(_User.ResetPasswordDate, "yyyyMMddHHmmss", null);
                        if (DateTime.Now.CompareTo(resetdateTime.AddHours(24)) <= 0)
                        {
                            return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_SetPassword.cshtml");
                        }
                        else
                        {
                            ViewBag.Linkexpired = true;
                            ModelState.AddModelError(string.Empty, Translate.Text("UnSubscribe failure"));
                            return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_SetPassword.cshtml");
                        }
                    }
                    else
                    {
                        LogService.Error(new Exception("Invalid User"), this);
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex, this);
                }
            }
            else
            {
                LogService.Error(new Exception("Invalid details"), this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SetNewPassword(IdealHomeSetNewPassword model)
        {
            if (ModelState.IsValid)
            {
                if (model != null && !string.IsNullOrWhiteSpace(model.Id) && model.Password.Equals(model.ConfirmPassword))
                {
                    try
                    {
                        var _User = ContentRepository.GetItem<User>(new GetItemByIdOptions(Guid.Parse(model.Id)));
                        if (_User != null && !string.IsNullOrWhiteSpace(_User.ResetPasswordDate))
                        {
                            DateTime resetdateTime = DateTime.ParseExact(_User.ResetPasswordDate, "yyyyMMddHHmmss", null);
                            if (DateTime.Now.CompareTo(resetdateTime.AddHours(24)) <= 0)
                            {
                                ViewBag.success = true;
                                _User.Password = model.ConfirmPassword.Trim();
                                _User.ResetPasswordDate = string.Empty;
                                using (new SecurityDisabler())
                                {
                                    GetSitecoreServiceMaster().SaveItem(_User);
                                }
                                PublishItem(_User.Item, false, false, PublishMode.SingleItem);
                                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_SetPassword.cshtml");
                            }
                            else
                            {
                                ViewBag.Linkexpired = true;
                                ModelState.AddModelError(string.Empty, Translate.Text("UnSubscribe failure"));
                                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_SetPassword.cshtml");
                            }
                        }
                        else
                        {
                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("Invalid details"), Times.Once));
                            LogService.Error(new Exception("Invalid User"), this);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex, this);
                    }
                }
                else
                {
                    LogService.Error(new Exception("Invalid details"), this);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
            }
            return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_SetPassword.cshtml");
        }

        [HttpGet]
        public ActionResult UserRegistration()
        {
            try
            {
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_DASHBOARD);
                }

                User model = new User();

                string error;

                if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out error))
                {
                    CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out model);

                    ModelState.AddModelError(string.Empty, error);
                }

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_UserRegistration.cshtml", GetDropDownList(model));
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UserRegistration(User model)
        {
            try
            {
                bool status = false;
                string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

                if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
                {
                    status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                }
                else if (!ReCaptchaHelper.Recaptchasetting())
                {
                    status = true;
                }

                if (!status)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
                string error;

                if (ModelState.IsValid && status)
                {
                    User refModel;

                    if (model.IsDEWACustomer)
                    {
                        if (!CheckContractAccount(model, out error))
                        {
                            model = GetDropDownList(model);

                            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));

                            CacheProvider.Store(CacheKeys.IDEAL_HOME_USER_MODEL, new CacheItem<User>(model));

                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_USER_REGISTRATION);
                        }
                    }

                    if (TryLogin(model, out error, out refModel))
                    {
                        error = Translate.Text(DictionaryKeys.IdealHomeConsumer.AccountAlreadyExists);

                        model = GetDropDownList(model);

                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));

                        CacheProvider.Store(CacheKeys.IDEAL_HOME_USER_MODEL, new CacheItem<User>(model));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_USER_REGISTRATION);
                    }
                    else
                    {
                        //Create sitecore user account
                        CreateUserAccount(model);
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_USER_REGISTRATION_SUCCESS);
                }

                // Recaptcha setting
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                model = GetDropDownList(model);
                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_UserRegistration.cshtml", model);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult DisplayUserProfile()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                User model = new User();

                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out model);

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_UserProfile.cshtml", GetDropDownList(model));
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DisplayUserProfile(User model)
        {
            try
            {
                UpdateProfile(model);

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_USER_PROFILE_UPDATED_SUCCESS);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult GetUserData(UserViewModel model)
        {
            try
            {
                bool isAdmin = false;

                if (Session["Admin"] != null)
                {
                    isAdmin = (bool)Session["Admin"];
                }
                if (!isAdmin)
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                model = GetUserList(model);

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/UsersListMaster.cshtml", model);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult GetUserDataAjax(UserViewModel model)
        {
            //var context = new SitecoreService("master");
            //var users = context.GetItem<SurveyUsers>(SitecoreItemPaths.IDEALHOMECONSUMERSURVEY_USER);
            //int pageNo = model.PageNo <= 1 ? 1 : model.PageNo;
            //int pageSize = 2;
            //var orderedlist = users.Users.OrderByDescending(x => x.Item.Statistics.Created).ToList();
            //model.ITEMPagedList = orderedlist.ToPagedList(pageNo, pageSize);

            try
            {
                model = GetUserList(model);

            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/_UsersList.cshtml", model);
        }

        public UserViewModel GetUserList(UserViewModel model)
        {
            var context = new SitecoreService("master");
            var users = context.GetItem<SurveyUsers>(SitecoreItemPaths.IDEALHOMECONSUMERSURVEY_USER);
            int pageNo = model.PageNo <= 1 ? 1 : model.PageNo;
            int pageSize = 5;
            var orderedlist = users.Users.OrderByDescending(x => x.Item.Statistics.Created).Where(x => x.IsAdmin == false).ToList();

            model.Total = orderedlist.Count;
            model.ITEMPagedList = orderedlist.ToPagedList(pageNo, pageSize);

            return model;
        }

        #endregion Actions

        #region Helpers

        /// <summary>
        /// Get the List of the vidoes
        /// </summary>
        /// <returns></returns>
        public VideoList GetVideo()
        {
            var _videoList = ContentRepository.GetItem<VideoList>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.IDEALHOMECONSUMER_VIDEOLIST)));
            CacheProvider.Store(CacheKeys.IDEAL_HOME_VIDEO_DETAILS, new AccessCountingCacheItem<VideoList>(_videoList, Times.Exactly(10)));
            return _videoList;
        }

        public VideoGroupList GetVideoGroup()
        {
            var _videoList = ContentRepository.GetItem<VideoGroupList>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.IDEALHOMECONSUMER_VIDEOGROUPLIST)));
            CacheProvider.Store(CacheKeys.IDEAL_HOME_VIDEO_DETAILS, new AccessCountingCacheItem<VideoGroupList>(_videoList, Times.Exactly(10)));
            return _videoList;
        }

        private void SendRegistrationEmail(User model)
        {
            //var sitecoreService = new SitecoreContext();
            var _emailConfig = ContentRepository.GetItem<EmailConfigurations>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.IDEALHOMECONSUMER_EMAIL_REGISTRATION_CONFIG)));

            var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();
            var _currentUser = model;

            string emailBody = _emailConfig.Body;
            string emailCC = _emailConfig.CC;

            emailBody = emailBody.Replace("$UserName", model.FullName).Replace("$User", model.EmailAddress).Replace("$Password", model.Password);

            emailserviceclient.SendEmail(_emailConfig.From, _currentUser.EmailAddress, string.Empty,emailCC, _emailConfig.Subject, emailBody, new List<Tuple<string, byte[]>>());
        }

        private void SendPasswordEmail(User model)
        {
            var _emailConfig = ContentRepository.GetItem<EmailConfigurations>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.IDEALHOMECONSUMER_EMAIL_PASSWORD_CONFIG)));

            var emailserviceclient = DependencyResolver.Current.GetService<IEmailServiceClient>();
            var _currentUser = model;

            string emailBody = _emailConfig.Body;
            var linkemailBody =
               LinkManager.GetItemUrl(Context.Database.GetItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_SETNEWPASSWORD),
                   new ItemUrlBuilderOptions { LanguageEmbedding = LanguageEmbedding.Always, AlwaysIncludeServerUrl = true, AddAspxExtension = false })
                   + "?id=" + HttpUtility.UrlEncode(model.Item.ID.ToString());
            emailBody = emailBody.Replace("$UserName", model.FullName).Replace("$ID", linkemailBody);

            string emailCC = _emailConfig.CC;

            var _response = emailserviceclient.SendEmail(_emailConfig.From, _currentUser.EmailAddress ,string.Empty,emailCC, _emailConfig.Subject, emailBody, new List<Tuple<string, byte[]>>());
        }

        public List<Answers> ParseFormCollection(FormCollection formItems)
        {
            List<Answers> _answers = new List<Answers>();

            foreach (string keys in formItems)
            {
                if (keys.StartsWith("Radio"))
                {
                    string[] _quetionAnswerPair = Request.Form[keys].Split('|');

                    _answers.Add(new Answers { Text = _quetionAnswerPair[0], Value = _quetionAnswerPair[1] });
                }
            }

            return _answers;
        }

        /// <summary>
        /// Called by the Api
        /// </summary>
        /// <returns></returns>
        public bool InsertVideoResponse(string videoItemID)
        {
            User _userModel = new User();
            SurveyResponse _surveyResponseModel = new SurveyResponse();
            VideoList _videoList = new VideoList();
            try
            {
                var sitecoreServiceMaster = GetSitecoreServiceMaster();

                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_SURVEY_DETAILS, out _surveyResponseModel);
                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _userModel);
                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_VIDEO_DETAILS, out _videoList);

                Guid userItemID = _userModel.Item.ID.Guid;

                var _userParent = sitecoreServiceMaster.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

                //find templated id of the item which matched with survey reponse
                string _videoTemplateId = SitecoreItemIdentifiers.IDEALHOMECONSUMER_VIDEO_RESPONSE_TEMPLATE;

                var _videoResponse = _userParent.Item.Children.Where(c => c.TemplateID.ToString().Equals(_videoTemplateId));
                ID _currentresponseItem = _videoResponse.First().ID;

                var _videoTemplate = sitecoreServiceMaster.GetItem<VideoResponse>(new GetItemByIdOptions(Guid.Parse(_currentresponseItem.ToString())) { Language = Language.Parse(Context.Language.Name) });

                Guid _videoItemGuid = new Guid(videoItemID);

                //create the New Section Response
                // _videoTemplate.NameValue = new System.Collections.Specialized.NameValueCollection();
                if (!_videoTemplate.NameValue.AllKeys.Contains(_videoItemGuid.ToString("N")))
                {
                    _videoTemplate.NameValue.Add(_videoItemGuid.ToString("N"), EnumVideoStatus.Watched.ToString());

                    using (new SecurityDisabler())
                    {
                        //Update Survey Response the item back to sitecore
                        sitecoreServiceMaster.SaveItem(_videoTemplate);
                        if (_videoTemplate.Item.Versions.GetVersions(true).Where(x => x.Language.ToString() == "ar-AE").Count() == 0)
                        {
                            CreateLanguageVersion(sitecoreServiceMaster.Database, _videoTemplate.Id, Context.Language.Name);
                        }
                        //publishing item
                        PublishItem(_videoTemplate.Item, true, true, PublishMode.Smart);
                    }
                }
                var watchedvideo = _videoTemplate.NameValue.AllKeys.AsEnumerable().ToList();

                _videoList = GetVideo();
                //var test = _videoList.VideoGallery.Select(x => x.Id.ToString("N")).ToList();
                // _videoList.VideoGallery.Where(x => watchedvideo.Contains(x.Id.ToString("N"))).ToList().ForEach(cc => cc.Watched = true);
                _videoList.completedVideoSection = _videoList.VideoGallery.Select(x => x.Id.ToString("N")).Except(watchedvideo).ToList().Any();

                if (!_videoList.completedVideoSection)
                {
                    _videoTemplate.IsCompleted = true;

                    using (new SecurityDisabler())
                    {
                        //Update Survey Response the item back to sitecore
                        sitecoreServiceMaster.SaveItem(_videoTemplate);

                        //publishing item
                        PublishItem(_videoTemplate.Item, true, true, PublishMode.Smart);
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return _videoList.completedVideoSection;
        }

        private void InsertSurveyResponse(List<Answers> _answers)
        {

            User _UserModel = new User();
            SectionList _SectionListModel = new SectionList();
            SurveyResponse _SurveyResponseModel = new SurveyResponse();
            try
            {
                var sitecoreServiceMaster = GetSitecoreServiceMaster();
                //var sitecoreServiceMaster = GetSitecoreServiceMaster();

                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_SURVEY_DETAILS, out _SurveyResponseModel);
                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _UserModel);
                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_SECTION_DETAILS, out _SectionListModel);

                Guid userItemID = _UserModel.Item.ID.Guid;

                var _user = sitecoreServiceMaster.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

                //find templated id of the item which matched with survey reponse
                //string _surveyResponseTemplate = SitecoreItemIdentifiers.IDEALHOMECONSUMER_SURVEY_RESPONSE_TEMPLATE;
                SurveyResponse _surveyResponse;
                if (_user.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted && _user.VideoResponses.FirstOrDefault().IsCompleted)
                {
                    _surveyResponse = _user.SurveyResponses.LastOrDefault();
                }
                else
                {
                    _surveyResponse = _user.SurveyResponses.FirstOrDefault();
                }

                //create the New Section Response
                var newItem = new SectionResponse();
                var getSection = sitecoreServiceMaster.GetItem<Section>(new GetItemByIdOptions(_SectionListModel.SelectedSection.Id) { Language = Language.Parse("en") });
                newItem.Name = getSection.SectionTitle.Trim();

                //newItem.Responses = new SectionResponse();
                newItem.NameValue = new System.Collections.Specialized.NameValueCollection();

                int _correctResponse = _surveyResponse.Correct.ToInt();
                int _wrongResponse = _surveyResponse.Wrong.ToInt();
                int _marks = _surveyResponse.Marks;

                //var storedAns = GetStoreAnsList(_surveyResponse);

                List<string> allStoreQues = new List<string>();
                foreach (System.Collections.Specialized.NameValueCollection storeDataItem in _surveyResponse.SectionResponses.Select(x => x.NameValue)?.ToList())
                {
                    allStoreQues.AddRange(storeDataItem.AllKeys);
                }
                string existQusItem = null;
                //insert answers to the fields
                foreach (var x in _answers)
                {
                    var ques = sitecoreServiceMaster.GetItem<Questions>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(x.Text.ToString())) { Language = Language.Parse(Context.Language.Name) });
                    var ans = sitecoreServiceMaster.GetItem<Answers>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse(x.Value.ToString())) { Language = Language.Parse(Context.Language.Name) });
                    string submitQus = x.Text.Replace("-", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);
                    if (ques != null)
                    {
                        existQusItem = allStoreQues.Where(xx => xx.Equals(submitQus)).FirstOrDefault();
                        if (ques.CorrectAnswer != null && ques.CorrectAnswer.Id.Equals(Guid.Parse(x.Value)))
                        {
                            _marks += ans.Value.ToInt();
                            _correctResponse = _correctResponse + 1;
                        }
                        else
                        {
                            _wrongResponse = _wrongResponse + 1;
                        }
                    }

                    newItem.NameValue.Add(submitQus, x.Value);
                }

                //mark section as completed
                newItem.IsCompleted = true;
                newItem.SectionID = _SectionListModel.SelectedSection.Item.ID.ToGuid().ToString();

                _surveyResponse.Correct = _correctResponse.ToString();
                _surveyResponse.Wrong = _wrongResponse.ToString();
                _surveyResponse.Marks = _marks;

                // _currentParent.Progress = _SurveyResponseModel.Progress;

                using (new SecurityDisabler())
                {
                    //save the item back to sitecore
                    if (existQusItem != null)
                    {
                        DeleteStoreAns(_surveyResponse, existQusItem);
                    }
                    var serviceResponse = sitecoreServiceMaster.CreateItem(_surveyResponse, newItem);

                    //Update Survey Response the item back to sitecore
                    sitecoreServiceMaster.SaveItem(_surveyResponse);

                    //Update the survey progress
                    _surveyResponse.Progress = Progress(_surveyResponse);

                    //Save the progress
                    sitecoreServiceMaster.SaveItem(_surveyResponse);

                    //Database masterDb = Factory.GetDatabase("master");

                    //creating lang version
                    if (_surveyResponse.Item.Versions.GetVersions(true).Where(x => x.Language.ToString() == "ar-AE").Count() == 0)
                    {
                        CreateLanguageVersion(sitecoreServiceMaster.Database, _surveyResponse.Id, Context.Language.Name);
                    }
                    CreateLanguageVersion(sitecoreServiceMaster.Database, newItem.Id, Context.Language.Name);

                    //publishing item
                    PublishItem(newItem.Item, true, true, PublishMode.Smart);
                    //}
                }
            }
            catch (Exception ex)
            {

                LogService.Info(new Exception("Ideal Home - Exception in InsertSurveyResponse" + ex.Message));
                LogService.Error(ex, this);
            }

        }

        private void UpdateProfile(User model)
        {
            User _UserModel = new User();
            try
            {


                var sitecoreServiceMaster = GetSitecoreServiceMaster();

                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _UserModel);

                Guid userItemID = _UserModel.Item.ID.Guid;

                var updateItem = sitecoreServiceMaster.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

                //specify the properties within the item and assign value
                //newItem.EmailAddress = model.EmailAddress.Trim();
                updateItem.FullName = model.FullName.Trim();
                updateItem.Password = model.Password.Trim();
                updateItem.Mobile = model.Mobile.Trim();
                //updateItem.Title = model.Title.Trim();
                updateItem.Nationality = model.Nationality.Trim();
                updateItem.TypeofResidence = model.TypeofResidence;
                updateItem.NumberofResidence = model.NumberofResidence;
                updateItem.IsDEWACustomer = model.IsDEWACustomer;
                if (!string.IsNullOrEmpty(model.account_number))
                {
                    updateItem.account_number = model.account_number.Trim();
                }
                if (!string.IsNullOrEmpty(model.owner_name))
                {
                    updateItem.owner_name = model.owner_name.Trim();
                }

                using (new SecurityDisabler())
                {
                    sitecoreServiceMaster.SaveItem(updateItem);
                }

                PublishItem(updateItem.Item, false, false, PublishMode.SingleItem);

                var _updatedUser = sitecoreServiceMaster.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

                CacheProvider.Store(CacheKeys.IDEAL_HOME_USER_MODEL, new AccessCountingCacheItem<User>(_updatedUser, Times.Max));
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        private bool CreateUserAccount(User model)
        {
            bool isAccoutCreated = false;
            try
            {

                LogService.Info(new Exception("Ideal Home - Start Create User Account\t" + model.EmailAddress));
                Database masterDb = Factory.GetDatabase("master");

                var sitecoreServiceMaster = GetSitecoreServiceMaster();

                var _parent = sitecoreServiceMaster.GetItem<User>(new GetItemByPathOptions(SitecoreItemPaths.IDEALHOMECONSUMERSURVEY_USER) { Language = Language.Parse("en") });

                //create the new user model
                var newItem = new User();

                //first give the name to the item
                newItem.Name = model.FullName + " " + DateTime.Now.ToString("dMyyhhmm");

                //specify the properties within the item and assign value
                newItem.EmailAddress = model.EmailAddress.Trim();
                newItem.FullName = model.FullName.Trim();

                //model.Password = RandomPassword(8);

                newItem.Password = model.Password.Trim();

                newItem.Mobile = model.Mobile.Trim();
                //newItem.Title = model.Title.Trim();
                newItem.Nationality = model.Nationality.Trim();
                newItem.TypeofResidence = model.TypeofResidence;
                newItem.NumberofResidence = model.NumberofResidence;
                newItem.IsDEWACustomer = model.IsDEWACustomer;
                if (!string.IsNullOrEmpty(model.account_number))
                {
                    newItem.account_number = model.account_number.Trim();
                }
                if (!string.IsNullOrEmpty(model.owner_name))
                {
                    newItem.owner_name = model.owner_name.Trim();
                }

                using (new SecurityDisabler())
                {
                    LogService.Info(new Exception("Ideal Home - Start Creating the item\t" + model.EmailAddress));
                    //save the item back to sitecore
                    var serviceResponse = sitecoreServiceMaster.CreateItem(_parent, newItem);

                    //creating lang version
                    CreateLanguageVersion(masterDb, newItem.Id, Context.Language.Name);
                    //creating lang version

                    LogService.Info(new Exception("Ideal Home - Start Creating the version"));
                    //publishing item

                    CreateLanguageVersion(masterDb, newItem.SurveyResponses.Select(x => x.Id).FirstOrDefault(), Context.Language.Name);
                    //publishing item
                    //PublishItem(newItem.SurveyResponses.Select(x => x.Item).FirstOrDefault(), true, true, PublishMode.Smart);

                    CreateLanguageVersion(masterDb, newItem.SurveyResponses.Select(x => x.Id).LastOrDefault(), Context.Language.Name);
                    ///publishing item
                    // PublishItem(newItem.SurveyResponses.Select(x => x.Item).LastOrDefault(), true, true, PublishMode.Smart);

                    CreateLanguageVersion(masterDb, newItem.VideoResponses.Select(x => x.Id).FirstOrDefault(), Context.Language.Name);
                    //publishing item
                    //PublishItem(newItem.SurveyResponses.Select(x => x.Item).LastOrDefault(), true, true, PublishMode.Smart);
                    LogService.Info(new Exception("Ideal Home - Start Publishing Single Item"));
                    PublishItem(newItem.Item, false, true, PublishMode.SingleItem);
                    LogService.Info(new Exception("Ideal Home - Start sending the mail"));
                    //Send registration email to user
                    SendRegistrationEmail(model);
                    LogService.Info(new Exception("Ideal Home - End Create User Account\t" + model.EmailAddress));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return isAccoutCreated;
        }

        /// <summary>
        /// publish item to web database
        /// </summary>
        /// <param name="item"></param>
        /// <param name="publishParent"></param>
        /// <param name="publishChildren"></param>
        /// <param name="publishMode"></param>
        protected void PublishItem(Item item, bool publishParent = false, bool publishChildren = false, PublishMode publishMode = PublishMode.SingleItem)
        {
            List<Language> _Languages = new List<Language>();
            try
            {
                _Languages.Add(LanguageManager.GetLanguage("ar-AE"));
                _Languages.Add(LanguageManager.GetLanguage("en"));

                foreach (var _lang in _Languages)
                {
                    PublishOptions publishOptions =
                                                 new PublishOptions(item.Database,
                                                           Database.GetDatabase("web"),
                                                           publishMode,
                                                           _lang,
                                                           System.DateTime.Now);  // Create a publisher with the publishoptions
                    Publisher publisher = new Publisher(publishOptions);

                    // Choose where to publish from
                    publisher.Options.RootItem = publishParent ? item.Parent : item;

                    // Publish children as well?
                    publisher.Options.Deep = publishChildren;

                    LogService.Info(new Exception("Ideal Home - End Publishing Smart" + _lang));


                    publisher.Publish();
                    // Do the publish!
                }

                LogService.Info(new Exception("Ideal Home - End Publishing Smart"));
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                LogService.Info(new Exception("Ideal Home - Exception in Publishing" + ex.Message));
            }

            //PublishOptions _enPublishOptions =
            //                    new PublishOptions(item.Database,
            //                              Database.GetDatabase("web"),
            //                              publishMode,
            //                              item.Language,
            //                              System.DateTime.Now);  // Create a publisher with the publishoptions
            //Publisher _enPublisher = new Publisher(_enPublishOptions);

            //// Choose where to publish from
            //_enPublisher.Options.RootItem = publishParent ? item.Parent : item;

            //// Publish children as well?
            //_enPublisher.Options.Deep = publishChildren;

            //// Do the publish!
            //_enPublisher.Publish();
        }

        public ISitecoreService GetSitecoreService()
        {
            return new SitecoreService(Sitecorex.Context.Database.Name.ToString());
        }

        public ISitecoreService GetSitecoreServiceMaster()
        {
            return new SitecoreService(Sitecore.Configuration.Factory.GetDatabase("master"));
        }

        private bool CheckContractAccount(User model, out string error)
        {
            LogService.Info(new Exception("Ideal Home - Start Checking Contract Account"));

            bool isValid = false;
            error = null;

            var response = DewaApiClient.GetBill(model.account_number, RequestLanguage, Request.Segment());
            if (response.Succeeded)
            {
                isValid = true;
            }
            else
            {
                isValid = false;
                error = response.Message;
            }

            LogService.Info(new Exception("Ideal Home - End Checking Contract Account"));

            return isValid;
        }

        protected void CreateLanguageVersion(Database db, Guid itemId, string language)
        {
            if (db == null || itemId == null || string.IsNullOrWhiteSpace(language))
                return;

            Item migratedItem = null;

            switch (language)
            {
                case "en":
                    migratedItem = db.GetItem(new ID(itemId), LanguageManager.GetLanguage("ar-AE"));
                    migratedItem.Versions.AddVersion();
                    break;

                case "ar-AE":
                    migratedItem = db.GetItem(new ID(itemId), LanguageManager.GetLanguage("en"));
                    migratedItem.Versions.AddVersion();
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// validate emailaddress to get password
        /// </summary>
        /// <param name="model"></param>
        /// <param name="error"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool TryLogin(User model, out string error, out User refModel, bool resetpwd = false)
        {
            error = null;
            refModel = new User();

            LogService.Info(new Exception("Ideal Home - Start Checking Existing Account"));

            var _surveyUsers = ContentRepository.GetItem<SurveyUsers>(new GetItemByPathOptions(SitecoreItemPaths.IDEALHOMECONSUMERSURVEY_USER) { Language = Language.Parse(Context.Language.Name) });

            var currentUser =
                _surveyUsers.Users.FirstOrDefault(
                    c =>
                        c.EmailAddress.ToLower() == model.EmailAddress.ToLower());

            LogService.Info(new Exception("Ideal Home - End Checking Existing Account"));

            if (currentUser != null)
            {
                refModel.Password = currentUser.Password;
                refModel.FullName = currentUser.FullName;
                refModel.Item = currentUser.Item;
                if (resetpwd)
                {
                    currentUser.ResetPasswordDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                    using (new SecurityDisabler())
                    {
                        GetSitecoreServiceMaster().SaveItem(currentUser);
                    }

                    PublishItem(currentUser.Item, false, false, PublishMode.SingleItem);
                }

                return true;
            }
            error = Translate.Text(DictionaryKeys.IdealHomeConsumer.InvalidAccount);
            return false;
        }

        private bool TryLogin(User model, out string error, out bool isAdmin)
        {
            error = null;
            isAdmin = false;

            var sitecoreService = GetSitecoreService();

            //var _surveyUsers = ContentRepository.GetItem<SurveyUsers>(new GetItemByPathOptions(SitecoreItemPaths.IDEALHOMECONSUMERSURVEY_USER) { Language = Language.Parse(Context.Language.Name) });
            var _surveyUsers = sitecoreService.GetItems<User>(new GetItemsByQueryOptions(new Query(SitecoreItemPaths.IDEALHOMECONSUMERSURVEY_USER + "/*")));
            var currentUser =
                _surveyUsers.FirstOrDefault(
                    c =>
                        c.EmailAddress.ToLower() == model.EmailAddress.ToLower() &&
                        string.Equals(c.Password, model.Password.Trim(), StringComparison.InvariantCulture));
            if (currentUser != null)
            {
                if (currentUser.IsAdmin)
                {
                    isAdmin = true;
                    Session["Admin"] = isAdmin;
                    Session["FullName"] = currentUser.FullName;
                }
                else
                {
                    isAdmin = false;

                    CacheProvider.Store(CacheKeys.IDEAL_HOME_USER_MODEL, new AccessCountingCacheItem<User>(currentUser, Times.Max));
                    AuthStateService.Save(new DewaProfile(currentUser.FullName, string.Empty, Roles.IdealHome)
                    {
                        IsContactUpdated = true,
                        Name = currentUser.FullName,
                        EmailAddress = currentUser.EmailAddress
                    }
                        );
                }

                return true;
            }
            error = Translate.Text(DictionaryKeys.IdealHomeConsumer.InvalidAccount);
            return false;
        }


        private User GetDropDownList(User model)
        {
            var titleTypes = ContentRepository.GetItem<ListDataSources>(new GetItemByPathOptions(DataSources.TITLELIST)).Items;
            var titleItems = titleTypes.Select(c => new SelectListItem { Text = c.Text, Value = c.Value });
            model.TitleList = titleItems.ToList();

            return model;
        }


        private SurveyResponse DisplayScoreStats()
        {
            User _UserModel = new User();
            var sitecoreService = GetSitecoreService();

            CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _UserModel);

            Guid userItemID = _UserModel.Item.ID.Guid;

            var _user = sitecoreService.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

            SurveyResponse _surveyResponse = null;

            if (_user.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted && _user.VideoResponses.FirstOrDefault().IsCompleted)
            {
                _surveyResponse = _user.SurveyResponses.LastOrDefault();
                _surveyResponse.Assessment = Translate.Text("J106.FinalAssessmentScore");
            }
            else
            {
                _surveyResponse = _user.SurveyResponses.FirstOrDefault();
                _surveyResponse.Assessment = Translate.Text("J106.FirstAssessmentScore");
            }

            return _surveyResponse;
        }

        private User DisplayDashBoard()
        {
            User _UserModel = new User();
            var sitecoreService = GetSitecoreService();

            CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _UserModel);

            Guid userItemID = _UserModel.Item.ID.Guid;

            var _user = sitecoreService.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

            return _user;
        }


        private int Progress(SurveyResponse _surveyResponse)
        {
            var sitecoreServiceMaster = GetSitecoreServiceMaster();

            var _sectionList = sitecoreServiceMaster.GetItem<SectionList>(SitecoreItemIdentifiers.IDEALHOMECONSUMER_SECTIONLIST);

            SurveyResponse _updatedsurveyResponse = sitecoreServiceMaster.GetItem<SurveyResponse>(_surveyResponse.Id.ToString());

            IEnumerable<string> sectionExists = _updatedsurveyResponse.SectionResponses.Select(x => x.SectionID);

            var _existingSectionList = _sectionList.Sections.Select(x => x.Id.ToString());
            var _nonSelectedSection = _existingSectionList.Except(sectionExists).ToList();

            decimal selectSectionCount = System.Convert.ToDecimal(sectionExists.Count());

            decimal totalSectionCount = System.Convert.ToDecimal(_existingSectionList.Count());
            decimal total = selectSectionCount / totalSectionCount;
            var Percentage = Math.Round(total * 100);

            return _updatedsurveyResponse.Progress = System.Convert.ToInt32(Percentage);
        }

        //public bool IsvalidAnswer(string id)
        //{
        //    User _UserModel = new User();
        //    var sitecoreService = GetSitecoreService();

        //    CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _UserModel);

        //    Guid userItemID = _UserModel.Item.ID.Guid;

        //    var _user = sitecoreService.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

        //    //var ques = sitecoreService.GetItem<Questions>(Guid.Parse(x.Text.ToString()), Language.Parse(Context.Language.Name));
        //    //var ans = sitecoreService.GetItem<Answers>(Guid.Parse(x.Value.ToString()), Language.Parse(Context.Language.Name));

        //    var _surveyResponse = _user.SurveyResponses.FirstOrDefault();
        //    IEnumerable<string> _sections = _surveyResponse.SectionResponses.Select(x => x.SectionID);

        //    return false;
        //}

        #endregion Helpers

        #region Phase 2



        #region IdealHomeDashboard

        [HttpGet]
        public ActionResult FirstAssessmentV1()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                var _surveyResponseModel = DisplayDashBoard();//DisplayFirstAssesment();

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_FirstAssessment.cshtml", _surveyResponseModel);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult VideoV1()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                // Initial Step
                User _userModel = new User();
                var sitecoreService = GetSitecoreService();
                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out _userModel);
                Guid userItemID = _userModel.Item.ID.Guid;

                var _userParent = sitecoreService.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });
                if (!_userParent.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted)
                {
                    return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_VideoInitial.cshtml");
                }

                //Display Video by group category

                //find templated id of the item which matched with survey reponse
                string _videoTemplateId = SitecoreItemIdentifiers.IDEALHOMECONSUMER_VIDEO_RESPONSE_TEMPLATE;
                var _videoResponse = _userParent.Item.Children.Where(c => c.TemplateID.ToString().Equals(_videoTemplateId));
                ID _currentresponseItem = _videoResponse.First().ID;

                var _videoTemplate = sitecoreService.GetItem<VideoResponse>(new GetItemByIdOptions(Guid.Parse(_currentresponseItem.ToString())) { Language = Language.Parse(Context.Language.Name) });
                var watchedvideo = _videoTemplate.NameValue.AllKeys.AsEnumerable().ToList();

                var _videoGroupList = GetVideoGroup();

                _videoGroupList?.VideoCategoryList?.ToList()?.ForEach(x => x.VideoGallery?.Where(xx => watchedvideo.Contains(xx.Id.ToString("N"))).ToList().ForEach(cc => cc.Watched = true));
                //_videoGroupList.IsCompletedVideoList = _videoGroupList?.VideoCategoryList?.ToList()?.ForEach(y => y.VideoGallery.Select(yy => y.Id.ToString("N")).Except(watchedvideo).ToList().Any());

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_Video.cshtml", _videoGroupList);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult FinalAssessmentV1()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                var _surveyResponseModel = DisplayDashBoard();

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_FinalAssessment.cshtml", _surveyResponseModel);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult CertificateV1()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                var _surveyResponseModel = DisplayDashBoard();

                //var sitecoreService = new SitecoreContext();
                var _htmlConfig = ContentRepository.GetItem<HtmlTemplateConfigurations>(new GetItemByIdOptions(Guid.Parse(SitecoreItemIdentifiers.IDEALHOMECONSUMER_HTML_TEMPLATE_CONFIG)));

                if (_htmlConfig.HtmlImage != null)
                {
                    ViewBag.HtmlTemplate = _htmlConfig.HtmlImage.Src;
                    ViewBag.HtmlEmail = _htmlConfig.HtmlEmail;
                }
                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_Certificate.cshtml", _surveyResponseModel);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CertificateV1(User model, string fileName, string ImagePath)
        {
            try
            {
                string _path = string.Empty;
                if (Sitecorex.Context.Language.Name == "ar-AE")
                {
                    _path = Server.MapPath("/images/idealhome/certificate_ar.jpg");
                }
                else
                {
                    _path = Server.MapPath("/images/idealhome/certificate_en.jpg");
                }
                return RedirectToAction("DownloadPDF", new { fileName = fileName, imgPath = _path });
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        #endregion IdealHomeDashboard

        [HttpGet]
        public ActionResult DisplaySubHeaderV1()
        {
            try
            {
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                var _userModel = DisplayDashBoard();

                var _videoGroupList = GetVideoGroup();
                _userModel.Videos = new VideoList();

                _userModel.Videos.VideoGallery = _videoGroupList.VideoCategoryList.SelectMany(x => x.VideoGallery).Distinct();

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_SubHeader.cshtml", _userModel);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        /// <summary>
        /// Display the the Survey from CMS
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult DisplaySurveyV1(string id = "", int pre = 0)
        {
            int _currentStep = 0;
            try
            {
                //to search from master DB and later publish webdb
                var sitecoreService = GetSitecoreService();

                //check loggedin or not
                if (!IsLoggedIn || !CurrentPrincipal.Role.Equals(Roles.IdealHome))
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
                }

                var _sectionList = sitecoreService.GetItem<SectionList>(SitecoreItemIdentifiers.IDEALHOMECONSUMER_SECTIONLIST);
                User model = new User();
                SectionList _SectionListModel = new SectionList();

                CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out model);

                Guid userItemID = model.Item.ID.Guid;

                var _user = sitecoreService.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });

                SurveyResponse _surveyResponse = null;

                if (_user.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted && _user.VideoResponses.FirstOrDefault().IsCompleted)
                {
                    _surveyResponse = _user.SurveyResponses.LastOrDefault();
                }
                else
                {
                    _surveyResponse = _user.SurveyResponses.FirstOrDefault();
                }

                //SurveyResponse _surveyResponse = _user.SurveyResponses.FirstOrDefault();
                IEnumerable<string> sectionExists = _surveyResponse.SectionResponses.Select(x => x.SectionID);

                var _existingSectionList = _sectionList.Sections.Select(x => x.Id.ToString());
                var _existingSectionListcount = _existingSectionList.ToList().Count;
                var _nonSelectedSection = _existingSectionList.Except(sectionExists).ToList();

                #region [stored answer]

                _sectionList.StoredAnsList = GetStoreAnsList(_surveyResponse);

                #endregion [stored answer]

                if (pre > 0 && sectionExists != null && sectionExists.Count() > 0)
                {
                    _currentStep = pre - 1;
                    string _preSectionItem = System.Convert.ToString(_existingSectionList?.ToList()[_currentStep]);
                    _sectionList.SelectedSection = sitecoreService.GetItem<Section>(new GetItemByIdOptions(Guid.Parse(_preSectionItem)) { Language = Language.Parse(Context.Language.Name) });

                    //SectionResponse
                }

                #region first attempt

                if (_sectionList.SelectedSection == null)
                {
                    //Calculate Progress of the survey
                    decimal selectSectionCount = System.Convert.ToDecimal(sectionExists.Count());

                    if (selectSectionCount == 0)
                    {
                        selectSectionCount = 1;
                    }

                    CacheProvider.Store(CacheKeys.IDEAL_HOME_SURVEY_DETAILS, new AccessCountingCacheItem<SurveyResponse>(_surveyResponse, Times.Max));

                    if (_nonSelectedSection != null && _nonSelectedSection.Any())
                    {
                        string _nid = _nonSelectedSection.FirstOrDefault();
                        _currentStep = _existingSectionList.ToList().IndexOf(_nid);
                        _sectionList.SelectedSection = sitecoreService.GetItem<Section>(new GetItemByIdOptions(Guid.Parse(_nid)) { Language = Language.Parse(Context.Language.Name) });
                    }
                    else
                    {
                        var sitecoreServicemaster = GetSitecoreServiceMaster();
                        if (_user.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted && _user.VideoResponses.FirstOrDefault().IsCompleted && !_surveyResponse.IsSecondAttemptCompleted)
                        {
                            var _usermaster = sitecoreServicemaster.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });
                            _usermaster.SurveyResponses.LastOrDefault().IsFirstAttemptCompleted = true;
                            _usermaster.SurveyResponses.LastOrDefault().IsSecondAttemptCompleted = true;
                            using (new SecurityDisabler())
                            {
                                //save the item back to sitecore
                                sitecoreServicemaster.SaveItem(_usermaster.SurveyResponses.LastOrDefault());

                                //publishing item
                                PublishItem(_usermaster.SurveyResponses.LastOrDefault().Item, true, true, PublishMode.Smart);
                            }
                        }
                        else if (!_user.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted)
                        {
                            var _usermaster = sitecoreServicemaster.GetItem<User>(new GetItemByIdOptions(userItemID) { Language = Language.Parse(Context.Language.Name) });
                            _usermaster.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted = true;
                            using (new SecurityDisabler())
                            {
                                //save the item back to sitecore
                                sitecoreServicemaster.SaveItem(_usermaster.SurveyResponses.FirstOrDefault());

                                //publishing item
                                PublishItem(_usermaster.SurveyResponses.FirstOrDefault().Item, true, true, PublishMode.Smart);
                            }
                        }
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_DASHBOARD);
                    }
                }

                //_sectionList.CurrentStep = (_existingSectionListcount + 1) - _nonSelectedSection.ToList().Count;
                _sectionList.CurrentStep = _currentStep;
                _sectionList.TotalStep = _existingSectionList.ToList().Count;
                _sectionList.isProgress = _surveyResponse.Progress;
                _sectionList.IsStepComplete = _user.SurveyResponses.FirstOrDefault().IsFirstAttemptCompleted && _user.VideoResponses.FirstOrDefault().IsCompleted;
                CacheProvider.Store(CacheKeys.IDEAL_HOME_SECTION_DETAILS, new AccessCountingCacheItem<SectionList>(_sectionList, Times.Max));

                return PartialView("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/_DisplaySurvey.cshtml", _sectionList);

                #endregion first attempt
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        private List<string> GetStoreAnsList(SurveyResponse surveyResponse)
        {
            List<string> StoredAnsList = new List<string>();
            foreach (System.Collections.Specialized.NameValueCollection item in surveyResponse.SectionResponses.Select(x => x.NameValue))
            {
                foreach (string itemKey in item.AllKeys)
                {
                    StoredAnsList.Add(System.Convert.ToString(item.GetValues(itemKey).LastOrDefault()));
                }
            }

            return StoredAnsList;
        }

        private void DeleteStoreAns(SurveyResponse surveyResponse, string toDeleteQusId)
        {
            foreach (SectionResponse item in surveyResponse.SectionResponses)
            {
                if (item.NameValue.AllKeys.Contains(toDeleteQusId))
                {
                    item.Item.Delete();
                }
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DisplaySurveyV1(SectionList model, FormCollection formItems)
        {
            try
            {
                //Inserts the Response to Sitecore.
                InsertSurveyResponse(ParseFormCollection(formItems));
                if (model.isSaveExist == "isSaveExist")
                {
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_DASHBOARD);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_SURVEY);
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.IDEALHOMECONSUMER_LOGIN);
        }

        [HttpGet]
        public ActionResult OffersCarousel()
        {
            if (!string.IsNullOrWhiteSpace(RenderingContext.Current.Rendering.DataSource) && !DewastoreglobalConfiguration.ServiceOutage())
            {
                OfferConfig offerConfigdatasource = RenderingRepository.GetDataSourceItem<OfferConfig>();
                if (offerConfigdatasource != null && offerConfigdatasource.ModuleParameter != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleParameter.Text))
                {
                    User model = new User();
                    if (CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out model))
                    {
                        offerConfigdatasource.account_number = model.account_number;
                        offerConfigdatasource.IsDewastoreAllowed = (model.SurveyResponses.FirstOrDefault().Marks == 100 || (model.SurveyResponses.LastOrDefault().IsSecondAttemptCompleted && model.SurveyResponses.LastOrDefault().Marks >= 90));
                    }

                    if (offerConfigdatasource.ModuleParameter.Text.Equals("carousel"))
                    {
                        return View("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_OffersCarousel.cshtml", offerConfigdatasource);
                    }
                    //else if (offerConfigdatasource.ModuleParameter.Text.Equals("teaser"))
                    //{
                    //    return View("OffersTeaser", offerConfigdatasource);
                    //}
                }
            }

            return new ViewResult();
        }

        [HttpGet]
        public ActionResult GuestOffersCarousel()
        {
            if (!string.IsNullOrWhiteSpace(RenderingContext.Current.Rendering.DataSource) && !DewastoreglobalConfiguration.ServiceOutage())
            {
                OfferConfig offerConfigdatasource = RenderingRepository.GetDataSourceItem<OfferConfig>();
                if (offerConfigdatasource != null && offerConfigdatasource.ModuleParameter != null && !string.IsNullOrWhiteSpace(offerConfigdatasource.ModuleParameter.Text))
                {
                    User model = new User();
                    if (CacheProvider.TryGet(CacheKeys.IDEAL_HOME_USER_MODEL, out model))
                    {
                        offerConfigdatasource.account_number = model.account_number;
                        offerConfigdatasource.IsDewastoreAllowed = (model.SurveyResponses.FirstOrDefault().Marks == 100 || (model.SurveyResponses.LastOrDefault().IsSecondAttemptCompleted && model.SurveyResponses.LastOrDefault().Marks >= 90));
                    }

                    if (offerConfigdatasource.ModuleParameter.Text.Equals("carousel"))
                    {
                        return View("~/Views/Feature/IdealHome/IdealHomeConsumer/V1/Module/_GuestOffersCarousel.cshtml", offerConfigdatasource);
                    }
                    //else if (offerConfigdatasource.ModuleParameter.Text.Equals("teaser"))
                    //{
                    //    return View("OffersTeaser", offerConfigdatasource);
                    //}
                }
            }

            return new ViewResult();
        }

        #endregion Phase 2
    }
}