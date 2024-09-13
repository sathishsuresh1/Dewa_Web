namespace DEWAXP.Feature.DRRG.Controllers
{
    using DEWAXP.Foundation.Logger;
    using DEWAXP.Foundation.CustomDB.DRRGDataModel;
    using DEWAXP.Foundation.CustomDB.DataModel.CustomDataType.DRRG;
    using DEWAXP.Feature.DRRG.Models;
    using Sitecore.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using System.Data.Entity.Core.Objects;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Feature.DRRG.Filters.Mvc;
    using DEWAXP.Feature.DRRG.Extensions;
    using System.Globalization;
    using DEWAXP.Foundation.Content.Repositories;
    using Sitecore.Mvc.Presentation;
    using System.Threading;
    using System.Data;
    using Exception = System.Exception;
    using EntityFrameworkExtras.EF6;
    using DEWAXP.Foundation.Content.Filters.Mvc;
    using Sitecorex = global::Sitecore;
    using System.Data.Common;
    using System.Data.Entity.Infrastructure;
    using global::Sitecore.Data.Items;
    using Org.BouncyCastle.Ocsp;

    [OnlyCM]
    public class DRRGEvaluatorController : DRRGBaseController
    {
        public long FileSizeLimit = 2048000;
        public string[] supportedTypes = new[] { ".jpg", ".png", ".jpeg", ".pdf", ".doc", ".docx", ".PDF", ".PNG", ".JPG", ".JPEG", ".DOCX", ".DOC" };
        public string[] supportedLogoTypes = new[] { ".jpg", ".png", ".jpeg", ".PNG", ".JPG", ".JPEG" };

        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (IsLoggedIn && (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator) || CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager)))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG__EVALUATOR_Dashboard);
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                ClearCookiesSignOut();
                ViewBag.ReturnUrl = returnUrl;
            }
            return View("~/Views/Feature/DRRG/Evaluator/Login.cshtml");
        }
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl = null)
        {
            try
            {
                if (model != null && !string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password) && model.Username.Trim().EndsWith("@dewa.gov.ae"))
                {
                    Item item = Sitecorex.Context.Database.GetItem(SitecoreItemIdentifiers.DRRG_CONFIG);
                    string mgr = item["Schema Manager"];
                    List<string> lstmgr = mgr.Split(';').ToList();
                    bool group13 = lstmgr.Any(a => a.ToLower().Equals(model.Username.ToLower()));
                    if (group13)
                    {
                        if (fnValidateUser(model.Username, model.Password, model.Username, Roles.DRRGSchemaManager))
                        {
                            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG__EVALUATOR_Dashboard);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, Translate.Text("InvalidCredential_ErrorMessage"));
                        }
                    }
                    else
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            var evResult = context.DRRG_EvaluatorLogin.Where(x => x.loginid == model.Username).FirstOrDefault();
                            if (!string.IsNullOrWhiteSpace(model.Username) && evResult != null)
                            {
                                if (fnValidateUser(model.Username, model.Password, evResult.Reference_ID, Roles.DRRGEvaulator))
                                {
                                    if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                                    {
                                        return Redirect(returnUrl);
                                    }
                                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DRRG__EVALUATOR_Dashboard);
                                }
                                else
                                {
                                    ModelState.AddModelError(string.Empty, Translate.Text("InvalidCredential_ErrorMessage"));
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(model.Username))
                            {
                                //LogService.Error(new Exception(model.Username), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.Noteliglibleuser);
                            }
                            else
                            {
                                //LogService.Error(new Exception(model.Username), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/Login.cshtml");
        }

        [HttpGet]
        public ActionResult Registration()
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
            return View("~/Views/Feature/DRRG/Evaluator/Registration.cshtml", new EvaluatorRegistrationModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Registration(EvaluatorRegistrationModel model)
        {
            bool status = false;
            try
            {
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    status = ReCaptchaHelper.RecaptchaResponse(Request["g-recaptcha-response"]);
                }

                if (status)
                {
                    if (model != null && !string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Name) && model.Username.Trim().EndsWith("@dewa.gov.ae"))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.responseMessage, typeof(string));
                            string evacode = EvaluatorCode();
                            context.SP_DRRG_EvalutorRegistration(model.Username.Trim(), model.Name, evacode, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                ViewBag.Referenceid = evacode;
                                SendDRRGModuleEmail(model.Name, model.Username, evacode, DRRGStandardValues.EvaluatorRegistration);
                                return View("~/Views/Feature/DRRG/Evaluator/RegistrationSuccess.cshtml");
                            }
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.UserExists))
                            {
                                LogService.Error(new Exception(myString), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.Useremailalreadyexist);
                            }
                            else
                            {
                                LogService.Error(new Exception(myString), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, DRRGERRORCODE.Noteliglibleuser);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ErrorMessages.UNEXPECTED_ERROR);
                }
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/Registration.cshtml", model);
        }

        [HttpGet, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager)]
        public ActionResult Dashboard()
        {
            EvaluatorDashboardViewModel model = new EvaluatorDashboardViewModel { Name = CurrentPrincipal.Name, Role = CurrentPrincipal.Role };
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                    {
                        model.RoleName = Translate.Text("Evaluator");
                        ObjectParameter ltsubmittedParamresponse = new ObjectParameter(DRRGStandardValues.ltsubmitted, typeof(int));
                        ObjectParameter ltupdatedParamresponse = new ObjectParameter(DRRGStandardValues.ltupdated, typeof(int));
                        ObjectParameter ltrejectedParamresponse = new ObjectParameter(DRRGStandardValues.ltrejected, typeof(int));
                        ObjectParameter equipmentcountParamresponse = new ObjectParameter(DRRGStandardValues.equipmentcount, typeof(int));
                        var result = context.SP_DRRG_DashboardCount(ltsubmittedParamresponse, ltupdatedParamresponse, ltrejectedParamresponse, equipmentcountParamresponse);
                        model.Submittedcount = Convert.ToInt32(ltsubmittedParamresponse.Value);
                        model.Updatedcount = Convert.ToInt32(ltupdatedParamresponse.Value);
                        model.Rejectedcount = Convert.ToInt32(ltrejectedParamresponse.Value);
                        model.Equipmentcount = Convert.ToInt32(equipmentcountParamresponse.Value);
                    }
                    else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                    {
                        model.RoleName = Translate.Text("Scheme Manager");
                        model.Updatedcount = context.DRRG_EvaluatorLogin.Where(x => x.Status.Equals("Submitted")).Count();
                        int pvCount = context.DRRG_PVMODULE.Where(x => x.Status.Equals("ReviewerApproved")).Count();
                        int invCount = context.DRRG_InverterModule.Where(x => x.Status.Equals("ReviewerApproved")).Count();
                        int interfaceCount = context.DRRG_InterfaceModule.Where(x => x.Status.Equals("ReviewerApproved")).Count();
                        model.Equipmentcount = (pvCount + invCount + interfaceCount);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/Dashboard.cshtml", model);
        }
        [HttpGet, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator)]
        public ActionResult ManufacturerList()
        {
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    int index = 0;
                    List<SP_DRRG_GETSubmittedManufacturer_Result> result = new List<SP_DRRG_GETSubmittedManufacturer_Result>();
                    string status = DRRGStandardValues.AuthorizedLetterSubmitted;
                    ViewBag.manuref = 1;
                    if (RenderingContext.Current.Rendering.Parameters["updatedprofile"] != null && RenderingContext.Current.Rendering.Parameters["updatedprofile"] == "1")
                    {
                        ViewBag.manuref = 2;
                        status = DRRGStandardValues.AuthorizedLetterUpdated;
                        result = context.SP_DRRG_GETSubmittedManufacturer(status).OrderByDescending(x => x.CreatedDate).ToList();
                    }
                    else if (RenderingContext.Current.Rendering.Parameters["rejectedapplications"] != null && RenderingContext.Current.Rendering.Parameters["rejectedapplications"] == "1")
                    {
                        ViewBag.manuref = 3;
                        status = DRRGStandardValues.SchemaManagerRejected;
                        result = context.SP_DRRG_GETSubmittedManufacturer(status).OrderByDescending(x => x.CreatedDate).ToList();
                    }
                    else
                    {
                        result = context.SP_DRRG_GETSubmittedManufacturer(status).OrderByDescending(x => x.CreatedDate).ToList();
                    }
                    result.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                    {
                        modelName = x.Manufacturer_Name,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Local_Representative ? (x.User_First_Name + " " + x.User_Last_Name) : x.Company_Full_Name,
                        type = x.Local_Representative ? Translate.Text("DRRG.AuthorizedLocalRepresentative.Label") : Translate.Text("DRRG.Manufacturer.Label"),
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        serialnumber = (Interlocked.Increment(ref index)).ToString()
                    }));

                    //Lstmodule = Lstmodule.OrderByDescending(x => x.datedtSubmitted).ToList();
                    CacheProvider.Store(CacheKeys.DRRG_SUBMITTED_MANUFACTURERLIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/ManufacturerList.cshtml");
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator)]
        public ActionResult ManufacturerListAjax(int pagesize = 5, string keyword = "", string statustxt = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG_SUBMITTED_MANUFACTURERLIST, out Lstmodule))
                {
                    FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGModules.strdataindex = "0";
                    if (Lstmodule != null && Lstmodule.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstmodule = Lstmodule.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.representative) && x.representative.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGModules.namesort = namesort;
                        filteredDRRGModules.totalpage = Pager.CalculateTotalPages(Lstmodule.Count(), pagesize);
                        filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                        filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGModules.lastitem = page * pagesize < Lstmodule.Count() ? page * pagesize : Lstmodule.Count();
                        filteredDRRGModules.totalitem = Lstmodule.Count();
                        filteredDRRGModules.Lstmodule = Lstmodule.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager), HttpGet]
        public ActionResult GetDRRGManufacturerDetails(string id, string strstatus = "")
        {
            var itemLock = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var applicatioSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == id).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (applicatioSession != null && applicatioSession.UserId != CurrentPrincipal.UserId)
                        {
                            var sessionTime = context.DRRG_UserSession.Where(x => x.userid == applicatioSession.UserId).OrderByDescending(x => x.fromtime).FirstOrDefault();
                            if (sessionTime != null && applicatioSession.UserId != CurrentPrincipal.UserId && DateTime.Now >= sessionTime.fromtime && DateTime.Now <= sessionTime.totime)
                            {
                                itemLock = true;
                            }
                            else
                            {
                                context.DRRG_ApplicationSession.Add(new DRRG_ApplicationSession
                                {
                                    ReferenceId = id,
                                    SessionId = CurrentPrincipal.SessionToken,
                                    UserId = CurrentPrincipal.UserId,
                                    CreatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture)
                                });
                                context.SaveChanges();
                            }
                        }
                        else if (applicatioSession == null)
                        {
                            context.DRRG_ApplicationSession.Add(new DRRG_ApplicationSession
                            {
                                ReferenceId = id,
                                SessionId = CurrentPrincipal.SessionToken,
                                UserId = CurrentPrincipal.UserId,
                                CreatedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), CultureInfo.InvariantCulture)
                            });
                            context.SaveChanges();
                        }

                        if (id.ToLower().StartsWith("manufacturer") || id.ToLower().StartsWith("manuf"))
                        {
                            ViewBag.manuref = 1;
                            string status = DRRGStandardValues.AuthorizedLetterSubmitted;
                            if (RenderingContext.Current.Rendering.Parameters["updatedprofile"] != null && RenderingContext.Current.Rendering.Parameters["updatedprofile"] == "1")
                            {
                                ViewBag.manuref = 2;
                                status = DRRGStandardValues.AuthorizedLetterUpdated;
                            }
                            else if (RenderingContext.Current.Rendering.Parameters["rejectedapplications"] != null && RenderingContext.Current.Rendering.Parameters["rejectedapplications"] == "1")
                            {
                                ViewBag.manuref = 3;
                                status = DRRGStandardValues.SchemaManagerRejected;
                            }
                            if (!string.IsNullOrWhiteSpace(strstatus))
                                status = strstatus;
                            //var manuresult = context.SP_DRRG_GETManufacturerbyID(id, status).ToList();
                            var manuresult = context.DRRG_Manufacturer_Details.Where(x => x.Manufacturer_Code == id && x.Status == status).ToList();
                            if (manuresult != null && manuresult.Count() > 0)
                            {
                                var factresult = context.SP_DRRG_GETFactorybyID(id).ToList();
                                if (factresult != null && factresult.Count() > 0)
                                {
                                    var pvresultfiles = context.SP_DRRG_GETFilesbyIDAdmin(id).ToList();
                                    var rejectedcomments = context.SP_DRRG_GETManuRejectedComments(id).ToList();
                                    var rejectedfiles = context.SP_DRRG_GETManuRejectedFilesbyIDAdmin(id).ToList();
                                    var result = GetManufacturerDetail(id, manuresult, factresult, pvresultfiles, rejectedcomments, rejectedfiles);
                                    result.UserRole = CurrentPrincipal.Role;
                                    result.itemLock = itemLock;
                                    return View("~/Views/Feature/DRRG/Evaluator/Details.cshtml", result);
                                }
                            }
                            else
                            {
                                LogService.Error(new Exception(id), this);
                                ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                            }
                        }
                        if (id.ToLower().StartsWith("pv"))
                        {
                            //var pvresult = context.SP_DRRG_GETFilteredPVModulebyID(DRRGStandardValues.ADMIN, id, string.Empty).ToList();
                            var pvresult = context.DRRG_PVMODULE.Where(x => x.PV_ID == id).ToList();

                            var pvfiltered = pvresult.Where(y => y != null);
                            if (pvfiltered != null && pvfiltered.ToList() != null && pvfiltered.Count() > 0)
                            {
                                ViewBag.manuref = 4;
                                var pvresultfiles = context.SP_DRRG_GETFilesbyIDAdmin(id).ToList();
                                var result = GetPVModuleDetail(id, pvresult, pvresultfiles: pvresultfiles);
                                result.UserRole = CurrentPrincipal.Role;
                                result.itemLock = itemLock;
                                return View("~/Views/Feature/DRRG/Evaluator/Details.cshtml", result);
                            }
                        }
                        else if (id.ToLower().StartsWith("ip"))
                        {
                            //var ipresult = context.SP_DRRG_GETFilteredInterfaceModulebyID(DRRGStandardValues.ADMIN, id, string.Empty).ToList();
                            var ipresult = context.DRRG_InterfaceModule.Where(x => x.Interface_ID == id).ToList();
                            var ipfiltered = ipresult.Where(y => y != null);
                            if (ipfiltered != null && ipfiltered.ToList() != null && ipfiltered.Count() > 0)
                            {
                                ViewBag.manuref = 4;
                                var pvresultfiles = context.SP_DRRG_GETFilesbyIDAdmin(id).ToList();
                                var result = GetInterfaceModuleDetail(id, ipresult, pvresultfiles: pvresultfiles);
                                result.UserRole = CurrentPrincipal.Role;
                                result.itemLock = itemLock;
                                return View("~/Views/Feature/DRRG/Evaluator/Details.cshtml", result);
                            }
                        }
                        else if (id.ToLower().StartsWith("inv"))
                        {
                            //var ivresult = context.SP_DRRG_GETFilteredInverterModulebyID(DRRGStandardValues.ADMIN, id, string.Empty).ToList();
                            var ivresult = context.DRRG_InverterModule.Where(x => x.Inverter_ID == id).ToList();
                            var ivfiltered = ivresult.Where(y => y != null);
                            if (ivfiltered != null && ivfiltered.ToList() != null && ivfiltered.Count() > 0)
                            {
                                ViewBag.manuref = 4;
                                var pvresultfiles = context.SP_DRRG_GETFilesbyIDAdmin(id).ToList();
                                var result = GetInverterModuleDetail(id, ivresult, pvresultfiles: pvresultfiles);
                                result.UserRole = CurrentPrincipal.Role;
                                result.itemLock = itemLock;
                                return View("~/Views/Feature/DRRG/Evaluator/Details.cshtml", result);
                            }
                        }
                    }

                }
                else
                {
                    ModelState.AddModelError(string.Empty, DRRGERRORCODE.CheckLink);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Dashboard/Details.cshtml");
        }

        [HttpGet, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager)]
        public ActionResult Attachment(string id)
        {
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var pvresultfiles = context.SP_DRRG_GETFilesbyfileIDAdmin(id).ToList();
                    if (pvresultfiles != null && pvresultfiles.Count > 0)
                    {
                        byte[] bytes = pvresultfiles.FirstOrDefault().Content;
                        string type = pvresultfiles.FirstOrDefault().ContentType;
                        return File(bytes, type);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return null;
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator)]
        public ActionResult closeApplication(string regnumber)
        {
            string message = string.Empty;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                    if (applicationSession != null)
                    {
                        foreach (var item in applicationSession)
                        {
                            context.DRRG_ApplicationSession.Remove(item);
                            context.SaveChanges();
                        }
                        message = "success";
                        return Json(new { status = true, Message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager)]
        public ActionResult ApproveRegistration(string regnumber, string saltmethod = null)
        {
            string message = string.Empty;
            try
            {
                #region [Manufacturer]
                if (regnumber.ToLower().StartsWith("manufacturer") || regnumber.ToLower().StartsWith("manuf"))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                        ObjectParameter manufacturernameParamresponse = new ObjectParameter(DRRGStandardValues.manufacturername, typeof(string));
                        ObjectParameter manufactureremailParamresponse = new ObjectParameter(DRRGStandardValues.manufactureremail, typeof(string));
                        var result = context.SP_DRRG_ManufacturerApproval(CurrentPrincipal.Username, regnumber, manufacturernameParamresponse, manufactureremailParamresponse, myOutputParamresponse);
                        string myString = Convert.ToString(myOutputParamresponse.Value);
                        string manuname = Convert.ToString(manufacturernameParamresponse.Value);
                        string manuemail = Convert.ToString(manufactureremailParamresponse.Value);
                        if (!string.IsNullOrWhiteSpace(myString) && !string.IsNullOrWhiteSpace(manuname) && !string.IsNullOrWhiteSpace(manuemail) && myString.Equals(DRRGStandardValues.Success))
                        {
                            DRRGExtensions.Logger(manuname, "Approved by " + CurrentPrincipal.Name, "manufacturer", regnumber, DateTime.Now, CurrentPrincipal.UserId, "Approved");
                            SendDRRGModuleEmail(manuname, manuemail, regnumber, DRRGStandardValues.Registrationapproved);
                            return Json(new { status = true, Message = Translate.Text("New User Registration Success Message").Replace("{refno}", regnumber) }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            message = GetDRRGErrormessage(myString);
                        }
                    }
                }
                #endregion

                #region [PV]
                if (regnumber.ToLower().StartsWith("pv"))
                {
                    #region [Evaulator Approval]
                    if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                            var pvDetails = context.DRRG_PVMODULE.Where(x => x.PV_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                            var result = context.SP_DRRG_PVModuleApproval(CurrentPrincipal.Username, regnumber, !string.IsNullOrWhiteSpace(saltmethod) ? saltmethod : pvDetails.Salt_Mist_Test_Method, myOutputParamresponse);

                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                if (applicationSession != null)
                                {
                                    foreach (var item in applicationSession)
                                    {
                                        context.DRRG_ApplicationSession.Remove(item);
                                        context.SaveChanges();
                                    }
                                }
                                DRRGExtensions.Logger(pvDetails != null ? pvDetails.Model_Name : "PV", "Approved by evaluator - " + CurrentPrincipal.Name, "PV Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "ReviewerApproved");
                                return Json(new { status = true, Message = Translate.Text("The PV Module Registration Application (Ref." + regnumber + ") has been approved successfully. It is now pending final approval of the Scheme Manager.") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = GetDRRGErrormessage(myString);
                            }
                        }
                    }
                    #endregion

                    #region [Scheme manager Approval]
                    else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                            var pvDetails = context.DRRG_PVMODULE.Where(x => x.PV_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                            var result = context.SP_DRRG_SchemaMgr_PVModuleApproval(CurrentPrincipal.Username, regnumber, !string.IsNullOrWhiteSpace(saltmethod) ? saltmethod : pvDetails.Salt_Mist_Test_Method, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                DRRGExtensions.Logger(pvDetails != null ? pvDetails.Model_Name : "PV", "Approved by scheme manager - " + CurrentPrincipal.Name, "PV Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "Approved");
                                var userName = context.DRRG_UserLogin.Where(x => x.Login_username.Equals(pvDetails.Userid)).FirstOrDefault();
                                SendDRRGApproveEmail(userName.Name, pvDetails.Userid, pvDetails.PV_ID, pvDetails.Model_Name, DRRGStandardValues.PVModuleApproved);
                                return Json(new { status = true, Message = Translate.Text("The PV Module Registration Application (Ref." + regnumber + ") has been approved successfully.") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = GetDRRGErrormessage(myString);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region [IP]
                if (regnumber.ToLower().StartsWith("ip"))
                {
                    #region [Evaulator Approval]
                    if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                            var result = context.SP_DRRG_PVInterfaceApproval(CurrentPrincipal.Username, regnumber, saltmethod, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                if (applicationSession != null)
                                {
                                    foreach (var item in applicationSession)
                                    {
                                        context.DRRG_ApplicationSession.Remove(item);
                                        context.SaveChanges();
                                    }
                                }
                                var ipvDetails = context.DRRG_InterfaceModule.Where(x => x.Interface_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                DRRGExtensions.Logger(ipvDetails != null ? ipvDetails.Model_Name : "IP", "Approved by evaulator - " + CurrentPrincipal.Name, "Interface Protection Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "ReviewerApproved");
                                return Json(new { status = true, Message = Translate.Text("The Interface Protection Registration Application (Ref." + regnumber + ") has been approved successfully. It is now pending final approval of the Scheme Manager.") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = GetDRRGErrormessage(myString);
                            }
                        }
                    }
                    #endregion

                    #region [Scheme manager Approval]
                    else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                            var result = context.SP_DRRG_SchemaMgr_PVInterfaceApproval(CurrentPrincipal.Username, regnumber, saltmethod, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                var ipvDetails = context.DRRG_InterfaceModule.Where(x => x.Interface_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                DRRGExtensions.Logger(ipvDetails != null ? ipvDetails.Model_Name : "IP", "Approved by scheme manager - " + CurrentPrincipal.Name, "Interface Protection Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "Approved");
                                var userName = context.DRRG_UserLogin.Where(x => x.Login_username.Equals(ipvDetails.Userid)).FirstOrDefault();
                                SendDRRGApproveEmail(userName.Name, ipvDetails.Userid, ipvDetails.Interface_ID, ipvDetails.Model_Name, DRRGStandardValues.IPModuleApproved);
                                return Json(new { status = true, Message = Translate.Text("The Interface Protection Registration Application (Ref." + regnumber + ") has been approved successfully.") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = GetDRRGErrormessage(myString);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region [IV]
                if (regnumber.ToLower().StartsWith("inv") || regnumber.ToLower().StartsWith("iv"))
                {
                    #region [Evaulator Approval]
                    if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                            var result = context.SP_DRRG_PVInverterApproval(CurrentPrincipal.Username, regnumber, saltmethod, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                if (applicationSession != null)
                                {
                                    foreach (var item in applicationSession)
                                    {
                                        context.DRRG_ApplicationSession.Remove(item);
                                        context.SaveChanges();
                                    }
                                }
                                var ivDetails = context.DRRG_InverterModule.Where(x => x.Inverter_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                DRRGExtensions.Logger(ivDetails != null ? ivDetails.Model_Name : "IV", "Approved by evaulator - " + CurrentPrincipal.Name, "Inverter Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "ReviewerApproved");
                                return Json(new { status = true, Message = Translate.Text("The PV Inverter Registration Application (Ref." + regnumber + ") has been approved successfully. It is now pending final approval of the Scheme Manager.") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = GetDRRGErrormessage(myString);
                            }
                        }
                    }
                    #endregion

                    #region [scheme manager Approval]
                    else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                    {
                        using (DRRGEntities context = new DRRGEntities())
                        {
                            ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                            var result = context.SP_DRRG_SchemaMgr_PVInverterApproval(CurrentPrincipal.Username, regnumber, saltmethod, myOutputParamresponse);
                            string myString = Convert.ToString(myOutputParamresponse.Value);
                            if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                            {
                                var ivDetails = context.DRRG_InverterModule.Where(x => x.Inverter_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                DRRGExtensions.Logger(ivDetails != null ? ivDetails.Model_Name : "IV", "Approved by scheme manager - " + CurrentPrincipal.Name, "Inverter Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "Approved");
                                var userName = context.DRRG_UserLogin.Where(x => x.Login_username.Equals(ivDetails.Userid)).FirstOrDefault();
                                SendDRRGApproveEmail(userName.Name, ivDetails.Userid, ivDetails.Inverter_ID, ivDetails.Model_Name, DRRGStandardValues.IVModuleApproved);
                                return Json(new { status = true, Message = Translate.Text("The PV Inverter Registration Application (Ref." + regnumber + ") has been approved successfully.") }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                message = GetDRRGErrormessage(myString);
                            }
                        }
                    }
                    #endregion

                }
                #endregion
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager)]
        public ActionResult RejectionRegistration(RejectedViewModel model)
        {
            string error = string.Empty;
            string message = string.Empty;
            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            try
            {
                var rejectedcode = RejectedCode();

                #region [Manufacturer]
                if (!string.IsNullOrWhiteSpace(model.regnumber) && (model.regnumber.ToLower().StartsWith("manufacturer") || model.regnumber.ToLower().StartsWith("manuf")))
                {
                    error = Addfile(model.file, FileType.RejectedFile, FileEntity.Manufacturer, rejectedcode, error, FileSizeLimit, supportedTypes, model.regnumber, dRRG_Files_TYlist);
                }
                #endregion

                #region [PV Module]
                if (!string.IsNullOrWhiteSpace(model.regnumber) && model.regnumber.ToLower().StartsWith("pv"))
                {
                    error = Addfile(model.file, FileType.RejectedFile, FileEntity.PVmodule, rejectedcode, error, FileSizeLimit, supportedTypes, model.regnumber, dRRG_Files_TYlist);
                }
                #endregion

                #region[IV Module]
                if (!string.IsNullOrWhiteSpace(model.regnumber) && (model.regnumber.ToLower().StartsWith("inv") || model.regnumber.ToLower().StartsWith("iv")))
                {
                    error = Addfile(model.file, FileType.RejectedFile, FileEntity.Invertermodule, rejectedcode, error, FileSizeLimit, supportedTypes, model.regnumber, dRRG_Files_TYlist);
                }
                #endregion

                #region[IP Module]
                if (!string.IsNullOrWhiteSpace(model.regnumber) && model.regnumber.ToLower().StartsWith("ip"))
                {
                    error = Addfile(model.file, FileType.RejectedFile, FileEntity.Interfacemodule, rejectedcode, error, FileSizeLimit, supportedTypes, model.regnumber, dRRG_Files_TYlist);
                }
                #endregion

                if (ModelState.IsValid)
                {
                    try
                    {
                        #region [Manufacturer]
                        if (!string.IsNullOrWhiteSpace(model.regnumber) && model.regnumber.ToLower().StartsWith("manuf"))
                        {
                            Proc_DRRG_RejectionModule procedure = new Proc_DRRG_RejectionModule()
                            {
                                useremail = CurrentPrincipal.Username,
                                manufacturecode = model.regnumber,
                                rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                dRRG_Files_TY = dRRG_Files_TYlist
                            };
                            using (DRRGEntities context = new DRRGEntities())
                            {
                                context.Database.ExecuteStoredProcedure(procedure);
                                string errormessage = procedure.error;
                                string manuname = procedure.manufacturername;
                                string manuemail = procedure.manufactureremail;
                                if (!string.IsNullOrWhiteSpace(procedure.error) && !string.IsNullOrWhiteSpace(manuname) && !string.IsNullOrWhiteSpace(manuemail) && procedure.error.Equals(DRRGStandardValues.Success))
                                {
                                    var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == model.regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                    if (applicationSession != null)
                                    {
                                        foreach (var item in applicationSession)
                                        {
                                            context.DRRG_ApplicationSession.Remove(item);
                                            context.SaveChanges();
                                        }
                                    }
                                    List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                    if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                    {
                                        dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                    }
                                    DRRGExtensions.Logger(model.modelname, "", "manufacturer", model.regnumber, DateTime.Now, CurrentPrincipal.UserId, "ReviewerRejected");
                                    SendDRRGRejectionEmail(manuname, manuemail, model.regnumber, !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty, string.Empty, DRRGStandardValues.ManufacturerRejected, attList);
                                    return Json(new { status = true, Message = Translate.Text("The New User Registration has been rejected successfully.") }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    LogService.Error(new Exception(procedure.error), this);
                                    message = GetDRRGErrormessage(procedure.error);
                                }
                            }
                        }
                        #endregion

                        #region [PV Module]
                        if (!string.IsNullOrWhiteSpace(model.regnumber) && model.regnumber.ToLower().StartsWith("pv"))
                        {
                            #region [Evaulator Rejection]
                            if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                            {
                                Proc_DRRG_PV_RejectionModule procedure = new Proc_DRRG_PV_RejectionModule()
                                {
                                    useremail = CurrentPrincipal.Username,
                                    pvcode = model.regnumber,
                                    rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                    rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                    dRRG_Files_TY = dRRG_Files_TYlist
                                };
                                using (DRRGEntities context = new DRRGEntities())
                                {
                                    context.Database.ExecuteStoredProcedure(procedure);
                                    string errormessage = procedure.error;
                                    string pvusername = procedure.pvusername;
                                    string pvuseremail = procedure.pvuseremail;
                                    string pvmodelname = procedure.pvmodelname;
                                    if (!string.IsNullOrWhiteSpace(procedure.error) && procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == model.regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                        if (applicationSession != null)
                                        {
                                            foreach (var item in applicationSession)
                                            {
                                                context.DRRG_ApplicationSession.Remove(item);
                                                context.SaveChanges();
                                            }
                                        }
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                        if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                        {
                                            dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                        }
                                        var pvDetails = context.DRRG_PVMODULE.Where(x => x.PV_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();
                                        //DRRGExtensions.Logger(model.modelname, "", "PV Module", model.regnumber, DateTime.Now, CurrentPrincipal.UserId, "ReviewerRejected");
                                        DRRGExtensions.Logger(pvDetails != null ? pvDetails.Model_Name : "PV", "Rejected by evaulator - " + CurrentPrincipal.Name, "PV Module", model.regnumber, DateTime.Now, CurrentPrincipal.Username, "ReviewerRejected");
                                        SendDRRGRejectionEmail(pvusername, pvuseremail, model.regnumber, model.rejectionreason, pvmodelname, DRRGStandardValues.PVModuleRejected, attList);
                                        return Json(new { status = true, Message = Translate.Text("The PV Module Registration Application (Ref.{refnumber}) has been rejected successfully.").Replace("{refnumber}", model.regnumber) }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            #endregion

                            #region [scheme manager Rejection]
                            else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                            {
                                Proc_DRRG_PV_Mgr_RejectionModule schema_mgr_procedure = new Proc_DRRG_PV_Mgr_RejectionModule()
                                {
                                    useremail = CurrentPrincipal.Username,
                                    pvcode = model.regnumber,
                                    rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                    rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                    dRRG_Files_TY = dRRG_Files_TYlist
                                };
                                using (DRRGEntities context = new DRRGEntities())
                                {
                                    context.Database.ExecuteStoredProcedure(schema_mgr_procedure);
                                    string errormessage = schema_mgr_procedure.error;
                                    string pvusername = schema_mgr_procedure.pvusername;
                                    string pvuseremail = schema_mgr_procedure.pvuseremail;
                                    string pvmodelname = schema_mgr_procedure.pvmodelname;
                                    if (!string.IsNullOrWhiteSpace(schema_mgr_procedure.error) && schema_mgr_procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                        if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                        {
                                            dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                        }
                                        //DRRGExtensions.Logger(model.modelname, "", "PV Module", model.regnumber, DateTime.Now, CurrentPrincipal.UserId, "Rejected");
                                        var pvDetails = context.DRRG_PVMODULE.Where(x => x.PV_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();

                                        DRRGExtensions.Logger(pvDetails != null ? pvDetails.Model_Name : "PV", "Rejected by scheme manager - " + CurrentPrincipal.Name, "PV Module", model.regnumber, DateTime.Now, CurrentPrincipal.Username, "Rejected");
                                        //SendDRRGRejectionEmail(pvusername, pvuseremail, model.regnumber, model.rejectionreason, pvmodelname, DRRGStandardValues.PVModuleRejected, attList);
                                        return Json(new { status = true, Message = Translate.Text("The PV Module Registration Application (Ref.{refnumber}) has been rejected successfully.").Replace("{refnumber}", model.regnumber) }, JsonRequestBehavior.AllowGet);
                                    }
                                }

                            }
                            #endregion
                        }
                        #endregion

                        #region [IV Module]
                        if (!string.IsNullOrWhiteSpace(model.regnumber) && model.regnumber.ToLower().StartsWith("inv"))
                        {
                            #region [Evaulator Rejection]
                            if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                            {
                                Proc_DRRG_IV_RejectionModule procedure = new Proc_DRRG_IV_RejectionModule()
                                {
                                    useremail = CurrentPrincipal.Username,
                                    pvcode = model.regnumber,
                                    rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                    rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                    dRRG_Files_TY = dRRG_Files_TYlist
                                };
                                using (DRRGEntities context = new DRRGEntities())
                                {
                                    context.Database.ExecuteStoredProcedure(procedure);
                                    string errormessage = procedure.error;
                                    string pvusername = procedure.pvusername;
                                    string pvuseremail = procedure.pvuseremail;
                                    string pvmodelname = procedure.pvmodelname;
                                    if (!string.IsNullOrWhiteSpace(procedure.error) && procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == model.regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                        if (applicationSession != null)
                                        {
                                            foreach (var item in applicationSession)
                                            {
                                                context.DRRG_ApplicationSession.Remove(item);
                                                context.SaveChanges();
                                            }
                                        }
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                        if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                        {
                                            dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                        }
                                        var ivDetails = context.DRRG_InverterModule.Where(x => x.Inverter_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();
                                        DRRGExtensions.Logger(ivDetails != null ? ivDetails.Model_Name : "IV", "Rejected by evaulator - " + CurrentPrincipal.Name, "Inverter Module", model.regnumber, DateTime.Now, CurrentPrincipal.Username, "ReviewerRejected");
                                        //DRRGExtensions.Logger(model.modelname, "", "Inverter Module", model.regnumber, DateTime.Now, CurrentPrincipal.UserId, "ReviewerRejected");
                                        SendDRRGRejectionEmail(pvusername, pvuseremail, model.regnumber, !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty, pvmodelname, DRRGStandardValues.IVModuleRejected, attList);
                                        return Json(new { status = true, Message = Translate.Text("The PV Inverter Registration Application (Ref.{refnumber}) has been rejected successfully.").Replace("{refnumber}", model.regnumber) }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            #endregion

                            #region [Scheme manager Rejection]
                            else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                            {
                                Proc_DRRG_IV_Mgr_RejectionModule schema_mgr_procedure = new Proc_DRRG_IV_Mgr_RejectionModule()
                                {
                                    useremail = CurrentPrincipal.Username,
                                    pvcode = model.regnumber,
                                    rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                    rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                    dRRG_Files_TY = dRRG_Files_TYlist
                                };
                                using (DRRGEntities context = new DRRGEntities())
                                {
                                    context.Database.ExecuteStoredProcedure(schema_mgr_procedure);
                                    string errormessage = schema_mgr_procedure.error;
                                    string pvusername = schema_mgr_procedure.pvusername;
                                    string pvuseremail = schema_mgr_procedure.pvuseremail;
                                    string pvmodelname = schema_mgr_procedure.pvmodelname;
                                    if (!string.IsNullOrWhiteSpace(schema_mgr_procedure.error) && schema_mgr_procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                        if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                        {
                                            dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                        }
                                        var ivDetails = context.DRRG_InverterModule.Where(x => x.Inverter_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();
                                        DRRGExtensions.Logger(ivDetails != null ? ivDetails.Model_Name : "IV", "Rejected by scheme manager - " + CurrentPrincipal.Name, "Inverter Module", model.regnumber, DateTime.Now, CurrentPrincipal.Username, "Rejected");
                                        //SendDRRGRejectionEmail(pvusername, pvuseremail, model.regnumber, model.rejectionreason, pvmodelname, DRRGStandardValues.IVModuleRejected, attList);
                                        return Json(new { status = true, Message = Translate.Text("The PV Inverter Registration Application (Ref.{refnumber}) has been rejected successfully.").Replace("{refnumber}", model.regnumber) }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion

                        #region [IP Module]
                        if (!string.IsNullOrWhiteSpace(model.regnumber) && model.regnumber.ToLower().StartsWith("ip"))
                        {
                            #region [Evaulator Rejection]
                            if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                            {
                                Proc_DRRG_IP_RejectionModule procedure = new Proc_DRRG_IP_RejectionModule()
                                {
                                    useremail = CurrentPrincipal.Username,
                                    pvcode = model.regnumber,
                                    rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                    rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                    dRRG_Files_TY = dRRG_Files_TYlist
                                };
                                using (DRRGEntities context = new DRRGEntities())
                                {
                                    context.Database.ExecuteStoredProcedure(procedure);
                                    string errormessage = procedure.error;
                                    string pvusername = procedure.pvusername;
                                    string pvuseremail = procedure.pvuseremail;
                                    string pvmodelname = procedure.pvmodelname;
                                    if (!string.IsNullOrWhiteSpace(procedure.error) && procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        var applicationSession = context.DRRG_ApplicationSession.Where(x => x.ReferenceId == model.regnumber && x.UserId == CurrentPrincipal.UserId).ToList();
                                        if (applicationSession != null)
                                        {
                                            foreach (var item in applicationSession)
                                            {
                                                context.DRRG_ApplicationSession.Remove(item);
                                                context.SaveChanges();
                                            }
                                        }
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                        if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                        {
                                            dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                        }
                                        var ipDetails = context.DRRG_InterfaceModule.Where(x => x.Interface_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();
                                        DRRGExtensions.Logger(ipDetails != null ? ipDetails.Model_Name : "IP", "Rejected by evaulator - " + CurrentPrincipal.Name, "Interface Protection Module", model.regnumber, DateTime.Now, CurrentPrincipal.Username, "ReviewerRejected");
                                        //DRRGExtensions.Logger(model.modelname, "", "Interface Protection Module", model.regnumber, DateTime.Now, CurrentPrincipal.UserId, "ReviewerRejected");
                                        SendDRRGRejectionEmail(pvusername, pvuseremail, model.regnumber, !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty, pvmodelname, DRRGStandardValues.IPModuleRejected, attList);
                                        return Json(new { status = true, Message = Translate.Text("The Interface Protection Registration Application (Ref.{refnumber}) has been rejected successfully.").Replace("{refnumber}", model.regnumber) }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            #endregion

                            #region [Scheme manager Rejection]
                            else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                            {
                                Proc_DRRG_IP_Mgr_RejectionModule schema_mgr_procedure = new Proc_DRRG_IP_Mgr_RejectionModule()
                                {
                                    useremail = CurrentPrincipal.Username,
                                    pvcode = model.regnumber,
                                    rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                    rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                    dRRG_Files_TY = dRRG_Files_TYlist
                                };
                                using (DRRGEntities context = new DRRGEntities())
                                {
                                    context.Database.ExecuteStoredProcedure(schema_mgr_procedure);
                                    string errormessage = schema_mgr_procedure.error;
                                    string pvusername = schema_mgr_procedure.pvusername;
                                    string pvuseremail = schema_mgr_procedure.pvuseremail;
                                    string pvmodelname = schema_mgr_procedure.pvmodelname;
                                    if (!string.IsNullOrWhiteSpace(schema_mgr_procedure.error) && schema_mgr_procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();
                                        if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                        {
                                            dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                        }
                                        var ipDetails = context.DRRG_InterfaceModule.Where(x => x.Interface_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();

                                        DRRGExtensions.Logger(ipDetails != null ? ipDetails.Model_Name : "IP", "Rejected by scheme manager - " + CurrentPrincipal.Name, "Interface Protection Module", model.regnumber, DateTime.Now, CurrentPrincipal.Username, "Rejected");
                                        //SendDRRGRejectionEmail(pvusername, pvuseremail, model.regnumber, model.rejectionreason, pvmodelname, DRRGStandardValues.IPModuleRejected, attList);
                                        return Json(new { status = true, Message = Translate.Text("The Interface Protection Registration Application (Ref.{refnumber}) has been rejected successfully.").Replace("{refnumber}", model.regnumber) }, JsonRequestBehavior.AllowGet);
                                    }
                                }
                            }
                            #endregion
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex, this);
                        message = ErrorMessages.UNEXPECTED_ERROR;
                    }
                }
                else
                {
                    message = error;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator), HttpGet]
        public ActionResult RejectedApplication()
        {
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var result = context.SP_DRRG_GetRejectedModulesBySchemaManager().ToList();
                    result.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                    {
                        modelName = x.modelname,
                        status = x.Status,
                        statusText = GetViewStatus(x.Status),
                        remarks = GetRemarks(x.Status, x.referenceid),
                        referenceNumber = x.referenceid,
                        type = x.model,
                        datedtSubmitted = x.CreatedDate,
                        updatedDate = x.UpdatedDate,
                        manufacturerName = x.manuname,
                        EvaluatedBy = GetEvaluator(x.Reviewed_By),
                        applicationtype = Translate.Text(x.model),
                        dateSubmitted = x.CreatedDate.ToString(),
                    }));

                    CacheProvider.Store(CacheKeys.DRRG_MODULELIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/RejectedApplication.cshtml");
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator)]
        public ActionResult ApplicationStatusAjax(int pagesize = 5, string keyword = "", string applicationtype = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG_MODULELIST, out Lstmodule))
                {
                    FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGModules.strdataindex = "0";
                    if (Lstmodule != null && Lstmodule.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(applicationtype))
                        {
                            if (applicationtype.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.PVmodule.ToString())).ToList();
                            }
                            else if (applicationtype.Equals("2"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Invertermodule.ToString())).ToList();
                            }
                            else if (applicationtype.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Interfacemodule.ToString())).ToList();
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstmodule = Lstmodule.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.manufacturerName) && x.manufacturerName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.EvaluatedBy) && x.EvaluatedBy.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.type) && x.type.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGModules.namesort = namesort;
                        filteredDRRGModules.totalpage = Pager.CalculateTotalPages(Lstmodule.Count(), pagesize);
                        filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                        filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGModules.lastitem = page * pagesize < Lstmodule.Count() ? page * pagesize : Lstmodule.Count();
                        filteredDRRGModules.totalitem = Lstmodule.Count();
                        int index = (page - 1) * pagesize;
                        filteredDRRGModules.Lstmodule = Lstmodule.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        filteredDRRGModules.Lstmodule.ForEach(x => x.serialnumber = (Interlocked.Increment(ref index)).ToString());
                        return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager), HttpGet]
        public ActionResult EquipmentList()
        {
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        bool filterStatus = false;
                        if (RenderingContext.Current.Rendering.Parameters["rejectedapplications"] != null && RenderingContext.Current.Rendering.Parameters["rejectedapplications"] == "1")
                        {
                            filterStatus = true;
                            ViewBag.equipref = 1;
                        }
                        var result = context.SP_DRRG_GETModulesByAdmin().ToList();

                        result.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                        {
                            modelName = x.modelname,
                            status = x.Status,
                            statusText = GetViewStatus(x.Status),
                            remarks = "",
                            referenceNumber = x.referenceid,
                            type = x.model,
                            applicationtype = Translate.Text(x.model),
                            manufacturerName = x.manuname,
                            datedtSubmitted = x.CreatedDate,
                            dateSubmitted = x.CreatedDate.ToString(),
                        }));

                        if (filterStatus)
                            Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("rejected")).ToList();
                        else
                            Lstmodule = Lstmodule.Where(x => !x.status.ToLower().Equals("rejected")).ToList();

                        Lstmodule = Lstmodule.OrderByDescending(x => x.datedtSubmitted).ToList();
                        CacheProvider.Store(CacheKeys.DRRG__EVALUATOR_MODULELIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                    }
                }
                else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var result = context.SP_DRRG_GETModulesBySchemaManager().ToList();

                        result.Where(y => y != null).ForEach((x) => Lstmodule.Add(new ModuleItem
                        {
                            modelName = x.modelname,
                            status = x.Status,
                            statusText = GetViewStatus(x.Status),
                            remarks = "",
                            referenceNumber = x.referenceid,
                            type = x.model,
                            applicationtype = Translate.Text(x.model),
                            manufacturerName = x.manuname,
                            datedtSubmitted = x.CreatedDate,
                            dateSubmitted = x.CreatedDate.ToString(),
                        }));

                        PVModule pVModule = new PVModule();
                        List<SelectListItem> selectListItems = new List<SelectListItem>();
                        ViewBag.pvmodules = Lstmodule.Where(x => x.type == "PVMODULE" && x.status.Equals("ReviewerApproved")).Count();
                        ViewBag.ivmodules = Lstmodule.Where(x => x.type == "INVERTERMODULE" && x.status.Equals("ReviewerApproved")).Count();
                        ViewBag.ipmodules = Lstmodule.Where(x => x.type == "INTERFACEMODULE" && x.status.Equals("ReviewerApproved")).Count();

                        ViewBag.pvmodules_updated = Lstmodule.Where(x => x.type == "PVMODULE" && x.status == "Updated").Count();
                        ViewBag.ivmodules_updated = Lstmodule.Where(x => x.type == "INVERTERMODULE" && x.status == "Updated").Count();
                        ViewBag.ipmodules_updated = Lstmodule.Where(x => x.type == "INTERFACEMODULE" && x.status == "Updated").Count();

                        Lstmodule = Lstmodule.Where(x => x.status == "ReviewerApproved").OrderByDescending(x => x.datedtSubmitted).ToList();

                        CacheProvider.Store(CacheKeys.DRRG__SCHEMAMGR_MODULELIST, new CacheItem<List<ModuleItem>>(Lstmodule, TimeSpan.FromMinutes(40)));
                        return View("~/Views/Feature/DRRG/Evaluator/SchemaMgrEquipmentList.cshtml");
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/EquipmentList.cshtml");
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager)]
        public ActionResult EquipmentListAjax(int pagesize = 5, string keyword = "", string applicationtype = "", string statustxt = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG__EVALUATOR_MODULELIST, out Lstmodule))
                {
                    FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGModules.strdataindex = "0";
                    if (Lstmodule != null && Lstmodule.Count > 0)
                    {
                        if (CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
                        {
                            if (!string.IsNullOrWhiteSpace(statustxt) && statustxt.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("submitted") || x.status.ToLower().Equals("rejected")).ToList();
                            }
                            else if (!string.IsNullOrWhiteSpace(statustxt) && statustxt.Equals("2"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("updated")).ToList();
                            }
                            else if (!string.IsNullOrWhiteSpace(statustxt) && statustxt.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("rejected")).ToList();
                            }
                        }
                        else if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                        {
                            if (!string.IsNullOrWhiteSpace(statustxt) && statustxt.Equals("1"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("approved")).ToList();
                            }
                            else if (!string.IsNullOrWhiteSpace(statustxt) && statustxt.Equals("2"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("reviewerapproved")).ToList();
                            }
                            else if (!string.IsNullOrWhiteSpace(statustxt) && statustxt.Equals("3"))
                            {
                                Lstmodule = Lstmodule.Where(x => x.status.ToLower().Equals("reviewerrejected")).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(applicationtype) && applicationtype.Equals("1"))
                        {
                            Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.PVmodule.ToString())).ToList();
                        }
                        else if (!string.IsNullOrWhiteSpace(applicationtype) && applicationtype.Equals("2"))
                        {
                            Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Invertermodule.ToString())).ToList();
                        }
                        else if (!string.IsNullOrWhiteSpace(applicationtype) && applicationtype.Equals("3"))
                        {
                            Lstmodule = Lstmodule.Where(x => x.type.Equals(FileEntity.Interfacemodule.ToString())).ToList();
                        }
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstmodule = Lstmodule.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.type) && x.type.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.manufacturerName) && x.manufacturerName.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGModules.namesort = namesort;
                        filteredDRRGModules.totalpage = Pager.CalculateTotalPages(Lstmodule.Count(), pagesize);
                        filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                        filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGModules.lastitem = page * pagesize < Lstmodule.Count() ? page * pagesize : Lstmodule.Count();
                        filteredDRRGModules.totalitem = Lstmodule.Count();
                        int index = (page - 1) * pagesize;
                        filteredDRRGModules.Lstmodule = Lstmodule.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        filteredDRRGModules.Lstmodule.ForEach(x => x.serialnumber = (Interlocked.Increment(ref index)).ToString());
                        return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager), ValidateAntiForgeryToken]
        public ActionResult ApproveMultipleRegistration(string refIds, string saltmethod = null)
        {
            string message = string.Empty;
            try
            {
                if (refIds != null)
                {
                    string[] refValues = refIds.Split(',');

                    using (DRRGEntities context = new DRRGEntities())
                    {
                        foreach (var regnumber in refValues)
                        {
                            #region [PV]
                            if (regnumber.ToLower().StartsWith("pv"))
                            {
                                #region [Scheme manager Approval]
                                if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                                {
                                    ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                                    var pvDetails = context.DRRG_PVMODULE.Where(x => x.PV_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                    var result = context.SP_DRRG_SchemaMgr_PVModuleApproval(CurrentPrincipal.Username, regnumber, !string.IsNullOrWhiteSpace(saltmethod) ? saltmethod : pvDetails.Salt_Mist_Test_Method, myOutputParamresponse);
                                    string myString = Convert.ToString(myOutputParamresponse.Value);
                                    if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                                    {
                                        DRRGExtensions.Logger(pvDetails != null ? pvDetails.Model_Name : "PV", "Approved by scheme manager - " + CurrentPrincipal.Name, "PV Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "Approved");
                                        var userName = context.DRRG_UserLogin.Where(x => x.Login_username.Equals(pvDetails.Userid)).FirstOrDefault();
                                        SendDRRGApproveEmail(userName.Name, pvDetails.Userid, pvDetails.PV_ID, pvDetails.Model_Name, DRRGStandardValues.PVModuleApproved);
                                    }
                                    else
                                    {
                                        message = GetDRRGErrormessage(myString);
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region [IP]
                            if (regnumber.ToLower().StartsWith("ip"))
                            {
                                #region [Scheme manager Approval]
                                if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                                {
                                    ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                                    var result = context.SP_DRRG_SchemaMgr_PVInterfaceApproval(CurrentPrincipal.Username, regnumber, saltmethod, myOutputParamresponse);
                                    string myString = Convert.ToString(myOutputParamresponse.Value);
                                    if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                                    {
                                        var ipvDetails = context.DRRG_InterfaceModule.Where(x => x.Interface_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                        DRRGExtensions.Logger(ipvDetails != null ? ipvDetails.Model_Name : "IP", "Approved by scheme manager - " + CurrentPrincipal.Name, "Interface Protection Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "Approved");
                                        var userName = context.DRRG_UserLogin.Where(x => x.Login_username.Equals(ipvDetails.Userid)).FirstOrDefault();
                                        SendDRRGApproveEmail(userName.Name, ipvDetails.Userid, ipvDetails.Interface_ID, ipvDetails.Model_Name, DRRGStandardValues.IPModuleApproved);
                                    }
                                    else
                                    {
                                        message = GetDRRGErrormessage(myString);
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            #region [IV]
                            if (regnumber.ToLower().StartsWith("inv"))
                            {
                                #region [Scheme manager Approval]
                                if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                                {
                                    ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                                    var result = context.SP_DRRG_SchemaMgr_PVInverterApproval(CurrentPrincipal.Username, regnumber, saltmethod, myOutputParamresponse);
                                    string myString = Convert.ToString(myOutputParamresponse.Value);
                                    if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                                    {
                                        var ivDetails = context.DRRG_InverterModule.Where(x => x.Inverter_ID.ToLower().Equals(regnumber.ToLower())).FirstOrDefault();
                                        DRRGExtensions.Logger(ivDetails != null ? ivDetails.Model_Name : "IV", "Approved by scheme manager - " + CurrentPrincipal.Name, "Inverter Module", regnumber, DateTime.Now, CurrentPrincipal.Username, "Approved");
                                        var userName = context.DRRG_UserLogin.Where(x => x.Login_username.Equals(ivDetails.Userid)).FirstOrDefault();
                                        SendDRRGApproveEmail(userName.Name, ivDetails.Userid, ivDetails.Inverter_ID, ivDetails.Model_Name, DRRGStandardValues.IVModuleApproved);
                                    }
                                    else
                                    {
                                        message = GetDRRGErrormessage(myString);
                                    }
                                }
                                #endregion
                            }
                            #endregion

                            message = Translate.Text("The published 'List of Eligible Equipment' has been updated Successfully.");
                        }
                        return Json(new { status = true, Message = message }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager), ValidateAntiForgeryToken]
        public ActionResult RejectMultipleRegistration(RejectedViewModel model)
        {
            string message = string.Empty;
            string error = string.Empty;
            string[] refValues = null;
            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            List<ModuleItem> Lstmodule = new List<ModuleItem>();
            try
            {
                var rejectedcode = RejectedCode();
                if (!string.IsNullOrWhiteSpace(model.regnumber))
                {
                    refValues = model.regnumber.Split(',');
                }
                if (refValues != null)
                    Addfile(model.file, FileType.RejectedFile, FileEntity.EvaluatorRejection, rejectedcode, error, FileSizeLimit, supportedTypes, refValues[0], dRRG_Files_TYlist);

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(model.regnumber))
                        {

                            using (DRRGEntities context = new DRRGEntities())
                            {
                                foreach (var referenceId in refValues)
                                {
                                    #region [PV]
                                    if (referenceId.ToLower().StartsWith("pv"))
                                    {
                                        Proc_DRRG_PV_Mgr_RejectionModule schema_mgr_procedure_PV = new Proc_DRRG_PV_Mgr_RejectionModule()
                                        {
                                            useremail = CurrentPrincipal.Username,
                                            pvcode = referenceId,
                                            rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                            rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                            dRRG_Files_TY = dRRG_Files_TYlist
                                        };
                                        context.Database.ExecuteStoredProcedure(schema_mgr_procedure_PV);
                                        string pverrormessage = schema_mgr_procedure_PV.error;
                                        string pvusername = schema_mgr_procedure_PV.pvusername;
                                        string pvuseremail = schema_mgr_procedure_PV.pvuseremail;
                                        string pvmodelname = schema_mgr_procedure_PV.pvmodelname;
                                        if (!string.IsNullOrWhiteSpace(schema_mgr_procedure_PV.error) && schema_mgr_procedure_PV.error.Equals(DRRGStandardValues.Success))
                                        {
                                            var pvDetails = context.DRRG_PVMODULE.Where(x => x.PV_ID.ToLower().Equals(referenceId.ToLower())).FirstOrDefault();
                                            DRRGExtensions.Logger(pvDetails != null ? pvDetails.Model_Name : "PV", "Rejected by scheme manager - " + CurrentPrincipal.Name, "PV Module", referenceId, DateTime.Now, CurrentPrincipal.Username, "Rejected");
                                        }
                                    }

                                    #endregion

                                    #region [IP]
                                    if (referenceId.ToLower().StartsWith("ip"))
                                    {
                                        Proc_DRRG_IP_Mgr_RejectionModule schema_mgr_procedure_IP = new Proc_DRRG_IP_Mgr_RejectionModule()
                                        {
                                            useremail = CurrentPrincipal.Username,
                                            pvcode = referenceId,
                                            rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                            rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                            dRRG_Files_TY = dRRG_Files_TYlist
                                        };

                                        context.Database.ExecuteStoredProcedure(schema_mgr_procedure_IP);
                                        string iperrormessage = schema_mgr_procedure_IP.error;
                                        string ipusername = schema_mgr_procedure_IP.pvusername;
                                        string ipuseremail = schema_mgr_procedure_IP.pvuseremail;
                                        string ipmodelname = schema_mgr_procedure_IP.pvmodelname;
                                        if (!string.IsNullOrWhiteSpace(schema_mgr_procedure_IP.error) && schema_mgr_procedure_IP.error.Equals(DRRGStandardValues.Success))
                                        {
                                            var ipDetails = context.DRRG_InterfaceModule.Where(x => x.Interface_ID.ToLower().Equals(referenceId.ToLower())).FirstOrDefault();
                                            DRRGExtensions.Logger(ipDetails != null ? ipDetails.Model_Name : "IP", "Rejected by scheme manager - " + CurrentPrincipal.Name, "Interface Protection Module", referenceId, DateTime.Now, CurrentPrincipal.Username, "Rejected");
                                        }
                                    }
                                    #endregion

                                    #region [IV]
                                    if (referenceId.ToLower().StartsWith("inv"))
                                    {
                                        Proc_DRRG_IV_Mgr_RejectionModule schema_mgr_procedure_IV = new Proc_DRRG_IV_Mgr_RejectionModule()
                                        {
                                            useremail = CurrentPrincipal.Username,
                                            pvcode = referenceId,
                                            rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                            rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                            dRRG_Files_TY = dRRG_Files_TYlist
                                        };

                                        context.Database.ExecuteStoredProcedure(schema_mgr_procedure_IV);
                                        string errormessage = schema_mgr_procedure_IV.error;
                                        string invusername = schema_mgr_procedure_IV.pvusername;
                                        string invuseremail = schema_mgr_procedure_IV.pvuseremail;
                                        string invmodelname = schema_mgr_procedure_IV.pvmodelname;
                                        if (!string.IsNullOrWhiteSpace(schema_mgr_procedure_IV.error) && schema_mgr_procedure_IV.error.Equals(DRRGStandardValues.Success))
                                        {
                                            var ivDetails = context.DRRG_InverterModule.Where(x => x.Inverter_ID.ToLower().Equals(model.regnumber.ToLower())).FirstOrDefault();
                                            DRRGExtensions.Logger(ivDetails != null ? ivDetails.Model_Name : "IV", "Rejected by scheme manager - " + CurrentPrincipal.Name, "Inverter Module", referenceId, DateTime.Now, CurrentPrincipal.Username, "Rejected");
                                        }
                                    }


                                    #endregion
                                }
                                return Json(new { status = true, Message = Translate.Text("The list of the newly approved equipment has been rejected successfully and sent back to the Evaluators for their further action.") }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex, this);
                        message = ErrorMessages.UNEXPECTED_ERROR;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager), HttpGet]
        public ActionResult EvaluatorsRegistrationList()
        {
            try
            {
                if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                {
                    List<EvaluatorItem> Lstevaluator = new List<EvaluatorItem>();

                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var result = context.SP_DRRG_GETEvaluatorListBySchemaManager().ToList();
                        var index = 0;
                        result.Where(y => y != null).ForEach((x) => Lstevaluator.Add(new EvaluatorItem
                        {
                            Id = x.id.ToString(),
                            loginid = x.loginid,
                            ReferenceID = x.Reference_ID,
                            Name = x.Name,
                            CreatedDate = x.CreatedDate?.ToString(),
                            UpdatedDate = x.UpdatedDate?.ToString(),
                            Status = x.Status,
                            ReviewedBy = x.Reviewed_By,
                            ApprovedBy = x.Approved_By,
                            PublishedBy = x.Published_By,
                            serialNumber = (Interlocked.Increment(ref index)).ToString()
                        }));
                        //Lstevaluator = Lstevaluator.OrderByDescending(x => x.CreatedDate).ToList();
                        CacheProvider.Store(CacheKeys.DRRG__EVALUATOR_LIST, new CacheItem<List<EvaluatorItem>>(Lstevaluator, TimeSpan.FromMinutes(40)));
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/EvaluatorsRegistrationList.cshtml");
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager)]
        public ActionResult EvaluatorsListAjax(int pagesize = 5, string keyword = "", string statustxt = "", int page = 1, string namesort = "")
        {
            keyword = keyword.Trim();
            List<EvaluatorItem> Lstevaluator = new List<EvaluatorItem>();
            try
            {
                if (CacheProvider.TryGet(CacheKeys.DRRG__EVALUATOR_LIST, out Lstevaluator))
                {
                    FilteredDRRGModules filteredDRRGEvaluators = new FilteredDRRGModules
                    {
                        page = page
                    };
                    pagesize = pagesize > 100 ? 100 : pagesize;
                    filteredDRRGEvaluators.strdataindex = "0";
                    if (Lstevaluator != null && Lstevaluator.Count > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(statustxt))
                        {
                            if (statustxt.Equals("1"))
                            {
                                Lstevaluator = Lstevaluator.Where(x => x.Status.ToLower().Equals("approved")).ToList();
                            }
                            else if (statustxt.Equals("2"))
                            {
                                Lstevaluator = Lstevaluator.Where(x => x.Status.ToLower().Equals("submitted")).ToList();
                            }
                            else if (statustxt.Equals("3"))
                            {
                                Lstevaluator = Lstevaluator.Where(x => x.Status.ToLower().Equals("rejected")).ToList();
                            }
                        }
                        if (!string.IsNullOrWhiteSpace(keyword))
                        {
                            Lstevaluator = Lstevaluator.Where(x => (!string.IsNullOrWhiteSpace(x.ReferenceID) && x.ReferenceID.ToLower().Contains(keyword.ToLower()))
                            || (!string.IsNullOrWhiteSpace(x.Name) && x.Name.ToLower().Contains(keyword.ToLower()))
                            ).ToList();
                        }
                        filteredDRRGEvaluators.namesort = namesort;
                        filteredDRRGEvaluators.totalpage = Pager.CalculateTotalPages(Lstevaluator.Count(), pagesize);
                        filteredDRRGEvaluators.pagination = filteredDRRGEvaluators.totalpage > 1 ? true : false;
                        filteredDRRGEvaluators.firstitem = ((page - 1) * pagesize) + 1;
                        filteredDRRGEvaluators.lastitem = page * pagesize < Lstevaluator.Count() ? page * pagesize : Lstevaluator.Count();
                        filteredDRRGEvaluators.totalitem = Lstevaluator.Count();
                        int index = (page - 1) * pagesize;
                        filteredDRRGEvaluators.Lstevaluator = Lstevaluator.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                        filteredDRRGEvaluators.Lstevaluator.ForEach(x => x.serialNumber = (Interlocked.Increment(ref index)).ToString());
                        return Json(new { status = true, Message = filteredDRRGEvaluators }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager)]
        public ActionResult ApproveEvaluatorRegistration(string refIds)
        {
            string message = string.Empty;
            try
            {
                if (refIds != null)
                {
                    string[] refValues = refIds.Split(',');
                    List<EvaluatorItem> Lstevaluator = new List<EvaluatorItem>();

                    using (DRRGEntities context = new DRRGEntities())
                    {
                        if (CacheProvider.TryGet(CacheKeys.DRRG__EVALUATOR_LIST, out Lstevaluator))
                        {
                            if (Lstevaluator != null && Lstevaluator.Count > 0)
                            {
                                foreach (var referenceId in refValues)
                                {
                                    ObjectParameter myOutputParamresponse = new ObjectParameter(DRRGStandardValues.error, typeof(string));
                                    var result = context.SP_DRRG_EvaluatorRegApproval(referenceId.Trim(), CurrentPrincipal.Username, myOutputParamresponse);
                                    var evaluatorDetail = Lstevaluator.Where(x => x.ReferenceID.ToLower().Equals(referenceId.ToLower())).FirstOrDefault();
                                    string myString = Convert.ToString(myOutputParamresponse.Value);
                                    if (!string.IsNullOrWhiteSpace(myString) && myString.Equals(DRRGStandardValues.Success))
                                    {
                                        SendDRRGModuleEmail(evaluatorDetail.Name, evaluatorDetail.loginid, referenceId, DRRGStandardValues.EvaluatorRegistrationApproved);
                                        message = Translate.Text("The New Evaluator Registration Application(s) has (have) been approved Successfully.");
                                    }
                                    else
                                    {
                                        message = GetDRRGErrormessage(myString);
                                    }
                                }
                                return Json(new { status = true, Message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager)]
        public ActionResult RejectvaluatorRegistration(RejectedViewModel model)
        {
            string message = string.Empty;
            string error = string.Empty;
            string[] refValues = null;
            List<DRRG_Files_TY> dRRG_Files_TYlist = new List<DRRG_Files_TY>();
            List<EvaluatorItem> Lstevaluator = new List<EvaluatorItem>();
            try
            {
                var rejectedcode = RejectedCode();
                if (!string.IsNullOrWhiteSpace(model.regnumber))
                {
                    refValues = model.regnumber.Split(',');
                }
                if (refValues != null)
                    Addfile(model.file, FileType.RejectedFile, FileEntity.EvaluatorRejection, rejectedcode, error, FileSizeLimit, supportedTypes, refValues[0], dRRG_Files_TYlist);

                if (ModelState.IsValid)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(model.regnumber))
                        {

                            using (DRRGEntities context = new DRRGEntities())
                            {
                                foreach (var referenceId in refValues)
                                {
                                    Proc_DRRG_Evaluator_Mgr_Rejection procedure = new Proc_DRRG_Evaluator_Mgr_Rejection()
                                    {
                                        useremail = CurrentPrincipal.Username,
                                        referenceid = referenceId,
                                        rejectedreason = !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty,
                                        rejectedfileid = dRRG_Files_TYlist.Count > 0 ? rejectedcode : string.Empty,
                                        dRRG_Files_TY = dRRG_Files_TYlist
                                    };
                                    context.Database.ExecuteStoredProcedure(procedure);
                                    string errormessage = procedure.error;
                                    if (!string.IsNullOrWhiteSpace(procedure.error) && procedure.error.Equals(DRRGStandardValues.Success))
                                    {
                                        List<Tuple<string, byte[]>> attList = new List<Tuple<string, byte[]>>();

                                        if (CacheProvider.TryGet(CacheKeys.DRRG__EVALUATOR_LIST, out Lstevaluator))
                                        {
                                            if (Lstevaluator != null && Lstevaluator.Count > 0)
                                            {
                                                var evaluatorDetail = Lstevaluator.Where(x => x.ReferenceID.ToLower().Equals(referenceId.ToLower())).FirstOrDefault();

                                                if (dRRG_Files_TYlist != null && dRRG_Files_TYlist.Count > 0)
                                                {
                                                    dRRG_Files_TYlist.ForEach(x => attList.Add(new Tuple<string, byte[]>(x.Name, x.Content)));
                                                }
                                                SendDRRGRejectionEmail(evaluatorDetail.Name, evaluatorDetail.loginid, model.regnumber, !string.IsNullOrWhiteSpace(model.rejectionreason) ? model.rejectionreason.Replace("\r\n", "<br>") : string.Empty, string.Empty, DRRGStandardValues.EvaluatorRegistrationRejected, attList);
                                                message = Translate.Text("The New Evaluator Registration Application(s) has (have) been rejected Successfully.");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LogService.Error(new Exception(procedure.error), this);
                                        message = GetDRRGErrormessage(procedure.error);
                                    }
                                }
                                return Json(new { status = true, Message = message }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogService.Error(ex, this);
                        message = ErrorMessages.UNEXPECTED_ERROR;
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = message }, JsonRequestBehavior.AllowGet);
        }
        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager), HttpGet]
        public ActionResult Reports()
        {
            ReportModel model = new ReportModel();
            try
            {
                if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        model.selectedPeriod = "1";
                        var command = context.Database.Connection.CreateCommand();
                        command.CommandText = "[dbo].[SP_DRRG_SchemaMgr_GetReports]";
                        command.CommandType = CommandType.StoredProcedure;
                        DbParameter paramFrom = command.CreateParameter();
                        paramFrom.ParameterName = "@fromDate";
                        paramFrom.DbType = DbType.String;
                        paramFrom.Direction = ParameterDirection.Input;
                        paramFrom.Value = "";

                        DbParameter paramTo = command.CreateParameter();
                        paramTo.ParameterName = "@toDate";
                        paramTo.DbType = DbType.String;
                        paramTo.Direction = ParameterDirection.Input;
                        paramTo.Value = "";

                        command.Parameters.Add(paramFrom);
                        command.Parameters.Add(paramTo);
                        context.Database.Connection.Open();
                        var reader = command.ExecuteReader();

                        List<ProcessedApplications> _processedApplications =
                        ((IObjectContextAdapter)context).ObjectContext.Translate<ProcessedApplications>
                        (reader).ToList();
                        reader.NextResult();
                        List<ProcessedApplications> _processedApplicationsMonYear =
                            ((IObjectContextAdapter)context).ObjectContext.Translate<ProcessedApplications>
                    (reader).ToList();
                        reader.NextResult();
                        List<ProcessedApplications> _processedApplicationsModel =
                            ((IObjectContextAdapter)context).ObjectContext.Translate<ProcessedApplications>
                    (reader).ToList();

                        model.Submitted = _processedApplications.Where(x => x.Status == "Submitted").Select(x => x.Applications).FirstOrDefault();
                        model.ReviewerApproved = _processedApplications.Where(x => x.Status == "ReviewerApproved" ).Select(x => x.Applications).FirstOrDefault();
                        model.Updated = _processedApplications.Where(x =>  x.Status == "Updated").Select(x => x.Applications).FirstOrDefault();
                        model.SchemeMgrRejected = _processedApplications.Where(x =>  x.Status == "Rejected" ).Select(x => x.Applications).FirstOrDefault();
                        model.Approved = _processedApplications.Where(x => x.Status == "Approved").Select(x => x.Applications).FirstOrDefault();
                        model.Rejected = _processedApplications.Where(x => x.Status == "ReviewerRejected").Select(x => x.Applications).FirstOrDefault();

                        model.PVModuleApproved = _processedApplicationsModel.Where(x => x.Status == "Approved" && x.Model == "PVMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.PVModuleRejected = _processedApplicationsModel.Where(x => x.Status == "ReviewerRejected" && x.Model == "PVMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IVModuleApproved = _processedApplicationsModel.Where(x => x.Status == "Approved" && x.Model == "INVERTERMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IVModuleRejected = _processedApplicationsModel.Where(x => x.Status == "ReviewerRejected" && x.Model == "INVERTERMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IPModuleApproved = _processedApplicationsModel.Where(x => x.Status == "Approved" && x.Model == "INTERFACEMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IPModuleRejected = _processedApplicationsModel.Where(x => x.Status == "ReviewerRejected" && x.Model == "INTERFACEMODULE").Select(x => x.Applications).FirstOrDefault();

                        var pvEquipmentManufacturer = context.DRRG_PVMODULE.Where(x => x.Status.Equals("Approved")).Select(x => x.Manufacturer_Code).Distinct().Count();
                        var invEquipmentManufacturer = context.DRRG_InverterModule.Where(x => x.Status.Equals("Approved")).Select(x => x.Manufacturer_Code).Distinct().Count();
                        var ipEquipmentManufacturer = context.DRRG_InterfaceModule.Where(x => x.Status.Equals("Approved")).Select(x => x.Manufacturer_Code).Distinct().Count();
                        List<EquipmentType> processedEquipmentManufacturer = new List<EquipmentType>();

                        processedEquipmentManufacturer.Add(new EquipmentType    
                        {
                            ApplicationName = "Interface Protection Manufacturers",
                            Applications = ipEquipmentManufacturer
                        });
                        processedEquipmentManufacturer.Add(new EquipmentType
                        {
                            ApplicationName = "PV Inverter Manufacturers",
                            Applications = invEquipmentManufacturer
                        });
                        processedEquipmentManufacturer.Add(new EquipmentType
                        {
                            ApplicationName = "PV Module Manufacturers",
                            Applications = pvEquipmentManufacturer
                        });
                        model.processedEquipmentManufacturer = processedEquipmentManufacturer.ToList();
                        model.processedApplications = _processedApplications.ToList();
                        model.processedApplicationsMonYear = _processedApplicationsMonYear;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return View("~/Views/Feature/DRRG/Evaluator/Reports.cshtml", model);
        }
        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGSchemaManager)]
        public ActionResult Reports(ReportModel model)
        {
            try
            {
                if (CurrentPrincipal.Role.Equals(Roles.DRRGSchemaManager))
                {
                    using (DRRGEntities context = new DRRGEntities())
                    {
                        var command = context.Database.Connection.CreateCommand();
                        command.CommandText = "[dbo].[SP_DRRG_SchemaMgr_GetReports]";
                        command.CommandType = CommandType.StoredProcedure;

                        DateTime.TryParse(model.fromDate, out DateTime fromdateTime);
                        DateTime.TryParse(model.toDate, out DateTime todatetime);

                        DbParameter paramFrom = command.CreateParameter();
                        paramFrom.ParameterName = "@fromDate";
                        paramFrom.DbType = DbType.String;
                        paramFrom.Direction = ParameterDirection.Input;
                        paramFrom.Value = model.fromDate != null ? fromdateTime.ToString("yyyy-MM-dd") : "";

                        DbParameter paramTo = command.CreateParameter();
                        paramTo.ParameterName = "@toDate";
                        paramTo.DbType = DbType.String;
                        paramTo.Direction = ParameterDirection.Input;
                        paramTo.Value = model.toDate != null ? todatetime.ToString("yyyy-MM-dd") : "";

                        command.Parameters.Add(paramFrom);
                        command.Parameters.Add(paramTo);
                        context.Database.Connection.Open();
                        var reader = command.ExecuteReader();

                        List<ProcessedApplications> _processedApplications =
                        ((IObjectContextAdapter)context).ObjectContext.Translate<ProcessedApplications>
                        (reader).ToList();
                        reader.NextResult();
                        List<ProcessedApplications> _processedApplicationsMonYear =
                            ((IObjectContextAdapter)context).ObjectContext.Translate<ProcessedApplications>
                    (reader).ToList();
                        reader.NextResult();
                        List<ProcessedApplications> _processedApplicationsModel =
                            ((IObjectContextAdapter)context).ObjectContext.Translate<ProcessedApplications>
                    (reader).ToList();

                        model.Submitted = _processedApplications.Where(x => x.Status == "Submitted").Select(x => x.Applications).FirstOrDefault();
                        model.ReviewerApproved = _processedApplications.Where(x => x.Status == "ReviewerApproved").Select(x => x.Applications).FirstOrDefault();
                        model.Updated = _processedApplications.Where(x => x.Status == "Updated").Select(x => x.Applications).FirstOrDefault();
                        model.SchemeMgrRejected = _processedApplications.Where(x => x.Status == "Rejected").Select(x => x.Applications).FirstOrDefault();
                        model.Approved = _processedApplications.Where(x => x.Status == "Approved").Select(x => x.Applications).FirstOrDefault();
                        model.Rejected = _processedApplications.Where(x => x.Status == "ReviewerRejected").Select(x => x.Applications).FirstOrDefault();

                        model.PVModuleApproved = _processedApplicationsModel.Where(x => x.Status == "Approved" && x.Model == "PVMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.PVModuleRejected = _processedApplicationsModel.Where(x => x.Status == "ReviewerRejected" && x.Model == "PVMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IVModuleApproved = _processedApplicationsModel.Where(x => x.Status == "Approved" && x.Model == "INVERTERMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IVModuleRejected = _processedApplicationsModel.Where(x => x.Status == "ReviewerRejected" && x.Model == "INVERTERMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IPModuleApproved = _processedApplicationsModel.Where(x => x.Status == "Approved" && x.Model == "INTERFACEMODULE").Select(x => x.Applications).FirstOrDefault();
                        model.IPModuleRejected = _processedApplicationsModel.Where(x => x.Status == "ReviewerRejected" && x.Model == "INTERFACEMODULE").Select(x => x.Applications).FirstOrDefault();

                        DateTime dtFrom = Convert.ToDateTime(model.fromDate);
                        DateTime dtTo = Convert.ToDateTime(model.toDate);

                        var pvEquipmentManufacturer = context.DRRG_PVMODULE.Where(x => x.Status.Equals("Approved")
                                                                                    && x.UpdatedDate >= dtFrom
                                                                                    && x.UpdatedDate <= dtTo).Select(x => x.Manufacturer_Code).Distinct().Count();
                        var invEquipmentManufacturer = context.DRRG_InverterModule.Where(x => x.Status.Equals("Approved")
                                                                                    && x.UpdatedDate >= dtFrom
                                                                                    && x.UpdatedDate <= dtTo).Select(x => x.Manufacturer_Code).Distinct().Count();
                        var ipEquipmentManufacturer = context.DRRG_InterfaceModule.Where(x => x.Status.Equals("Approved")
                                                                                    && x.UpdatedDate >= dtFrom
                                                                                    && x.UpdatedDate <= dtTo).Select(x => x.Manufacturer_Code).Distinct().Count();
                        List<EquipmentType> processedEquipmentManufacturer = new List<EquipmentType>();

                        processedEquipmentManufacturer.Add(new EquipmentType
                        {
                            ApplicationName = Translate.Text("DRRG_InterfaceProtectionManufacturers"),
                            Applications = ipEquipmentManufacturer
                        });
                        processedEquipmentManufacturer.Add(new EquipmentType
                        {
                            ApplicationName = Translate.Text("DRRG_PVInverterManufacturers"),
                            Applications = invEquipmentManufacturer
                        });
                        processedEquipmentManufacturer.Add(new EquipmentType
                        {
                            ApplicationName = Translate.Text("DRRG_PVModuleManufacturers"),
                            Applications = pvEquipmentManufacturer
                        });
                        model.processedEquipmentManufacturer = processedEquipmentManufacturer.ToList();
                        model.processedApplications = _processedApplications.ToList();
                        model.processedApplicationsMonYear = _processedApplicationsMonYear;

                        model.processedApplications = _processedApplications.ToList();
                        model.processedApplicationsMonYear = _processedApplicationsMonYear;
                    }
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
            return View("~/Views/Feature/DRRG/Evaluator/Reports.cshtml", model);
        }
        private string GetViewStatus(string status)
        {
            string strStatus = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(status) && (status.Equals("Submitted") || status.Equals("Updated") || status.Equals("Rejected")))
                {
                    strStatus = Translate.Text("DRRG_UnderEvaluation");
                }
                else if (!string.IsNullOrWhiteSpace(status) && status.Equals("ReviewerApproved"))
                {
                    strStatus = Translate.Text("DRRG_PendingFinalApproval");
                }
                else if (!string.IsNullOrWhiteSpace(status) && (status.Equals("ReviewerRejected")))
                {
                    strStatus = Translate.Text("DRRG_Rejected");
                }
                else if (!string.IsNullOrWhiteSpace(status) && status.Equals("Approved"))
                {
                    strStatus = Translate.Text("DRRG_Published");
                }
                else
                {
                    strStatus = status;
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return strStatus;
        }
        private ApplicationLogComposite GetAdminLogs()
        {
            ApplicationLogComposite model = new ApplicationLogComposite();
            using (DRRGEntities context = new DRRGEntities())
            {
                var result = context.SP_DRRG_GETModulesByAdminLogs()
                                    .Where(x => !string.IsNullOrWhiteSpace(x.Status))
                                    .OrderBy(x => x.CreatedDate)
                                    .ThenByDescending(x => x.UpdatedDate).ToList();
                int index = 0;
                result.Where(y => y != null).ForEach((x) => model.ApplicationLog.Add(new ApplicationLog
                {
                    serialnumber = (Interlocked.Increment(ref index)).ToString(),
                    modelName = x.modelname,
                    status = x.Status,
                    statusText = GetViewStatus(x.Status),
                    Representativename = x.CompanyName,
                    referenceNumber = x.referenceid,
                    type = x.model,
                    equipmentType = Translate.Text(x.model),
                    manufacturerName = x.manuname,
                    datedtSubmitted = x.CreatedDate,
                    dateSubmitted = x.CreatedDate.ToString(),
                    Evaluator = !string.IsNullOrWhiteSpace(x.Evaluator) ?
                                 !string.IsNullOrWhiteSpace(context.DRRG_EvaluatorLogin.Where(e => e.loginid.ToLower().Equals(x.Evaluator.ToLower())).Select(s => s.Name).FirstOrDefault()) ?
                                 context.DRRG_EvaluatorLogin.Where(e => e.loginid.ToLower().Equals(x.Evaluator.ToLower())).Select(s => s.Name).FirstOrDefault() : "N/A" : "N/A",
                    ManufacturerCode = x.manucode
                }));
                var prevManufacturer = string.Empty;
                foreach (var item in model.ApplicationLog.ToList())
                {
                    if (prevManufacturer != item.manufacturerName)
                        model.ManufacturerList.Add(item.ManufacturerCode, item.manufacturerName);
                    prevManufacturer = item.manufacturerName;
                }
                model.EvaluatedDate = model.ApplicationLog.Select(x => x.dateSubmitted).Distinct().ToList();

                model.EvaluatorList = model.ApplicationLog.Select(x => x.Evaluator).Distinct().ToList();

                model.ApplicationLog = model.ApplicationLog.OrderByDescending(x => x.datedtSubmitted).ToList();

            }
            return model;
        }
        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager), HttpGet]
        public ActionResult ApplicationLogs()
        {
            ApplicationLogComposite model = new ApplicationLogComposite();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    model = GetAdminLogs();
                    CacheProvider.Store(CacheKeys.DRRG__APPLICATIONLOG_LIST, new CacheItem<ApplicationLogComposite>(model, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/ApplicationLogList.cshtml");
        }

        [HttpPost, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager)]
        public ActionResult GetApplicationLogList(int pagesize = 5, string keyword = "", string applicationtype = "", string statustxt = "all", int page = 1, string startDate = "", string Endate = "")
        {
            keyword = keyword.Trim();
            try
            {
                ApplicationLogComposite filterModel = new ApplicationLogComposite();
                filterModel = GetAdminLogs();
                FilteredDRRGModules filteredDRRGModules = new FilteredDRRGModules
                {
                    page = page
                };
                pagesize = pagesize > 100 ? 100 : pagesize;
                filteredDRRGModules.strdataindex = "0";
                if (filterModel != null && filterModel.ApplicationLog.Count > 0)
                {
                    if (!string.IsNullOrWhiteSpace(applicationtype))
                    {
                        if (applicationtype.Equals("1"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.type.Equals(FileEntity.PVmodule.ToString())).ToList();
                        }
                        else if (applicationtype.Equals("2"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.type.Equals(FileEntity.Invertermodule.ToString())).ToList();
                        }
                        else if (applicationtype.Equals("3"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.type.Equals(FileEntity.Interfacemodule.ToString())).ToList();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(statustxt) && !statustxt.Equals("all"))
                    {
                        if (statustxt.Equals("1"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.statusText.Equals("Published")).ToList();
                        }
                        else if (statustxt.Equals("2"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.statusText.Equals("Pending Final Approval")).ToList();
                        }
                        else if (statustxt.Equals("3"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.statusText.Equals("Under Evaluation")).ToList();
                        }
                        else if (statustxt.Equals("4"))
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.statusText.Equals("Deleted")).ToList();
                        }
                        else
                        {
                            filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.statusText.Equals("Rejected")).ToList();
                        }
                    }
                    if (!string.IsNullOrEmpty(startDate))
                    {
                        DateTime.TryParse(startDate, out DateTime fromdateTime);
                        filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.datedtSubmitted != null && x.datedtSubmitted.Value.Date >= fromdateTime.Date).ToList();
                    }
                    if (!string.IsNullOrEmpty(Endate))
                    {
                        DateTime.TryParse(Endate, out DateTime todatetime);
                        filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => x.datedtSubmitted != null && x.datedtSubmitted.Value.Date <= todatetime.Date).ToList();
                    }

                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        filterModel.ApplicationLog = filterModel.ApplicationLog.Where(x => (!string.IsNullOrWhiteSpace(x.referenceNumber) && x.referenceNumber.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.modelName) && x.modelName.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.type) && x.type.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.manufacturerName) && x.manufacturerName.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.Evaluator) && x.Evaluator.ToLower().Contains(keyword.ToLower()))
                        || (!string.IsNullOrWhiteSpace(x.Representativename) && x.Representativename.ToLower().Contains(keyword.ToLower()))
                        ).ToList();
                    }

                    filteredDRRGModules.LastRecords.EvaluatorList = filterModel.EvaluatorList;
                    filteredDRRGModules.LastRecords.ManufacturerList = filterModel.ManufacturerList;
                    filteredDRRGModules.LastRecords.EvaluatedDate = filterModel.EvaluatedDate;
                    //filteredDRRGModules.namesort = namesort;
                    filteredDRRGModules.totalpage = Pager.CalculateTotalPages(filterModel.ApplicationLog.Count(), pagesize);
                    filteredDRRGModules.pagination = filteredDRRGModules.totalpage > 1 ? true : false;
                    filteredDRRGModules.firstitem = ((page - 1) * pagesize) + 1;
                    filteredDRRGModules.lastitem = page * pagesize < filterModel.ApplicationLog.Count() ? page * pagesize : filterModel.ApplicationLog.Count();
                    filteredDRRGModules.totalitem = filterModel.ApplicationLog.Count();
                    int index = ((page - 1) * pagesize);
                    filteredDRRGModules.LastRecords.ApplicationLog = filterModel.ApplicationLog.Skip((page - 1) * pagesize).Take(pagesize).ToList();
                    filteredDRRGModules.LastRecords.ApplicationLog.ForEach(x => x.serialnumber = (Interlocked.Increment(ref index)).ToString());
                    return Json(new { status = true, Message = filteredDRRGModules }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EligibleEquipmentList()
        {
            List<DRRG_Manufacturer_Details> lst = null;
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    lst = context.DRRG_Manufacturer_Details.Where(x => x.Status.ToLower().Equals("approved")).OrderBy(x => x.Manufacturer_Name).ToList();
                    //lst = (from mn in context.DRRG_Manufacturer_Details
                    //       join pv in context.DRRG_PVMODULE on mn.Manufacturer_Code equals pv.Manufacturer_Code
                    //       join pinv in context.DRRG_InverterModule on mn.Manufacturer_Code equals pinv.Manufacturer_Code
                    //       join ip in context.DRRG_InterfaceModule on mn.Manufacturer_Code equals ip.Manufacturer_Code
                    //       orderby mn.Manufacturer_Name ascending
                    //       select mn).Where(x => x.Status.ToLower().Equals("approved")).ToList();

                    ViewBag.CurrentRole = CurrentPrincipal.Role;
                    CacheProvider.Store(CacheKeys.DRRG__ELIGIBLEEQUIPMENT_LIST, new CacheItem<List<DRRG_Manufacturer_Details>>(lst, TimeSpan.FromMinutes(40)));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return View("~/Views/Feature/DRRG/Evaluator/EligibleEquipmentList.cshtml", lst);
        }

        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator), HttpGet]
        public ActionResult BackendList()
        {
            List<DRRG_Manufacturer_Details> lst = null;
            using (DRRGEntities context = new DRRGEntities())
            {
                lst = context.DRRG_Manufacturer_Details.ToList();
                CacheProvider.Store(CacheKeys.DRRG__BACKEND_LIST, new CacheItem<List<DRRG_Manufacturer_Details>>(lst, TimeSpan.FromMinutes(40)));
            }
            return View("~/Views/Feature/DRRG/Evaluator/BackendList.cshtml", lst);
        }

        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator), HttpGet]
        public ActionResult GetDRRGModulesByManufacturer(string manuCode, string type)
        {
            DRRGModuleList model = new DRRGModuleList();
            model.isAdmin = false;
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.DRRGEvaulator))
            {
                model.isAdmin = true;
            }
            using (DRRGEntities context = new DRRGEntities())
            {
                if (type.ToLower().Trim() == "pv")
                {
                    int index = 0;
                    var pv = context.SP_DRRG_GETPVModuleByManuId(manuCode.Trim()).ToList();
                    pv.Where(y => y != null).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        modelName = x.PV_ID,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Cell_Technology,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Nominal_Power,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_PvModuleListing.cshtml", model);
                }
                else if (type.ToLower().Trim() == "inv")
                {
                    int index = 0;
                    var iv = context.SP_DRRG_GETInverterModuleByManuId(manuCode.Trim()).ToList();
                    iv.Where(y => y != null).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        modelName = x.Inverter_ID,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Model_Name,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = Convert.ToString(x.Protection_Degree),
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();
                    return PartialView("~/Views/Feature/DRRG/Evaluator/_InverterModuleListing.cshtml", model);
                }
                else if (type.ToLower().Trim() == "ip")
                {
                    int index = 0;
                    var ip = context.SP_DRRG_GETInterfaceModuleByManuId(manuCode.Trim()).ToList();
                    ip.Where(y => y != null).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        modelName = x.Interface_ID,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Model_Name,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Compliance,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();
                    return PartialView("~/Views/Feature/DRRG/Evaluator/_InterfaceModule.cshtml", model);
                }
                else
                {
                    int index = 0;
                    var pv = context.SP_DRRG_GETPVModuleByManuId(manuCode.Trim()).ToList();
                    pv.Where(y => y != null).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        modelName = x.PV_ID,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Cell_Technology,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Nominal_Power,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method
                        // id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();
                    return PartialView("~/Views/Feature/DRRG/Evaluator/_PvModuleListing.cshtml", model);
                }
            }

        }


        public ActionResult GetManufacturers(string manuCode, string type)
        {
            DRRGModuleList model = new DRRGModuleList();
            model.isAdmin = false;
            using (DRRGEntities context = new DRRGEntities())
            {
                if (type.ToLower().Trim() == "pv")
                {
                    int index = 0;
                    var pv = context.DRRG_PVMODULE.Where(x => x.Status.ToLower().Equals("approved") && x.Manufacturer_Code.ToLower().Equals(manuCode.ToLower())).ToList();
                    pv.Where(y => y != null && y.Status.ToLower().Equals("approved")).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        pvId = x.PV_ID,
                        modelName = x.Model_Name,
                        nominalpower = GetNominalPowerDetails(x),
                        celltechnology = x.Cell_Technology,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Cell_Technology,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Nominal_Power,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method,
                        extraCompliance = getExtraCompliance(x.PV_ID, "pv", context)
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_PvModuleListing.cshtml", model);
                }
                else if (type.ToLower().Trim() == "inv" || type.ToLower().Trim() == "iv")
                {
                    int index = 0;
                    var iv = context.DRRG_InverterModule.Where(x => x.Status.ToLower().Equals("approved") && x.Manufacturer_Code.ToLower().Equals(manuCode.ToLower())).ToList();
                    iv.Where(y => y != null).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        inverterId = x.Inverter_ID,
                        modelName = x.Model_Name,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Model_Name,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = Convert.ToString(x.Protection_Degree),
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method,
                        ratedpower = GetRatedPower(x),
                        acparentpower = GetACParentPower(x),
                        usageCategory = x.Function_String,

                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0,
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_InverterModuleListing.cshtml", model);
                }
                else if (type.ToLower().Trim() == "ip")
                {
                    int index = 0;
                    var ip = context.DRRG_InterfaceModule.Where(x => x.Status.ToLower().Equals("approved") && x.Manufacturer_Code.ToLower().Equals(manuCode.ToLower())).ToList();
                    ip.Where(y => y != null && y.Status.ToLower().Equals("approved")).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        interfaceId = x.Interface_ID,
                        modelName = x.Model_Name,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Model_Name,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Compliance,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method,
                        extraCompliance = getExtraCompliance(x.Interface_ID, "ip", context)
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_InterfaceModule.cshtml", model);
                }
                else
                {
                    int index = 0;
                    var pv = context.SP_DRRG_GETPVModuleByManuId(manuCode.Trim()).ToList();
                    pv.Where(y => y != null && y.Status.ToLower().Equals("approved")).ForEach((x) => model.ModuleItem.Add(new ModuleItem
                    {
                        modelName = x.PV_ID,
                        referenceNumber = x.Manufacturer_Code,
                        representative = x.Cell_Technology,
                        datedtSubmitted = x.CreatedDate,
                        dateSubmitted = x.CreatedDate.ToString(),
                        type = x.Nominal_Power,
                        serialnumber = (Interlocked.Increment(ref index)).ToString(),
                        testMethod = x.Salt_Mist_Test_Method
                        //id = x.File_ID.HasValue ? x.File_ID.Value : 0
                    }));

                    model.ModuleItem = model.ModuleItem.OrderByDescending(x => x.datedtSubmitted).ToList();

                    return PartialView("~/Views/Feature/DRRG/Evaluator/_PvModuleListing.cshtml", model);
                }
            }

        }
        [HttpGet, TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator)]
        public ActionResult DownloadFile(string id)
        {
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    long fileId = Convert.ToInt64(id);
                    var pvresultfiles = context.DRRG_Files.Where(x => x.File_ID == fileId).ToList();
                    if (pvresultfiles != null && pvresultfiles.Count > 0)
                    {
                        byte[] bytes = pvresultfiles.FirstOrDefault().Content;
                        string type = pvresultfiles.FirstOrDefault().ContentType;
                        return File(bytes, type);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return null;
        }
        [HttpGet]
        public ActionResult mdFile(string id, string manu)
        {
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var pvresultfiles = context.SP_DRRG_GETFilesbyID(id, manu).ToList();
                    if (pvresultfiles != null && pvresultfiles.Count > 0)
                    {
                        byte[] bytes = pvresultfiles.Where(x => x.File_Type.Equals(FileType.ModelDataSheet)).OrderByDescending(x => x.File_ID).FirstOrDefault().Content;
                        string type = pvresultfiles.FirstOrDefault().ContentType;
                        return File(bytes, type);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }

            return null;
        }
        public string historyStatus(string status)
        {
            string strStatus = status;
            switch (status)
            {
                case "In Progress":
                case "Rejected":
                    strStatus = Translate.Text("DRRG_UnderEvaluation");
                    break;
                case "Updated":
                    strStatus = Translate.Text("DRRG_Updated");
                    break;
                case "ReviewerRejected":
                    strStatus = Translate.Text("DRRG_Rejected");
                    break;
                case "ReviewerApproved":
                    strStatus = Translate.Text("DRRG_PendingFinalApproval");
                    break;
                case "Deleted":
                    strStatus = Translate.Text("DRRG_Removed");
                    break;
                default:
                    strStatus = Translate.Text("DRRG_Published");
                    break;
            }
            return strStatus;
        }
        [TwoPhaseDRRGAuthorize(CurrentDRRGRole = Roles.DRRGEvaulator + "," + Roles.DRRGSchemaManager), HttpGet]
        public ActionResult GetApplicationLogHistory(string referenceId)
        {
            List<ApplicationHistory> dRRG_ApplicationHistories = new List<ApplicationHistory>();
            try
            {
                using (DRRGEntities context = new DRRGEntities())
                {
                    var logHistrory = context.DRRG_ApplicationHistory.Where(l => l.Reference == referenceId.ToLower().Trim()).OrderBy(x => x.Date).ToList();
                    Nullable<System.DateTime> prevDate = null;
                    foreach (var item in logHistrory)
                    {

                        string processTime = string.Empty;
                        if (prevDate != null)
                        {
                            TimeSpan diffDate = Convert.ToDateTime(item.Date) - Convert.ToDateTime(prevDate);
                            //processTime = String.Format("{0} days, {1} hours, {2} minutes, {3} seconds", diffDate.Days.ToString(), diffDate.Hours, diffDate.Minutes, diffDate.Seconds);

                            if (diffDate.Days != 0)
                                processTime = String.Format("{0} days, ", diffDate.Days.ToString());
                            if (diffDate.Hours != 0)
                                processTime += String.Format("{0} hours, ", diffDate.Hours);
                            if (diffDate.Minutes != 0)
                                processTime += String.Format("{0} minutes, ", diffDate.Minutes);
                            //if (diffDate.Seconds != 0)
                            //    processTime += String.Format("{0} seconds, ", diffDate.Seconds);
                            processTime = processTime.TrimEnd().TrimEnd(',');
                        }
                        dRRG_ApplicationHistories.Add(new ApplicationHistory
                        {
                            id = item.id,
                            Reference = item.Reference,
                            Name = item.Name,
                            Description = item.Description,
                            Status = item.Status,
                            StatusText = historyStatus(item.Status),
                            Date = Convert.ToDateTime(item.Date),
                            Type = item.Type,
                            User = item.User,
                            processingTime = processTime
                        });
                        prevDate = Convert.ToDateTime(item.Date);
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
            }
            return PartialView("~/Views/Feature/DRRG/Evaluator/_LogHistory.cshtml", dRRG_ApplicationHistories);
        }
    }
}
