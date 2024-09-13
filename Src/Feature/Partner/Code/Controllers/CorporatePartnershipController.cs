// <copyright file="CorporatePartnershipController.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.Partner.Controllers
{
    using DEWAXP.Feature.Partner.Models.CorporatePartnership;
    using DEWAXP.Feature.Partner.Models.CorporatePartnership.StaticModels;
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Models.Kofax;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Content.Services;
    using DEWAXP.Foundation.Content.Utils;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.CorporatePortal;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Integration.Responses.KofaxRest;
    using DEWAXP.Foundation.Logger;
    using global::Sitecore.Globalization;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Script.Serialization;
    using Attribute = DEWAXP.Foundation.Content.Models.Kofax.Attribute;
    using SitecoreX = global::Sitecore.Context;

    /// <summary>
    /// Defines the <see cref="CorporatePartnershipController" />.
    /// </summary>
    public class CorporatePartnershipController : BaseController
    {
        /// <summary>
        /// The Login.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            if (IsCorportateuserloggedin())
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalDashboard);
            }
            return View("~/Views/Feature/Partner/CorporatePartnership/_LoginFormMain.cshtml", new LoginModel());
        }

        /// <summary>
        /// The Login.
        /// </summary>
        /// <param name="model">The model<see cref="LoginModel"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model)
        {
            GetPartnerName(model.Username, model.Password, out string partnername, out string coordinatorname);
            //ServiceResponse<bool> response = CorportatePortalClient.LoginService(model.Username, model.Password);
            //if (response.Succeeded && response.Payload)
            if (!string.IsNullOrWhiteSpace(partnername) && !string.IsNullOrWhiteSpace(coordinatorname))
            {
                model.CP_PartnerName = partnername;
                model.CP_CoorName = coordinatorname;
                model.CP_PartnerID = "NA";
                CacheProvider.Store(CacheKeys.Corporate_Portal_LOGIN_MODEL, new CacheItem<LoginModel>(model));
                //CacheProvider.Store(CacheKeys.Corporate_Portal_USER_DETAILS, new CacheItem<userDetailsExternal>(response.Payload.userDetails));
                AuthStateService.Save(new DewaProfile(model.Username, string.Empty, Roles.CorporatePortal)
                {
                    Name = model.Username,
                    IsContactUpdated = true
                });
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalDashboard);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Translate.Text("cpportal_invalidlogin"));
            }
            return View("~/Views/Feature/Partner/CorporatePartnership/_LoginFormMain.cshtml", new LoginModel());
        }

        /// <summary>
        /// The GetPartnerName
        /// </summary>
        /// <returns>The <see cref="List{string}"/></returns>
        private void GetPartnerName(string username, string password, out string partnername, out string coordinatorname)
        {
            partnername = string.Empty;
            coordinatorname = string.Empty;
            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
                baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.CPVariableUserManagement) { Attribute = GetUserAttributes(username, password) });
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CPUserManagement, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("Kofax Service User Management") { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        var ut = res.Payload.Values.Where(x => x.TypeName.Equals(CP_TYPE_NAME_UM)).FirstOrDefault();
                        partnername = ut.Attribute.Where(x => x.Name.Equals(CP_PARTNER_NAME)).FirstOrDefault().Value;
                        coordinatorname = ut.Attribute.Where(x => x.Name.Equals(CP_CORD_NAME)).FirstOrDefault().Value;
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }
        }

        /// <summary>
        /// Edit User Password
        /// </summary>
        /// <returns>The <see cref="List{string}"/></returns>
        private void ChangePassword(string username, string password, out string status)
        {
            status = string.Empty;
            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
                baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.CPVariableUserManagement) { Attribute = ChangePasswordAttributes(username, password) });
                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CPUserManagement, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception("Kofax Service User Management") { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        status = CP_SUCCESS;
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

        }



        /// <summary>
        /// The Logout.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Logout()
        {
            CacheProvider.Remove(CacheKeys.Corporate_Portal_LOGIN_MODEL);
            CacheProvider.Remove(CacheKeys.Corporate_Portal_USER_DETAILS);
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The Dashboard.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult Dashboard()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_USER_DETAILS, out taskDetailsExternalResponse response))
                {
                    global::Sitecore.Data.Items.Item CPConfig = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.CorporatePortal_CONFIG);
                    InboxViewModel inboxmodel = new InboxViewModel
                    {
                        sentAndPipeLineTaskResponse = ListofMsg_ids_issues(model.Username, int.Parse(CPConfig.Fields["Dashboard Record"].Value), RequestType.RecentMessages),
                        taskDetailsExternalResponse = response
                    };
                    GetSubmittedRequestCount(model.Username, out string eventcount, out int ideascount, out string issuecount, out string requestscount);
                    inboxmodel.taskDetailsExternalResponse.serviceCounts.submittedRequest = requestscount;
                    inboxmodel.taskDetailsExternalResponse.serviceCounts.submittedIdeasCount = ideascount;
                    inboxmodel.taskDetailsExternalResponse.serviceCounts.reportedIssue = issuecount;

                    return View("~/Views/Feature/Partner/CorporatePartnership/Dashboard.cshtml", inboxmodel);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        // GET: CorporatePartnership
        /// <summary>
        /// The ProfileAvatar.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ProfileAvatar()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                global::Sitecore.Data.Items.Item CPConfig = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.CorporatePortal_CONFIG);
                ServiceResponse<taskDetailsExternalResponse> response = CorportatePortalClient.GetDashboardDetails(model.CP_CoorName, model.CP_PartnerName, CPConfig.Fields["Dashboard Record"].Value);
                if (response.Succeeded && response.Payload != null && response.Payload.userDetails != null && response.Payload.userDetails.active)
                {
                    if (response.Payload.partnerDetails != null && !string.IsNullOrWhiteSpace(response.Payload.partnerDetails.partnerID) && !string.IsNullOrWhiteSpace(response.Payload.partnerDetails.partnerName))
                    {
                        model.CP_PartnerID = response.Payload.partnerDetails.partnerID;
                        model.CP_PartnerName = response.Payload.partnerDetails.partnerName;
                        CacheProvider.Store(CacheKeys.Corporate_Portal_LOGIN_MODEL, new CacheItem<LoginModel>(model));
                    }
                    CacheProvider.Store(CacheKeys.Corporate_Portal_USER_DETAILS, new CacheItem<taskDetailsExternalResponse>(response.Payload));
                    return View("~/Views/Feature/Partner/CorporatePartnership/ProfileAvatar.cshtml",new UserModel { Name = response.Payload.userDetails.userName, UserProfilePhoto = response.Payload.userDetails.profile });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The DashboardServiceCount.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult DashboardServiceCount()
        {
            List<DashboardServiceItem> DashboardServiceItemList = new List<DashboardServiceItem>();

            return View("~/Views/Feature/Partner/CorporatePartnership/DashboardServiceCount.cshtml",DashboardServiceItemList);
        }

        /// <summary>
        /// The Inbox.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult Inbox()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                global::Sitecore.Data.Items.Item CPConfig = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.CorporatePortal_CONFIG);
                ServiceResponse<taskDetailsExternalResponse> response = CorportatePortalClient.GetDashboardDetails(model.CP_CoorName, model.CP_PartnerName, CPConfig.Fields["Dashboard Record"].Value);
                if (response.Succeeded)
                {
                    InboxViewModel inboxmodel = new InboxViewModel
                    {
                        sentAndPipeLineTaskResponse = ListofMsg_ids_issues(model.Username, 0, RequestType.RecentMessages),
                        taskDetailsExternalResponse = response.Payload
                    };
                    CacheProvider.Store(CacheKeys.Corporate_Portal_INBOX, new CacheItem<InboxViewModel>(inboxmodel));
                    return View("~/Views/Feature/Partner/CorporatePartnership/Inbox.cshtml");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalDashboard);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The InboxAjax.
        /// </summary>
        /// <param name="sortby">The sortby<see cref="string"/>.</param>
        /// <param name="receivedorsent">The receivedorsent<see cref="string"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <param name="page">The page<see cref="int"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult InboxAjax(string sortby, string receivedorsent, string keyword, int page = 1)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_INBOX, out InboxViewModel model))
            {
                InboxListViewModel InboxListViewModel = new InboxListViewModel
                {
                    page = page
                };
                //if (receivedorsent.Equals("received"))
                //{
                //    if (model.taskDetailsExternalResponse != null && model.taskDetailsExternalResponse.taskDetails != null && model.taskDetailsExternalResponse.taskDetails.Count() > 0)
                //    {
                //        InboxListViewModel.receivedmessage = true;
                //        InboxListViewModel.receivedorsent = receivedorsent;
                //        taskDetailExternalEntity[] taskdetails = model.taskDetailsExternalResponse.taskDetails;
                //        if (!string.IsNullOrWhiteSpace(keyword))
                //        {
                //            taskdetails = taskdetails.Where(x => x.title.ToLower().Contains(keyword.ToLower()) || x.objectName.ToLower().Contains(keyword.ToLower()) || x.taskSubject.ToLower().Contains(keyword.ToLower())).ToArray();
                //        }
                //        if (!string.IsNullOrWhiteSpace(sortby))
                //        {
                //            if (sortby.ToLower().Equals("name"))
                //            {
                //                taskdetails = taskdetails.OrderBy(x => x.title).ToArray();
                //            }
                //        }

                //        IEnumerable<InboxMessage> inboxmessages = taskdetails.Select(x => new InboxMessage
                //        {
                //            messageurl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CorporatePortalMessageDetails) + "?id=" + x.workitem_Id,
                //            messagereadornot = (!string.IsNullOrWhiteSpace(x.task_status) && x.task_status.Equals("acquired") ? "m65-inbox-list--received__mail-read" : string.Empty),
                //            messagesubject = x.taskSubject,
                //            messagetitle = x.title,
                //            messageobjectname = x.objectName,
                //            messagedate = (!string.IsNullOrWhiteSpace(x.date_sent) ? (Convert.ToDateTime(x.date_sent) != null ? Convert.ToDateTime(x.date_sent).ToString("dd MMM") : string.Empty) : string.Empty)
                //        });
                //        InboxListViewModel.totalpage = Pager.CalculateTotalPages(inboxmessages.Count(), 5);
                //        InboxListViewModel.pagination = InboxListViewModel.totalpage > 1 ? true : false;
                //        InboxListViewModel.pagenumbers = InboxListViewModel.totalpage > 1 ? GetPaginationRange(page, InboxListViewModel.totalpage) : new List<int>();
                //        inboxmessages = inboxmessages.Skip((page - 1) * 5).Take(5);
                //        InboxListViewModel.inboxmessage = new JavaScriptSerializer().Serialize(inboxmessages);
                //        return Json(new { status = true, Message = InboxListViewModel }, JsonRequestBehavior.AllowGet);
                //    }
                //}

                if (model.sentAndPipeLineTaskResponse != null && model.sentAndPipeLineTaskResponse.Count > 0)
                {
                    InboxListViewModel.receivedmessage = false;
                    InboxListViewModel.receivedorsent = receivedorsent;
                    List<CP_MSG_IDS_ISS> senttakslist = model.sentAndPipeLineTaskResponse;
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        senttakslist = senttakslist.Where(x => x.Message.ToLower().Contains(keyword.ToLower()) || x.Subject.ToLower().Contains(keyword.ToLower()) || x.RequesterName.ToLower().Contains(keyword.ToLower())).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(sortby))
                    {
                        if (sortby.ToLower().Equals("name"))
                        {
                            senttakslist = senttakslist.OrderBy(x => x.Subject).ToList();
                        }
                    }

                    IEnumerable<InboxMessage> inboxmessages = senttakslist.Select(x => new InboxMessage
                    {
                        messageurl = LinkHelper.GetItemUrl(SitecoreItemIdentifiers.CorporatePortalMessageDetails) + "?objid=" + x.Efolderid,
                        messagerequestername = x.RequesterName,
                        messagedetail = x.Message,
                        messagedate = Foundation.Content.Helpers.DateHelper.ConvertLocalDateFormate(x.CreatedDate, "dd MMM yyyy", "dd MMM yyyy"),
                        messagesubject = x.Subject
                    });
                    InboxListViewModel.totalpage = Pager.CalculateTotalPages(inboxmessages.Count(), 5);
                    InboxListViewModel.pagination = InboxListViewModel.totalpage > 1 ? true : false;
                    InboxListViewModel.pagenumbers = InboxListViewModel.totalpage > 1 ? GetPaginationRange(page, InboxListViewModel.totalpage) : new List<int>();
                    inboxmessages = inboxmessages.Skip((page - 1) * 5).Take(5);
                    InboxListViewModel.inboxmessage = new JavaScriptSerializer().Serialize(inboxmessages);
                    return Json(new { status = true, Message = InboxListViewModel }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = false, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// The HappinessResult.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult HappinessResult()
        {
            return View("~/Views/Feature/Partner/CorporatePartnership/HappinessResult.cshtml");
        }

        /// <summary>
        /// The JointServices.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult JointServices()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                return View("~/Views/Feature/Partner/CorporatePartnership/JointServices.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The JointServicesAjax.
        /// </summary>
        /// <param name="sortby">The sortby<see cref="string"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JointServicesAjax(string sortby, string keyword)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                string error = string.Empty;

                if (!CacheProvider.TryGet(CacheKeys.Corporate_Portal_JointerServices, out jointServicesEntity[] documentlist))
                {
                    ServiceResponse<getDocumentListEntity> response = CorportatePortalClient.GetDocumentListByType(model.CP_CoorName, model.CP_PartnerName, DocParam.jointServices.ToString());
                    if (response.Succeeded)
                    {
                        documentlist = response.Payload != null ? response.Payload.jointservices : null;
                    }
                    else
                    {
                        error = response.Message;
                    }
                }
                //var response = CorportatePortalClient.GetDocumentListByType(model.Username, model.Password,DocParam.jointServices.ToString());
                if (documentlist != null)
                {
                    CacheProvider.Store(CacheKeys.Corporate_Portal_JointerServices, new CacheItem<jointServicesEntity[]>(documentlist));
                    if (!string.IsNullOrWhiteSpace(sortby))
                    {
                        if (sortby.Equals(2))
                        {
                            documentlist = documentlist.ToList().OrderBy(x => x.serviceName).ToArray();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        documentlist = documentlist.ToList().Where(x => (!string.IsNullOrWhiteSpace(x.serviceName) && x.serviceName.ToLower().Contains(keyword.ToLower())) || (!string.IsNullOrWhiteSpace(x.partnerType) && x.partnerType.ToLower().Contains(keyword.ToLower())) || (!string.IsNullOrWhiteSpace(x.divisionName) && x.divisionName.ToLower().Contains(keyword.ToLower()))).ToArray();
                    }
                    return Json(new { status = true, Result = documentlist }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = false, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ProjectsAndInitiativesList.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpGet]
        public ActionResult ProjectsAndInitiativesList()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                return View("~/Views/Feature/Partner/CorporatePartnership/ProjectsAndInitiativesList.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ProjectsAndInitiativesAjax.
        /// </summary>
        /// <param name="sortby">The sortby<see cref="string"/>.</param>
        /// <param name="keyword">The keyword<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ProjectsAndInitiativesAjax(string sortby, string keyword)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                string error = string.Empty;

                if (!CacheProvider.TryGet(CacheKeys.Corporate_Portal_Projectandinitiatives, out projectsAndInitiativesEntity[] documentlist))
                {
                    ServiceResponse<getDocumentListEntity> response = CorportatePortalClient.GetDocumentListByType(model.CP_CoorName, model.CP_PartnerName, DocParam.projectAndInitiatives.ToString());
                    if (response.Succeeded)
                    {
                        documentlist = response.Payload != null ? response.Payload.projects_Initiatives : null;
                    }
                    else
                    {
                        error = response.Message;
                    }
                }
                if (documentlist != null)
                {
                    CacheProvider.Store(CacheKeys.Corporate_Portal_Projectandinitiatives, new CacheItem<projectsAndInitiativesEntity[]>(documentlist));
                    if (!string.IsNullOrWhiteSpace(sortby))
                    {
                        if (sortby.Equals(2))
                        {
                            documentlist = documentlist.ToList().OrderBy(x => x.projectInitiativeName).ToArray();
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(keyword))
                    {
                        documentlist = documentlist.ToList().Where(x => (!string.IsNullOrWhiteSpace(x.projectInitiativeName) && x.projectInitiativeName.ToLower().Contains(keyword.ToLower()))).ToArray();
                    }
                    return Json(new { status = true, Result = documentlist }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { status = false, Message = error }, JsonRequestBehavior.AllowGet);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The MousSlasAgreementsList.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult MousSlasAgreementsList()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                ServiceResponse<getDocumentListEntity> response = CorportatePortalClient.GetDocumentListByType(model.CP_CoorName, model.CP_PartnerName, DocParam.approvedDocuments.ToString());
                if (response.Succeeded)
                {
                    return View("~/Views/Feature/Partner/CorporatePartnership/MousSlasAgreementsList.cshtml",response.Payload);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                return View("~/Views/Feature/Partner/CorporatePartnership/_LoginFormMain.cshtml", new LoginModel());
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The DownloadAttachment.
        /// </summary>
        /// <param name="objectid">The objectid<see cref="string"/>.</param>
        /// <returns>The <see cref="FileResult"/>.</returns>
        [HttpGet]
        public FileResult DownloadAttachment(string objectid)
        {
            if (!string.IsNullOrEmpty(objectid))
            {
                if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
                {
                    ServiceResponse<contentEntity> response = CorportatePortalClient.DownloadContent(model.Username, model.Password, objectid);
                    if (response.Succeeded && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.content) && !string.IsNullOrWhiteSpace(response.Payload.status) && response.Payload.status.Equals("success"))
                    {
                        byte[] bytes = Convert.FromBase64String(response.Payload.content);
                        string type = MimeExtensions.GetExtension(response.Payload.format);
                        return File(bytes, response.Payload.format, response.Payload.objectName + type);
                    }
                    return null;
                }
            }
            return null;
        }

        /// <summary>
        /// The ForwardandReject.
        /// </summary>
        /// <param name="action">The action<see cref="string"/>.</param>
        /// <param name="comments">The comments<see cref="string"/>.</param>
        /// <param name="workid">The workid<see cref="string"/>.</param>
        /// <param name="transitionnamefwd">The transitionnamefwd<see cref="string"/>.</param>
        /// <param name="performernamefwd">The performernamefwd<see cref="string"/>.</param>
        /// <param name="transitionnamerj">The transitionnamerj<see cref="string"/>.</param>
        /// <param name="performernamerj">The performernamerj<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ForwardandReject(string action, string comments, string workid, string transitionnamefwd, string performernamefwd, string transitionnamerj, string performernamerj)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                if (action.Equals("forward"))
                {
                    ServiceResponse<bool> response = CorportatePortalClient.ForwardTaskWithTransitionSelect(model.Username, model.Password, workid, comments, transitionnamefwd, performernamefwd);
                    if (response != null && response.Succeeded)
                    {
                        return Json(new { status = true, payload = response.Payload, Message = "success" });
                    }
                }
                else if (action.Equals("reply"))
                {
                    ServiceResponse<returnTaskResult> response = CorportatePortalClient.ReturnTask(model.Username, model.Password, workid, comments, transitionnamerj, performernamerj);
                    if (response != null && response.Succeeded)
                    {
                        return Json(new { status = true, payload = response.Payload, Message = "success" });
                    }
                }
                return Json(new { status = false, Message = "failed" });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ComposeMessage.
        /// </summary>
        /// <param name="requestername">The requestername<see cref="string"/>.</param>
        /// <param name="requesteremail">The requesteremail<see cref="string"/>.</param>
        /// <param name="subject">The subject<see cref="string"/>.</param>
        /// <param name="message">The message<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ComposeMessage(string requestername, string requesteremail, string subject, string message)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                global::Sitecore.Data.Items.Item CPConfig = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.CorporatePortal_CONFIG);
                string tolist = CPConfig.Fields["To email"].Value;
                /*ServiceResponse<WebApiRestResponseEpass> response = SaveComposemessage(model, requestername, requesteremail, tolist, subject, message, CorporateportalRequests.ComposeMessage);
                if (response != null && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.eFolderId) && response.Payload.eFolderId.Length.Equals(31))
                {
                    return Json(new { status = true, Message = "success" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, (response != null && response.Payload != null && response.Payload.ErrorMessage != null) ? response.Payload.ErrorMessage.ToString() : ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                }*/
                var res = SaveDataToKofax(model, requestername, requesteremail, tolist, subject, message, RequestType.SubmitMessage);

                return Json(new { status = res.Item1, Message = string.Empty });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The SubmitIdeas.
        /// </summary>
        /// <param name="requestername">The requestername<see cref="string"/>.</param>
        /// <param name="requesteremail">The requesteremail<see cref="string"/>.</param>
        /// <param name="subject">The subject<see cref="string"/>.</param>
        /// <param name="message">The message<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult SubmitIdeas(string requestername, string requesteremail, string subject, string message)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                global::Sitecore.Data.Items.Item CPConfig = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.CorporatePortal_CONFIG);
                string tolist = CPConfig.Fields["To email"].Value;
                //ServiceResponse<WebApiRestResponseEpass> response = SaveComposemessage(model, requestername, requesteremail, tolist, subject, message, CorporateportalRequests.SubmitIdeas);

                /*if (response != null && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.eFolderId) && response.Payload.eFolderId.Length.Equals(31))
                {
                    return Json(new { status = true, Message = "success" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, (response != null && response.Payload != null && response.Payload.ErrorMessage != null) ? response.Payload.ErrorMessage.ToString() : ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                }*/
                var res = SaveDataToKofax(model, requestername, requesteremail, tolist, subject, message, RequestType.SubmitIdea);

                return Json(new { status = res.Item1, Message = string.Empty });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The ReportIssues.
        /// </summary>
        /// <param name="requestername">The requestername<see cref="string"/>.</param>
        /// <param name="requesteremail">The requesteremail<see cref="string"/>.</param>
        /// <param name="subject">The subject<see cref="string"/>.</param>
        /// <param name="message">The message<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ReportIssues(string requestername, string requesteremail, string subject, string message)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                global::Sitecore.Data.Items.Item CPConfig = SitecoreX.Database.GetItem(SitecoreItemIdentifiers.CorporatePortal_CONFIG);
                string tolist = CPConfig.Fields["To email"].Value;
                /*ServiceResponse<WebApiRestResponseEpass> response = SaveComposemessage(model, requestername, requesteremail, tolist, subject, message, CorporateportalRequests.ReportIssues);
                if (response != null && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.eFolderId) && response.Payload.eFolderId.Length.Equals(31))
                {
                    return Json(new { status = true, Message = "success" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, (response != null && response.Payload != null && response.Payload.ErrorMessage != null) ? response.Payload.ErrorMessage.ToString() : ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                }*/
                var res = SaveDataToKofax(model, requestername, requesteremail, tolist, subject, message, RequestType.SubmitIssue);

                return Json(new { status = res.Item1, Message = string.Empty });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The MeetingRequest.
        /// </summary>
        /// <param name="requesttype">The requesttype<see cref="string"/>.</param>
        /// <param name="updaterequest">The updaterequest<see cref="string"/>.</param>
        /// <param name="location">The location<see cref="string"/>.</param>
        /// <param name="eventname">The eventname<see cref="string"/>.</param>
        /// <param name="tolist">The tolist<see cref="string"/>.</param>
        /// <param name="fromdate">The fromdate<see cref="string"/>.</param>
        /// <param name="todate">The todate<see cref="string"/>.</param>
        /// <param name="fromtime">The fromtime<see cref="string"/>.</param>
        /// <param name="totime">The totime<see cref="string"/>.</param>
        /// <param name="message">The message<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MeetingRequest(string requesttype, string updaterequest, string location, string eventname, string tolist, string fromdate, string todate, string fromtime, string totime, string message)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                CultureInfo culture = SitecoreX.Culture;
                if (culture.ToString().Equals("ar-AE"))
                {
                    fromdate = CommonUtility.ConvertDateArToEn(fromdate);
                    todate = CommonUtility.ConvertDateArToEn(todate);
                }
                DateTime.TryParse(fromdate, out DateTime fromdateTime);
                DateTime.TryParse(todate, out DateTime todatetime);
                //ServiceResponse<WebApiRestResponseEpass> response = null;
                //RequestType requestType = GetTypeFromString(requesttype,;
                Tuple<bool, string> res = new Tuple<bool, string>(false, string.Empty);
                if (!string.IsNullOrWhiteSpace(updaterequest))
                {
                    res = UpdateMeetingInKofax(updaterequest, model.Username, tolist, message, GetTypeFromString(requesttype), location, eventname, fromdateTime.ToString(CP_DateFormat4SQL), todatetime.ToString(CP_DateFormat4SQL), fromtime, totime);
                    //response = UpdateMeetingrequest(updaterequest, model.Username, requesttype, location, eventname, tolist, fromdateTime, todatetime, fromtime, totime, message);
                }
                else
                {
                    res = SaveMeetingInKofax(model, tolist, message, GetTypeFromString(requesttype), location, eventname, fromdateTime.ToString(CP_DateFormat4SQL), todatetime.ToString(CP_DateFormat4SQL), fromtime, totime);
                    //response = SaveMeetingrequest(model.Username, requesttype, location, eventname, tolist, fromdateTime, todatetime, fromtime, totime, message);
                }
                /*if (response != null && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.eFolderId) && response.Payload.eFolderId.Length.Equals(31))
                {
                    return Json(new { status = true, Message = "success" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, (response.Payload != null && response.Payload.ErrorMessage != null) ? response.Payload.ErrorMessage.ToString() : ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                }*/

                return Json(new { status = res.Item1, Message = res.Item2 });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The UpdateIdeas.
        /// </summary>
        /// <param name="requestid">The requestid<see cref="string"/>.</param>
        /// <param name="updatedmessage">The updatedmessage<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateIdeas(string requestid, string updatedmessage)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                bool status = UpdateIIMMToKofax(requestid, updatedmessage, RequestType.UpdateIdea);
                return Json(new { status, Message = string.Empty });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The Updateissues.
        /// </summary>
        /// <param name="requestid">The requestid<see cref="string"/>.</param>
        /// <param name="updatedmessage">The updatedmessage<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Updateissues(string requestid, string updatedmessage)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                bool status = UpdateIIMMToKofax(requestid, updatedmessage, RequestType.UpdateIssue);
                return Json(new { status, Message = string.Empty });
                /*ServiceResponse<WebApiRestResponseEpass> response = UpdateIdeasIssuesrequest(requestid, updatedmessage, CorporateportalRequests.ReportIssues);
                if (response != null && response.Payload != null && !string.IsNullOrWhiteSpace(response.Payload.eFolderId) && response.Payload.eFolderId.Length.Equals(31))
                {
                    return Json(new { status = true, Message = "success" });
                }
                else
                {
                    ModelState.AddModelError(string.Empty, (response.Payload != null && response.Payload.ErrorMessage != null) ? response.Payload.ErrorMessage.ToString() : ErrorMessages.EFORM__FRONTEND_ERROR_MESSAGE);
                }

                return Json(new { status = false, Message = "failed" });*/
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The CancelMeetingRequest.
        /// </summary>
        /// <param name="requestid">The requestid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CancelMeetingRequest(string requestid)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                var status = CancelMeetingInKofax(requestid, RequestType.CancelIdea);

                return Json(new { status, Message = "failed" });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The CancelIdeas.
        /// </summary>
        /// <param name="requestid">The requestid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult CancelIdeas(string requestid)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                var status = UpdateIIMMToKofax(requestid, "", RequestType.CancelIdea);

                return Json(new { status, Message = "failed" });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The Cancelissues.
        /// </summary>
        /// <param name="requestid">The requestid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Cancelissues(string requestid)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                bool status = UpdateIIMMToKofax(requestid, string.Empty, RequestType.CancelIssue);
                return Json(new { status, Message = string.Empty });
            }
            return Json(new { status = false, islogin = false, Message = CP_LoginInAgain });
        }

        /// <summary>
        /// The SubmittedRequests.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult SubmittedRequests()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                return View("~/Views/Feature/Partner/CorporatePartnership/SubmittedRequests.cshtml",ListofSubmittedrequest(model.Username));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The SubmittedIdeas.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult SubmittedIdeas()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                return View("~/Views/Feature/Partner/CorporatePartnership/SubmittedIdeas.cshtml",ListofMsg_ids_issues(model.Username, 0, RequestType.GetIdeas));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ReportedIssues.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ReportedIssues()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                return View("~/Views/Feature/Partner/CorporatePartnership/ReportedIssues.cshtml",ListofMsg_ids_issues(model.Username, 0, RequestType.GetIssues));
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ProfileInfoForm.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ProfileInfoForm()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                ServiceResponse<externalUserEntity[]> response = CorportatePortalClient.GetUserDetailsexternal(model.CP_CoorName, model.CP_PartnerName);
                if (response != null && response.Succeeded && response.Payload != null && response.Payload.Count() > 0)
                {
                    externalUserEntity externalusermodelresponse = response.Payload.FirstOrDefault();
                    CacheProvider.Store(CacheKeys.Corporate_Portal_MANAGEUSERS, new CacheItem<externalUserEntity>(externalusermodelresponse));
                    return View("~/Views/Feature/Partner/CorporatePartnership/ProfileInfoForm.cshtml",externalusermodelresponse);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                externalUserEntity externalusermodel = null;
                return View("~/Views/Feature/Partner/CorporatePartnership/ProfileInfoForm.cshtml",externalusermodel);
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ContactInfoForm.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult ContactInfoForm()
        {
            externalUserEntity externalusermodel = null;
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_MANAGEUSERS, out externalusermodel))
                {
                    return View("~/Views/Feature/Partner/CorporatePartnership/ContactInfoForm.cshtml",externalusermodel);
                }
                else
                {
                    return View("~/Views/Feature/Partner/CorporatePartnership/ContactInfoForm.cshtml",externalusermodel);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The Users.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult Users()
        {
            return View("~/Views/Feature/Partner/CorporatePartnership/Users.cshtml");
        }

        [HttpGet]
        public ActionResult ChangePasswordForm()
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                return View("~/Views/Feature/Partner/CorporatePartnership/ChangePasswordForm.cshtml");
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The ChangePasswordForm.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangePasswordForm(SetNewPassword setnewpasspword)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                GetPartnerName(model.Username.Trim(), setnewpasspword.OldPassword, out string partnername, out string coordinatorname);

                if (!string.IsNullOrWhiteSpace(partnername) && !string.IsNullOrWhiteSpace(coordinatorname))
                {
                    ChangePassword(model.Username.Trim(), setnewpasspword.ConfirmPassword, out string status);

                    if (!string.IsNullOrEmpty(status) && status == CP_SUCCESS)
                    {
                        ViewBag.success = true;
                        return View("~/Views/Feature/Partner/CorporatePartnership/ChangePasswordForm.cshtml",setnewpasspword);
                    }

                    ModelState.AddModelError(string.Empty, Translate.Text("cpportal_invalidnewpassword"));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("cpportal_invalidcredentials"));
                }

            }
            ViewBag.fail = true;
            return View("~/Views/Feature/Partner/CorporatePartnership/ChangePasswordForm.cshtml",setnewpasspword);
        }

        /// <summary>
        /// The MessageDetail.
        /// </summary>
        /// <param name="id">The id<see cref="string"/>.</param>
        /// <param name="objid">The objid<see cref="string"/>.</param>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        public ActionResult MessageDetail(string objid)
        {
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out LoginModel model))
            {
                //if (!string.IsNullOrWhiteSpace(id))
                //{
                //    ServiceResponse<taskDetailsResponseV2> response = CorportatePortalClient.getTaskDetails(model.Username, model.Password, id);
                //    if (response.Succeeded)
                //    {
                //        ServiceResponse<taskDetailsExternalResponse> dashboardresponse = CorportatePortalClient.GetDashboardDetails(model.Username, model.Password, "All");
                //        if (dashboardresponse != null && dashboardresponse.Succeeded && dashboardresponse.Payload.taskDetails != null)
                //        {
                //            ViewBag.taskdetail = dashboardresponse.Payload.taskDetails.ToList().Where(x => x.workitem_Id.ToLower().Equals(id.ToLower())).FirstOrDefault();
                //            if (ViewBag.taskdetail != null)
                //            {
                //                taskDetailExternalEntity task = (taskDetailExternalEntity)ViewBag.taskdetail;

                //                ViewBag.relateddocumentresponse = CorportatePortalClient.GetRelatedDocuments(model.Username, model.Password, task.objectID).Payload;
                //                return View(response.Payload);
                //            }
                //        }
                //    }
                //    else
                //    {
                //        ModelState.AddModelError(string.Empty, response.Message);
                //    }
                //    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalmyInbox);
                //}
                if (!string.IsNullOrWhiteSpace(objid))
                {
                    List<CP_MSG_IDS_ISS> senttaskresponse = ListofMsg_ids_issues(model.Username, 0, RequestType.GetMessageDetails, objid);
                    if (senttaskresponse != null && senttaskresponse.Count > 0)
                    {
                        return View("~/Views/Feature/Partner/CorporatePartnership/SentMessageDetail.cshtml", senttaskresponse.FirstOrDefault());
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Check the URL"));
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalmyInbox);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.CorporatePortalLogin);
        }

        /// <summary>
        /// The GetSubmittedRequestCount.
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/>.</param>
        /// <param name="eventcount">The eventcount<see cref="string"/>.</param>
        /// <param name="ideascount">The ideascount<see cref="int"/>.</param>
        /// <param name="issuecount">The issuecount<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool GetSubmittedRequestCount(string userid, out string eventcount, out int ideascount, out string issuecount, out string requestscount)
        {
            eventcount = string.Empty; ideascount = 0; issuecount = string.Empty; requestscount = string.Empty;

            KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
            baseKofaxViewModel.Parameters.Add(new Parameter(KofaxConstants.CPVariableGetCountsInput) { Attribute = GetCountsAttributes(userid) });
            ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CPGetCountsPath, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
            LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

            if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
            {
                var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_COUNT_STATUS)).FirstOrDefault();
                if (os != null && os.Attribute.Count() > 0)
                {
                    eventcount = os.Attribute.Where(x => x.Name.Equals(CP_EVENT_COUNT)).FirstOrDefault().Value;
                    int.TryParse(os.Attribute.Where(x => x.Name.Equals(CP_IDEAS_COUNT)).FirstOrDefault().Value, out ideascount);
                    issuecount = os.Attribute.Where(x => x.Name.Equals(CP_ISSUES_COUNT)).FirstOrDefault().Value;
                    requestscount = os.Attribute.Where(x => x.Name.Equals(CP_REQUESTS_COUNT)).FirstOrDefault().Value;
                }
            }
            else
            {
                LogService.Debug(res.Message);
            }

            return true;
        }

        /// <summary>
        /// The ListofSubmittedrequest.
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/>.</param>
        /// <returns>The <see cref="List{SubmittedRequest}"/>.</returns>
        private List<SubmittedRequest> ListofSubmittedrequest(string userid)
        {
            List<SubmittedRequest> lstrequest = new List<SubmittedRequest>();
            var res = GetDataFromKofax(userid, RequestType.GetRequests);

            foreach (var a in res)
                lstrequest.Add(MapResponseToRequestModel(a.Attribute.ToList()));

            return lstrequest;
        }

        /// <summary>
        /// The ListofMsg_ids_issues.
        /// </summary>
        /// <param name="userid">The userid<see cref="string"/>.</param>
        /// <param name="corporateportalRequests">The corporateportalRequests<see cref="CorporateportalRequests"/>.</param>
        /// <returns>The <see cref="List{CP_MSG_IDS_ISS}"/>.</returns>
        private List<CP_MSG_IDS_ISS> ListofMsg_ids_issues(string userid, int rowcount, RequestType requestType, string efolderid = "")
        {
            List<CP_MSG_IDS_ISS> lstrequest = new List<CP_MSG_IDS_ISS>();

            try
            {
                var vals = GetDataFromKofax(userid, requestType);

                switch (requestType)
                {
                    case RequestType.GetIdeas:
                    case RequestType.GetIssues:
                        foreach (var i in vals)
                            lstrequest.Add(MapResponseToModel(i.Attribute.ToList()));
                        break;

                    case RequestType.RecentMessages:
                        foreach (var i in vals)
                        { lstrequest.Add(MapResponseToModel(i.Attribute.ToList())); }
                        lstrequest.OrderByDescending(x => x.Efolderid);
                        if (rowcount > 0) lstrequest = lstrequest.Take(rowcount).ToList();
                        break;
                }

            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return lstrequest;
        }

        /// <summary>
        /// The IsCorportateuserloggedin.
        /// </summary>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool IsCorportateuserloggedin()
        {
            LoginModel model = new LoginModel();
            if (CacheProvider.TryGet(CacheKeys.Corporate_Portal_LOGIN_MODEL, out model))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// The GetBytes.
        /// </summary>
        /// <param name="str">The str<see cref="string"/>.</param>
        /// <returns>The <see cref="byte[]"/>.</returns>
        internal static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// The GetPaginationRange.
        /// </summary>
        /// <param name="currentPage">The currentPage<see cref="int"/>.</param>
        /// <param name="totalPages">The totalPages<see cref="int"/>.</param>
        /// <returns>The <see cref="IEnumerable{int}"/>.</returns>
        private IEnumerable<int> GetPaginationRange(int currentPage, int totalPages)
        {
            const int desiredCount = 5;
            List<int> returnint = new List<int>();

            int start = currentPage - 1;
            int projectedEnd = start + desiredCount;
            if (projectedEnd > totalPages)
            {
                start = start - (projectedEnd - totalPages);
                projectedEnd = totalPages;
            }

            int p = start;
            while (p++ < projectedEnd)
            {
                if (p > 0)
                {
                    returnint.Add(p);
                }
            }
            return returnint;
        }

        /// <summary>
        /// Defines the DocParam.
        /// </summary>
        private enum DocParam
        {
            /// <summary>
            /// Defines the approvedDocuments.
            /// </summary>
            approvedDocuments = 0,

            /// <summary>
            /// Defines the jointServices.
            /// </summary>
            jointServices = 1,

            /// <summary>
            /// Defines the projectAndInitiatives.
            /// </summary>
            projectAndInitiatives = 2
        }

        #region Kofax to replace eforms, helper methods below.

        private const string CP_SUCCESS = "Success";
        private const string CP_OUTPUT_STATUS = "Output_Status";
        private const string CP_RUN_STATUS = "Run_Status";
        private const string CP_PARTNER_NAME = "PartnerName";
        private const string CP_CORD_NAME = "CoordinatorName";
        private const string CP_OUTPUT_COUNT_STATUS = "Output_Count_Status";
        private const string CP_EVENT_COUNT = "CountOfEvent";
        private const string CP_IDEAS_COUNT = "CountOfIdea";
        private const string CP_ISSUES_COUNT = "CountOfIssue";
        private const string CP_REQUESTS_COUNT = "CountOfMeeting";
        private const string CP_TYPE_NAME_UM = "T00034_UserManagment";
        private const string CP_PROCESSING_TYPE = "ProcessingType";
        private const string CP_USERNAME = "UserName";
        private const string CP_DateFormat4SQL = "yyyy-MM-dd";
        private const string CP_LoginInAgain = "Login again";

        private List<Attribute> GetUserAttributes(string username, string password)
        {
            return new List<Attribute>() {
                new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "3" },
                new Attribute(){ Type= kofaxTypeEnum.Text, Name="Username",Value=username }, new Attribute(){ Type= kofaxTypeEnum.Text, Name="Password", Value=password }
            };
        }

        private List<Attribute> ChangePasswordAttributes(string username, string password)
        {
            return new List<Attribute>() {
                new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "2" },
                new Attribute(){ Type= kofaxTypeEnum.Text, Name="Username",Value=username },
                new Attribute(){ Type= kofaxTypeEnum.Text, Name="Password", Value=password },
                new Attribute(){ Type= kofaxTypeEnum.Text, Name="ModifiedBy", Value=password }
            };
        }

        private List<Attribute> GetCountsAttributes(string username)
        {
            return new List<Attribute>() { new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_USERNAME, Value = username } };
        }

        private Tuple<bool, string> SaveDataToKofax(LoginModel loginModel, string requestername, string requesteremail, string tolist, string subject, string message, RequestType requestType)
        {
            Tuple<bool, string> retVal = new Tuple<bool, string>(true, string.Empty);

            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
                string serviceMethod = KofaxConstants.CPIMPathWithRobots;
                string responseVariable = KofaxConstants.CPVariableIdeaList;
                Parameter param = new Parameter("");
                string idField = "ID";
                switch (requestType)
                {
                    case RequestType.SubmitIdea:
                        param.VariableName = responseVariable;
                        //idField = Idea.IdeaID;
                        param.Attribute = new List<Attribute>()
                        {
                        new Attribute() { Type= kofaxTypeEnum.Text, Name=CP_PROCESSING_TYPE, Value="1" },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.UserName, Value=loginModel.Username },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.RequestorName, Value=requestername },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.FromMail, Value=requesteremail },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.ToMail, Value=tolist },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Msg_subject, Value=subject },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Message, Value=message },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.CreatedBy, Value=requestername },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnerid, Value=loginModel.CP_PartnerID },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnername, Value=loginModel.CP_PartnerName }
                    };

                        break;

                    case RequestType.SubmitIssue:
                        serviceMethod = KofaxConstants.IMRequestPath;
                        param.VariableName = responseVariable = KofaxConstants.IMVariableIssueManagement;
                        //idField = Issue.IssueID;
                        param.Attribute = new List<Attribute>()
                        {
                        new Attribute() { Type= kofaxTypeEnum.Text, Name=CP_PROCESSING_TYPE, Value="1" },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.UserName, Value=loginModel.Username },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.RequestorName, Value=requestername },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.FromMail, Value=requesteremail },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.ToMail, Value=tolist },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Msg_subject, Value=subject },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Message, Value=message },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.CreatedBy, Value=requestername },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnerid, Value=loginModel.CP_PartnerID },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnername, Value=loginModel.CP_PartnerName }
                    };
                        break;

                    case RequestType.SubmitMessage:
                        serviceMethod = KofaxConstants.MMRequestPath;
                        param.VariableName = responseVariable = KofaxConstants.MMVariableMessageManagement;
                        //idField = Issue.IssueID;
                        param.Attribute = new List<Attribute>()
                        {
                        new Attribute() { Type= kofaxTypeEnum.Text, Name=CP_PROCESSING_TYPE, Value="1" },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.UserName, Value=loginModel.Username },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.RequestorName, Value=requestername },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.FromMail, Value=requesteremail },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.ToMail, Value=tolist },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Msg_subject, Value=subject },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Message, Value=message },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.CreatedBy, Value=requestername },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnerid, Value=loginModel.CP_PartnerID },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnername, Value=loginModel.CP_PartnerName } };
                        break;
                }
                baseKofaxViewModel.Parameters.Add(param);

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(serviceMethod, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload?.RobotError == null && res?.Payload.Values != null && res?.Payload.Values.Length > 0 && res?.Payload.Values[0].Attribute != null && res?.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        //var ut = res.Payload.Values.Where(x => x.TypeName.Equals(KofaxConstants.CPVariableIdeaList)).FirstOrDefault();
                        string newID = os.Attribute.Where(x => x.Name.Equals(idField)).FirstOrDefault().Value;

                        return new Tuple<bool, string>(string.IsNullOrEmpty(newID) ? false : true, string.Empty);
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this); return new Tuple<bool, string>(false, ex.Message);
            }

            return retVal;
        }

        private Tuple<bool, string> SaveMeetingInKofax(LoginModel loginModel, string tolist, string message, RequestType requestType, string location, string eventname, string fromdate, string todate, string fromtime, string totime)
        {
            Tuple<bool, string> retVal = new Tuple<bool, string>(true, string.Empty);

            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
                //string serviceMethod = KofaxConstants.CPIMPathWithRobots;
                //string responseVariable = KofaxConstants.CPVariableIdeaList;
                Parameter param = new Parameter(KofaxConstants.CPVariableRequestManagement);
                //string idField = "";
                string rtype = "";
                switch (requestType)
                {
                    case RequestType.SubmitMeetingRequest:
                        rtype = "M"; break;
                    case RequestType.SubmitVisitRequest:
                        rtype = "V"; break;
                    case RequestType.SubmitBenchmarkRequest:
                        rtype = "B"; break;

                        /*
                {                   "type": "text",                    "name": "ProcessingType",                    "value": "1"                },
                {                    "type": "text",                    "name": "UserName",                    "value": "CP000001"                },
                {                    "type": "text",                    "name": "Partnerid",                    "value": "1"                },
                {                    "type": "text",                    "name": "Partnername",                    "value": "Mahmoud"                },
                {                    "type": "text",                    "name": "RequestorName",                    "value": "Terrain Okle"                },
                {                    "type": "text",                    "name": "FromMail",                    "value": "altaf.ahmed@dewa.gov.ae"                },
                {                   "type": "text",                    "name": "ToMail",                    "value": "altaf.ahmed@dewa.gov.ae"                },
                {                    "type": "text",                    "name": "Msg_subject",                    "value": "Test Meeting3"                },
                {                    "type": "text",                    "name": "Message",                    "value": "The message goes along Meeting3"                },
                {                    "type": "text",                    "name": "EVENTREQUESTTYPE",                    "value": "M"                },
                {                    "type": "text",                    "name": "LOCATION",                    "value": "HUDAIBA"                },
                {                    "type": "text",                    "name": "EVENTNAME",                    "value": "Meeting Event"                },
                {                    "type": "text",                    "name": "Fromdate",                    "value": "2021-07-15 07:59:59.999"                },
                {                    "type": "text",                    "name": "Todate",                    "value": "2021-07-15 16:59:59.999"                },
                {                    "type": "text",                    "name": "Fromtime",                    "value": "08:00.000"                },
                {                    "type": "text",                    "name": "Totime",                    "value": "17:00.000"                },
                {                    "type": "text",                    "name": "CreatedBy",                    "value": "Altaf.ahmed"                }
                {                    "name": "FromMail",                    "type": "text",                    "value": "Mahmoud.alyabroudi@dewa.gov.ae"                },
                {                    "name": "Partnerid",                    "type": "text",                    "value": "1"                },
                {                    "name": "Partnername",                    "type": "text",                    "value": "Mahmoud"                },
                {                    "name": "RequestorName",                    "type": "text",                    "value": "alyabroudi"                }
                     * */
                }
                //serviceMethod = KofaxConstants.IMRequestPath;
                //param.VariableName = KofaxConstants.IMVariableIssueManagement;

                //idField = Meeting.MeetingID;
                param.Attribute = new List<Attribute>()
                        {
                        new Attribute() { Type= kofaxTypeEnum.Text, Name=CP_PROCESSING_TYPE, Value="1" },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.UserName, Value=loginModel.Username },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.RequestorName, Value=loginModel.Username },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.FromMail, Value="noreply@dewa.gov.ae" },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.ToMail, Value=tolist },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Msg_subject, Value=eventname },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Message, Value=message },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.CreatedBy, Value=loginModel.Username },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnerid, Value=loginModel.CP_PartnerID},
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnername, Value=loginModel.CP_PartnerName},
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.LOCATION, Value=location },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.FromDate, Value=fromdate },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.ToDate, Value=todate },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.FromTime, Value=fromtime },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.ToTime, Value=totime },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.EventName, Value=eventname },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.EventRequestType, Value=rtype }
                        };

                baseKofaxViewModel.Parameters.Add(param);

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CPRequestsPath, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload?.RobotError == null && res?.Payload.Values != null && res?.Payload.Values.Length > 0 && res?.Payload.Values[0].Attribute != null && res?.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        //var ut = res.Payload.Values.Where(x => x.TypeName.Equals(KofaxConstants.CPVariableRequestManagement)).FirstOrDefault();
                        string newID = os.Attribute.Where(x => x.Name.Equals("ID")).FirstOrDefault().Value;

                        return new Tuple<bool, string>(string.IsNullOrEmpty(newID) ? false : true, string.Empty);
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this); return new Tuple<bool, string>(false, ex.Message);
            }

            return retVal;
        }

        private Tuple<bool, string> UpdateMeetingInKofax(string meetingid, string username, string tolist, string message, RequestType requestType, string location, string eventname, string fromdate, string todate, string fromtime, string totime)
        {
            Tuple<bool, string> retVal = new Tuple<bool, string>(false, string.Empty);

            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();

                Parameter param = new Parameter(KofaxConstants.CPVariableRequestManagement);
                var rtype = "";
                switch (requestType)
                {
                    case RequestType.SubmitMeetingRequest:
                        rtype = "M"; break;
                    case RequestType.SubmitVisitRequest:
                        rtype = "V"; break;
                    case RequestType.SubmitBenchmarkRequest:
                        rtype = "B"; break;
                }
                param.Attribute = new List<Attribute>()
                        {
                        new Attribute() { Type= kofaxTypeEnum.Text, Name=CP_PROCESSING_TYPE, Value="2" },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.MeetingID, Value=meetingid },
                        //new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.RequestorName, Value=requestername },
                        //new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.FromMail, Value=requesteremail },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.ToMail, Value=tolist },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Msg_subject, Value=eventname },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Message, Value=message },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.ModifiedBy, Value=username },
                        //new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnerid, Value=loginModel.CP_PartnerID },
                        //new Attribute() { Type= kofaxTypeEnum.Text, Name= Base.Partnername, Value=loginModel.CP_PartnerName },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.LOCATION, Value=location },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.FromDate, Value=fromdate },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.ToDate, Value=todate },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.FromTime, Value=fromtime },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.ToTime, Value=totime },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.EventName, Value=eventname },
                        new Attribute() { Type= kofaxTypeEnum.Text, Name= Meeting.EventRequestType, Value=rtype }
                        };

                baseKofaxViewModel.Parameters.Add(param);

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CPRequestsPath, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload?.RobotError == null && res?.Payload.Values != null && res?.Payload.Values.Length > 0 && res?.Payload.Values[0].Attribute != null && res?.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        //var ut = res.Payload.Values.Where(x => x.TypeName.Equals(KofaxConstants.CPVariableRequestManagement)).FirstOrDefault();
                        //string newID = ut.Attribute.Where(x => x.Name.Equals(idField)).FirstOrDefault().Value;

                        return new Tuple<bool, string>(true, string.Empty);
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this); return new Tuple<bool, string>(false, ex.Message);
            }

            return retVal;
        }

        private Tuple<bool, string> CancelMeetingInKofax(string meetingid, RequestType requestType)
        {
            Tuple<bool, string> retVal = new Tuple<bool, string>(false, string.Empty);

            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();

                Parameter param = new Parameter(KofaxConstants.CPVariableRequestManagement);
                /*var rtype = "";
                switch (requestType)
                {
                    case RequestType.SubmitMeetingRequest:
                        rtype = "M"; break;
                    case RequestType.SubmitVisitRequest:
                        rtype = "V"; break;
                    case RequestType.SubmitBenchmarkRequest:
                        rtype = "B"; break;
                }*/
                param.Attribute = new List<Attribute>
                        {
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "3" },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Meeting.MeetingID, Value = meetingid },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.ModifiedBy, Value = string.IsNullOrEmpty(CurrentPrincipal.UserId) ? (string.IsNullOrEmpty(CurrentPrincipal.Username) ? "" : CurrentPrincipal.Username) : CurrentPrincipal.UserId }
                        };

                baseKofaxViewModel.Parameters.Add(param);

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(KofaxConstants.CPRequestsPath, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload?.RobotError == null && res?.Payload.Values != null && res?.Payload.Values.Length > 0 && res?.Payload.Values[0].Attribute != null && res?.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        //var ut = res.Payload.Values.Where(x => x.TypeName.Equals(KofaxConstants.CPVariableRequestManagement)).FirstOrDefault();
                        //string newID = ut.Attribute.Where(x => x.Name.Equals(idField)).FirstOrDefault().Value;

                        return new Tuple<bool, string>(true, string.Empty);
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this); return new Tuple<bool, string>(false, ex.Message);
            }

            return retVal;
        }

        private bool UpdateIIMMToKofax(string recordId, string message, RequestType requestType)
        {
            //Tuple<bool, string> retVal = new Tuple<bool, string>(false, string.Empty);

            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
                string serviceMethod = KofaxConstants.CPIMPathWithRobots;
                string responseVariable = KofaxConstants.CPVariableIdeaList;
                Parameter param = new Parameter(responseVariable);
                switch (requestType)
                {
                    case RequestType.UpdateIdea:
                        //serviceMethod = KofaxConstants.CPIMPathWithRobots;
                        //responseVariable = KofaxConstants.CPVariableIdeaList;

                        param.Attribute = new List<Attribute>
                        {
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "2" },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Idea.IdeaID, Value = recordId },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.Message, Value = message },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.ModifiedBy, Value = string.IsNullOrEmpty(CurrentPrincipal.UserId) ? (string.IsNullOrEmpty(CurrentPrincipal.Username) ? "" : CurrentPrincipal.Username) : CurrentPrincipal.UserId }
                        };

                        // baseKofaxViewModel.Parameters.Add(param);
                        break;

                    case RequestType.CancelIdea:
                        param.Attribute = new List<Attribute>
                        {
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "3" },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Idea.IdeaID, Value = recordId },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.ModifiedBy, Value = string.IsNullOrEmpty(CurrentPrincipal.UserId) ? (string.IsNullOrEmpty(CurrentPrincipal.Username) ? "" : CurrentPrincipal.Username) : CurrentPrincipal.UserId }
                        };

                        //baseKofaxViewModel.Parameters.Add(param);
                        break;

                    case RequestType.UpdateIssue:
                        serviceMethod = KofaxConstants.IMRequestPath;
                        responseVariable = param.VariableName = KofaxConstants.IMVariableIssueManagement;

                        param.Attribute = new List<Attribute>
                        {
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "2" },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Issue.IssueID, Value = recordId },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.Message, Value = message },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.ModifiedBy, Value = string.IsNullOrEmpty(CurrentPrincipal.UserId) ? (string.IsNullOrEmpty(CurrentPrincipal.Username) ? "" : CurrentPrincipal.Username) : CurrentPrincipal.UserId }
                        };

                        //baseKofaxViewModel.Parameters.Add(param);
                        break;

                    case RequestType.CancelIssue:
                        serviceMethod = KofaxConstants.IMRequestPath;
                        responseVariable = param.VariableName = KofaxConstants.IMVariableIssueManagement;

                        param.Attribute = new List<Attribute>
                        {
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "3" },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Issue.IssueID, Value = recordId },
                            //new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.Message, Value = message },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.ModifiedBy, Value = string.IsNullOrEmpty(CurrentPrincipal.UserId) ? (string.IsNullOrEmpty(CurrentPrincipal.Username) ? "" : CurrentPrincipal.Username) : CurrentPrincipal.UserId }
                        };

                        break;
                }

                baseKofaxViewModel.Parameters.Add(param);

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(serviceMethod, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload?.RobotError == null && res?.Payload.Values != null && res?.Payload.Values.Length > 0 && res?.Payload.Values[0].Attribute != null && res?.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        //var ut = res.Payload.Values.Where(x => x.TypeName.Equals(KofaxConstants.CPVariableIdeaList)).FirstOrDefault();
                        //string ideaID = ut.Attribute.Where(x => x.Name.Equals(Idea.IdeaID)).FirstOrDefault().Value;

                        return true;
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }

            return false;
        }

        private CP_MSG_IDS_ISS MapResponseToModel(List<DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute> attributes)
        {
            CP_MSG_IDS_ISS m = new CP_MSG_IDS_ISS() { RequesterName = "NA", CancelledRequest = false, updatedMessages = new List<string>(), strupdatedMessages = "", updateddates = new List<string>(), strupdateddates = "" };
            foreach (var v in attributes)
            {
                switch (v.Name)
                {
                    case Base.FromMail:
                        m.fromemail = v.Value;
                        break;

                    case Message.MessageID:
                        m.Efolderid = v.Value;
                        break;

                    case Idea.IdeaID:
                        m.Efolderid = v.Value;
                        break;

                    case Base.Message:
                        m.Message = v.Value;
                        m.Ideas = v.Value;
                        m.Issues = v.Value;
                        break;

                    case Base.Msg_subject:
                        m.Subject = v.Value;
                        break;

                    case Base.Partnerid:
                        m.Partnerid = v.Value;
                        break;

                    case Base.Partnername:
                        m.Partnername = v.Value;
                        break;

                    case Base.CreationDate:
                        DateTime dt = DateTime.Now;
                        if (DateTime.TryParse(v.Value, out dt)) { m.CreatedDate = dt.ToString("dd MMM yyyy"); }
                        break;

                    case Base.UserName:
                        m.Userid = v.Value;
                        break;

                    case Issue.IssueID:
                        m.Efolderid = v.Value;
                        break;

                    case Base.RequestorName:
                        m.RequesterName = v.Value;
                        break;

                    case Base.CreatedBy:

                        break;
                }
            }
            return m;
        }

        private SubmittedRequest MapResponseToRequestModel(List<DEWAXP.Foundation.Integration.Responses.KofaxRest.Attribute> attributes)
        {
            SubmittedRequest m = new SubmittedRequest();
            DateTime dt = DateTime.Now;
            foreach (var v in attributes)
            {
                switch (v.Name)
                {
                    case Meeting.MeetingID:
                        m.Efolderid = v.Value;
                        break;

                    case Base.Message:
                        m.Message = v.Value;
                        break;
                    /*case Base.Msg_subject:
                        m.msg.Eventname = v.Value;
                        break;*/
                    case Meeting.EventName:
                        m.Eventname = v.Value;
                        break;

                    case Meeting.FromDate:
                        if (DateTime.TryParse(v.Value, out dt)) { m.FromDate = dt.ToString("dd MMM yyyy"); m.Month = dt.ToString("MMM yyyy"); m.Date = dt.Day.ToString(); }
                        break;

                    case Meeting.ToDate:
                        if (DateTime.TryParse(v.Value, out dt)) { m.ToDate = dt.ToString("dd MMM yyyy"); }
                        break;

                    case Meeting.FromTime:
                        m.FromTime = v.Value;
                        break;

                    case Meeting.ToTime:
                        m.ToTime = v.Value;
                        break;

                    case Meeting.LOCATION:
                        m.Location = v.Value;
                        break;

                    case Meeting.EventRequestType:
                        m.RequestType = GetTypeLongString(v.Value);
                        break;

                    case Base.ToMail:
                        m.ToList = v.Value;
                        break;
                        /*
                         Efolderid = x.EFOLDERID,
                       RequestType = x.EVENTREQUESTTYPE,
                       Eventname = x.EVENTNAME,
                       Location = x.LOCATION,
                       Month = (x.FROMDATE != null) ? x.FROMDATE.ToString("MMM yyyy") : string.Empty,
                       Date = (x.FROMDATE != null) ? x.FROMDATE.ToString("dd") : string.Empty,
                       FromTime = x.FROMTIME,
                       ToTime = x.TOTIME,
                       ToList = x.ATTENDEESEMAILID,
                       FromDate = (x.FROMDATE != null) ? x.FROMDATE.ToString("dd MMM yyyy") : string.Empty,
                       ToDate = (x.TODATE != null) ? x.TODATE.ToString("dd MMM yyyy") : string.Empty,
                       Message = x.MESSAGE,
                       CancelledRequest = x.STATUS != null ? x.STATUS.ToString().Equals("Canceled") : false,
                        */
                }
            }
            return m;
        }

        private List<Value> GetDataFromKofax(string userid, RequestType requestType)
        {
            try
            {
                KofaxBaseViewModel baseKofaxViewModel = new KofaxBaseViewModel();
                string serviceMethod = string.Empty;
                string responseVariable = string.Empty;
                Parameter param = new Parameter("");
                switch (requestType)
                {
                    case RequestType.GetIdeas:
                        serviceMethod = KofaxConstants.CPIMPathWithRobots;
                        responseVariable = param.VariableName = KofaxConstants.CPVariableIdeaList;

                        //baseKofaxViewModel.Parameters.Add(param);
                        break;

                    case RequestType.GetIssues:
                        serviceMethod = KofaxConstants.IMRequestPath;
                        responseVariable = param.VariableName = KofaxConstants.IMVariableIssueManagement;

                        //baseKofaxViewModel.Parameters.Add(param);
                        break;

                    case RequestType.GetRequests:
                        serviceMethod = KofaxConstants.CPRequestsPath;
                        responseVariable = param.VariableName = KofaxConstants.CPVariableRequestManagement;

                        break;
                        //case RequestType.get
                }

                param.Attribute = new List<Attribute>
                        {
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = CP_PROCESSING_TYPE, Value = "4" },
                            new Attribute() { Type = kofaxTypeEnum.Text, Name = Base.UserName, Value = userid }
                        };

                baseKofaxViewModel.Parameters.Add(param);

                ServiceResponse<KofaxRestResponse> res = KofaxRESTService.SubmitKofax(serviceMethod, JsonConvert.SerializeObject(baseKofaxViewModel, Converter.Settings));
#if DEBUG
                LogService.Info(new System.Exception(res.Message) { Source = JsonConvert.SerializeObject(res) });
#endif

                if (res.Succeeded && res.Payload?.RobotError == null && res?.Payload.Values != null && res?.Payload.Values.Length > 0 && res?.Payload.Values[0].Attribute != null && res?.Payload.Values[0].Attribute.Length > 0)
                {
                    var os = res.Payload.Values.Where(x => x.TypeName.Equals(CP_OUTPUT_STATUS)).FirstOrDefault();
                    if (os != null && os.Attribute.Where(x => x.Name.Equals(CP_RUN_STATUS) && x.Value.Equals(CP_SUCCESS)).Any())
                    {
                        return res.Payload.Values.Where(x => x.TypeName.Equals(responseVariable)).ToList();
                        //partnername = ut.Attribute.Where(x => x.Name.Equals(CP_PARTNER_NAME)).FirstOrDefault().Value;
                        //coordinatorname = ut.Attribute.Where(x => x.Name.Equals(CP_CORD_NAME)).FirstOrDefault().Value;
                        /*switch (corporateportalRequests)
                        {
                            case CorporateportalRequests.SubmitIdeas:
                                foreach (var i in ut)
                                    lstrequest.Add(MapResponseToModel(i.Attribute.ToList()));
                                goto jumpHere;
                        }*/
                    }
                }
                else
                {
                    LogService.Debug(res.Message);
                }

                List<DEWAXP.Foundation.Integration.Requests.RestServiceRequestParam> SqlParams = new List<DEWAXP.Foundation.Integration.Requests.RestServiceRequestParam>();
            }
            catch (Exception ex)
            {
                LogService.Fatal(ex, this);
            }
            return new List<Value>();
        }

        private enum RequestType
        {
            GetIdeas, SubmitIdea, UpdateIdea, CancelIdea, GetIssues, SubmitIssue, UpdateIssue, CancelIssue, GetRequests, SubmitBenchmarkRequest, UpdateBenchmarkRequest, SubmitVisitRequest, UpdateVisitRequest, GetCounts, GetUser, SubmitMessage, RecentMessages, GetMessageDetails, SubmitMeetingRequest, UpdateMeetingRequest, SubmitEvent
        }

        private RequestType GetTypeFromString(string type, bool isUpdateRequest = false)
        {
            switch (type)
            {
                case MeetingRequestTypeLong.Benchmark:
                    return isUpdateRequest ? RequestType.UpdateBenchmarkRequest : RequestType.SubmitBenchmarkRequest;

                case MeetingRequestTypeLong.Visit:
                    return isUpdateRequest ? RequestType.UpdateVisitRequest : RequestType.SubmitVisitRequest;

                default: //"Meeting Request":
                    return isUpdateRequest ? RequestType.UpdateMeetingRequest : RequestType.SubmitMeetingRequest;
            }
        }

        private string GetTypeLongString(string type)
        {
            switch (type)
            {
                case MeetingRequestType.Benchmark:
                    return MeetingRequestTypeLong.Benchmark;

                case MeetingRequestType.Visit:
                    return MeetingRequestTypeLong.Visit;

                default:
                    return MeetingRequestTypeLong.Meeting;
            }
        }

        #endregion Kofax to replace eforms, helper methods below.
    }
}