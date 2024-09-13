// <copyright file="MiscellaneousController.cs">
// Copyright (c) 2018
// </copyright>
// <author>DEWA\sivakumar.r</author>

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    using DEWAXP.Foundation.Content;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Filters.Mvc;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.APIHandler.Impl;
    using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar;
    using DEWAXP.Foundation.Integration.DewaSvc;
    using DEWAXP.Foundation.Integration.Responses;
    using DEWAXP.Foundation.Logger;
    using global::Sitecore;
    using global::Sitecore.Globalization;
    using Models.Miscellaneous;
    using Sitecore.ContentSearch.ComputedFields;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    using Sitecorex = global::Sitecore;

    /// <summary>
    /// Defines the <see cref="MiscellaneousController" />
    /// </summary>
    public class MiscellaneousController : BaseController
    {
        /// <summary>
        /// Defines the MiscellaneousActivity
        /// </summary>
        private enum MiscellaneousActivity
        {
            /// <summary>
            /// Defines the Step1
            /// </summary>
            Step1 = 0001,

            /// <summary>
            /// Defines the Step2
            /// </summary>
            Step2 = 0002,
        }

        /// <summary>
        /// Defines the MiscellaneousCode
        /// </summary>
        private enum MiscellaneousCode
        {
            /// <summary>
            /// Defines the MeterTestingProject
            /// </summary>
            MeterTestingProject = 0001,

            /// <summary>
            /// Defines the MeterTestingNewconnection
            /// </summary>
            MeterTestingNewconnection = 0002,

            /// <summary>
            /// Defines the OilTesting
            /// </summary>
            OilTesting = 0003,

            /// <summary>
            /// Defines the DemineralizedWater
            /// </summary>
            DemineralizedWater = 0004,

            /// <summary>
            /// Defines the JointerTesting
            /// </summary>
            JointerTesting = 0005,
        }

        #region TrackMiscellaenous

        [HttpGet]
        public ActionResult TrackMiscellaneous(string AppType, string AppNo)
        {
            try
            {

                /*
                 
                  "bpnumber":"6008371",
            "lang":"EN",
            "sessionid":"EPAY0EE633766A842182E1",           
            "userid":"DORMANSMITH",                          
                 
                 */
                if(string.IsNullOrEmpty(CurrentPrincipal.SessionToken) && string.IsNullOrEmpty(AppNo))
                    return PartialView("~/Views/Feature/GeneralServices/Miscellaneous/TrackMiscellaneous.cshtml", new TrackMiscellaneousModel());

                

                TrackMiscellaneousinput credReq = new TrackMiscellaneousinput()
                {
                    bpnumber = !string.IsNullOrEmpty(CurrentPrincipal.BusinessPartner) ? CurrentPrincipal.BusinessPartner : string.Empty,
                    sessionid = !string.IsNullOrEmpty(CurrentPrincipal.SessionToken) ? CurrentPrincipal.SessionToken : string.Empty,
                    userid = !string.IsNullOrEmpty(CurrentPrincipal.UserId) ? CurrentPrincipal.UserId : string.Empty,
                    notificationnumber = !string.IsNullOrEmpty(AppNo) ? AppNo : string.Empty
                };

                var response = MasarClient.MasarMiscellaneousTrackApplication(new MasarTrackMiscellaneousRequest()
                {
                    trackinput = credReq
                }, RequestLanguage, Request.Segment());

                if (response != null && response.Succeeded && response.Payload != null)
                {
                    if (response.Payload.trackdetailslist != null && response.Payload.trackdetailslist.Count > 0)
                    {
                        var filteredResults = response.Payload.trackdetailslist.Where(x => x.partofobjects == AppType);
                        var model = new TrackMiscellaneousModel();
                        model.ApplicationNO = !string.IsNullOrEmpty(AppNo) ? AppNo : string.Empty;

                        List<TrackMiscellaneousDetails> trkMisc = new List<TrackMiscellaneousDetails>();
                       
                        foreach (var item in filteredResults)
                        {
                            trkMisc.Add(new TrackMiscellaneousDetails()
                            {
                                CodeGroup = item.codegroup,
                                CodeGroupCoding = item.codegroupcoding,
                                CodeText = item.codetext,
                                Coding = item.coding,
                                CreateDate = item.createdate,
                                CustomerNumber = item.customernumber,
                                GeneralFlag = item.generalflag,
                                NotificationCompletedFlag = item.notificationcompletedflag,
                                NotificationNumber = item.notificationnumber,
                                NotificationTime = item.notificationtime,
                                NotificationType = item.notificationtype,
                                NotificationtyPetext = item.notificationtypetext,
                                PartOfObjects = item.partofobjects,
                                ShortText = item.shorttext,
                                Status =  !string.IsNullOrEmpty(item.status) ? item.status : string.Empty,
                                StatusDate = item.statusdate,
                                StatusTime = item.statustime                               

                            });
                        
                        }
                        model.trackMiscellaneousDetails = trkMisc;
                       
                        return PartialView("~/Views/Feature/GeneralServices/Miscellaneous/TrackMiscellaneous.cshtml", model);
                    }                    
                }
                else
                {
                    //ModelState.AddModelError(string.Empty, response.Message);
                    ModelState.AddModelError(string.Empty, Translate.Text("Webservice Error"));
                }


            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                // ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                ModelState.AddModelError(string.Empty, Translate.Text("Webservice Error"));
            }


            return PartialView("~/Views/Feature/GeneralServices/Miscellaneous/TrackMiscellaneous.cshtml", new TrackMiscellaneousModel());

        }


        #endregion


        #region MeterTestingProjects

        /// <summary>
        /// The MeterTestingProjects
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult MeterTestingProjects()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            miscellaneousInput input = new miscellaneousInput
            {
                activity = ((int)MiscellaneousActivity.Step1).ToString("D4"),
                businesspartnernumber = CurrentPrincipal.BusinessPartner,
                code = ((int)MiscellaneousCode.MeterTestingProject).ToString("D4"),
                codegroup = "MISC.SER",
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
                notificationinput = new notification
                {
                    customernumber = CurrentPrincipal.BusinessPartner,
                    codegroup = "MISC.SER",
                    credential = CurrentPrincipal.SessionToken,
                    coding = ((int)MiscellaneousCode.MeterTestingProject).ToString("D4"),
                }
            };
            var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
            if (request.Succeeded && request.Payload != null)
            {
                MeterTestingProject model = new MeterTestingProject
                {
                    Materials = request.Payload.notificationoutput.codegrouplist,
                    // BusinessPartners = GetBusinessPartners()
                    BusinessPartner = CurrentPrincipal.BusinessPartner,
                    BusinessPartnerDisplay = CurrentPrincipal.BusinessPartner + "-" + CurrentPrincipal.Name

                };
                CacheProvider.Store(CacheKeys.Miscellaneous_MeterTestingProjects, new AccessCountingCacheItem<MeterTestingProject>(model, Times.Once));
                return View("~/Views/Feature/GeneralServices/Miscellaneous/MeterTestingProjects.cshtml", model);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The MeterTestingProjects
        /// </summary>
        /// <param name="model">The model<see cref="MeterTestingProject"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MeterTestingProjects(MeterTestingProject model)
        {
            MeterTestingProject cachemodel;
            if (CacheProvider.TryGet(CacheKeys.Miscellaneous_MeterTestingProjects, out cachemodel))
            {
                var list = CustomJsonConvertor.DeserializeObject<List<notificationActivities>>(model.selectedMaterialJSON);
                list = listNotification(list, cachemodel.Materials, "MISC-MTP", 1, 99);
                string error = string.Empty;
                var Attachment = new byte[0];
                if (model.Attachment1 != null && model.Attachment1.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment1, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_PROJECT);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment1.InputStream.CopyTo(memoryStream);
                            Attachment = memoryStream.ToArray();
                        }
                    }
                }
                if (list.Count > 0)
                {
                    miscellaneousInput input = new miscellaneousInput
                    {
                        activity = ((int)MiscellaneousActivity.Step2).ToString("D4"),
                        attachflag = model.Attachment1 != null ? "X" : string.Empty,
                        businesspartnernumber = model.BusinessPartner,
                        code = ((int)MiscellaneousCode.MeterTestingProject).ToString("D4"),
                        codegroup = "MISC.SER",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,

                        notificationinput = new notification
                        {
                            attachments = model.Attachment1 == null ? null : new miscellaneousAttachment[]
                            {
                            new miscellaneousAttachment
                            {
                            content = Attachment,
                            contenttype = model.Attachment1.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment1.FileName.GetFileNameWithoutPath()
                        }
                            },
                            customernumber = model.BusinessPartner,
                            codegroup = "MISC.SER",
                            credential = CurrentPrincipal.SessionToken,
                            coding = ((int)MiscellaneousCode.MeterTestingProject).ToString("D4"),
                            comments = new comments[]
                            {
                            new comments
                            {
                                textline = model.Remarks
                            }
                            },
                            deviceid = model.SubstationNumber,
                            notifactivities = list.ToArray(),
                            purchaseordernumber = model.PurchaseOrderNumber
                        }
                    };
                    var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
                    if (request.Succeeded && request.Payload != null && request.Payload.notificationoutput != null && !string.IsNullOrWhiteSpace(request.Payload.notificationoutput.notificationnumber))
                    {
                        CacheProvider.Store(CacheKeys.Miscellaneous_Success, new AccessCountingCacheItem<string>(request.Payload.notificationoutput.notificationnumber, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_PROJECT_SUCCESS);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(request.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_PROJECT);
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("MeterTestingproject.Error"), Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_PROJECT);
        }

        #endregion MeterTestingProjects

        #region MeterTestingNewconnection

        /// <summary>
        /// The MeterTestingNewconnection
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
       // [HttpGet, TwoPhaseAuthorize]
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult MeterTestingNewconnection()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            miscellaneousInput input = new miscellaneousInput
            {
                activity = ((int)MiscellaneousActivity.Step1).ToString("D4"),
                businesspartnernumber = CurrentPrincipal.BusinessPartner,
                code = ((int)MiscellaneousCode.MeterTestingNewconnection).ToString("D4"),
                codegroup = "MISC.SER",
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
                notificationinput = new notification
                {
                    customernumber = CurrentPrincipal.BusinessPartner,
                    codegroup = "MISC.SER",
                    credential = CurrentPrincipal.SessionToken,
                    coding = ((int)MiscellaneousCode.MeterTestingNewconnection).ToString("D4"),
                }
            };
            var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
            if (request.Succeeded && request.Payload != null)
            {
                MeterTestingNewconnection model = new MeterTestingNewconnection
                {
                    Materials = request.Payload.notificationoutput.codegrouplist,
                    //BusinessPartners = GetBusinessPartners(), This is commented because of S4hana ISU Changes for fetch business partners RFC
                    BusinessPartner = CurrentPrincipal.BusinessPartner,
                    BusinessPartnerDisplay = CurrentPrincipal.BusinessPartner + "-" + CurrentPrincipal.Name
                };
                CacheProvider.Store(CacheKeys.Miscellaneous_MeterTestingNewconnection, new AccessCountingCacheItem<MeterTestingNewconnection>(model, Times.Once));
                return View("~/Views/Feature/GeneralServices/Miscellaneous/MeterTestingNewconnection.cshtml", model);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The MeterTestingNewconnection
        /// </summary>
        /// <param name="model">The model<see cref="MeterTestingNewconnection"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult MeterTestingNewconnection(MeterTestingNewconnection model)
        {
            MeterTestingNewconnection cachemodel;
            if (CacheProvider.TryGet(CacheKeys.Miscellaneous_MeterTestingNewconnection, out cachemodel))
            {
                var list = CustomJsonConvertor.DeserializeObject<List<notificationActivities>>(model.selectedMaterialJSON);
                list = listNotification(list, cachemodel.Materials, "MISC-MTN", 1, 99);
                string error = string.Empty;
                var Attachment = new byte[0];
                if (model.Attachment1 != null && model.Attachment1.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment1, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_NEW_CONNECTION);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment1.InputStream.CopyTo(memoryStream);
                            Attachment = memoryStream.ToArray();
                        }
                    }
                }
                if (list.Count > 0)
                {
                    miscellaneousInput input = new miscellaneousInput
                    {
                        activity = ((int)MiscellaneousActivity.Step2).ToString("D4"),
                        attachflag = model.Attachment1 != null ? "X" : string.Empty,
                        businesspartnernumber = model.BusinessPartner,
                        code = ((int)MiscellaneousCode.MeterTestingNewconnection).ToString("D4"),
                        codegroup = "MISC.SER",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,

                        notificationinput = new notification
                        {
                            attachments = model.Attachment1 == null ? null : new miscellaneousAttachment[]
                            {
                            new miscellaneousAttachment
                            {
                            content = Attachment,
                            contenttype = model.Attachment1.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment1.FileName.GetFileNameWithoutPath()
                        }
                            },
                            customernumber = model.BusinessPartner,
                            codegroup = "MISC.SER",
                            credential = CurrentPrincipal.SessionToken,
                            coding = ((int)MiscellaneousCode.MeterTestingNewconnection).ToString("D4"),
                            comments = new comments[]
                            {
                            new comments
                            {
                                textline = model.Remarks
                            }
                            },
                            deviceid = model.SubstationName,
                            notifactivities = list.ToArray(),
                            purchaseordernumber = model.ApplicationNumber
                        }
                    };
                    var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
                    if (request.Succeeded && request.Payload != null && request.Payload.notificationoutput != null && !string.IsNullOrWhiteSpace(request.Payload.notificationoutput.notificationnumber))
                    {
                        CacheProvider.Store(CacheKeys.Miscellaneous_Success, new AccessCountingCacheItem<string>(request.Payload.notificationoutput.notificationnumber, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_NEW_CONNECTION_SUCCESS);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(request.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_NEW_CONNECTION);
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("MeterTestingproject.Error"), Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.METER_TESTING_NEW_CONNECTION);
        }

        #endregion MeterTestingNewconnection

        #region OilTesting

        /// <summary>
        /// The OilTesting
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult OilTesting()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            miscellaneousInput input = new miscellaneousInput
            {
                activity = ((int)MiscellaneousActivity.Step1).ToString("D4"),
                businesspartnernumber = CurrentPrincipal.BusinessPartner,
                code = ((int)MiscellaneousCode.OilTesting).ToString("D4"),
                codegroup = "MISC.SER",
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
                notificationinput = new notification
                {
                    customernumber = CurrentPrincipal.BusinessPartner,
                    codegroup = "MISC.SER",
                    credential = CurrentPrincipal.SessionToken,
                    coding = ((int)MiscellaneousCode.OilTesting).ToString("D4"),
                }
            };
            var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
            if (request.Succeeded && request.Payload != null)
            {
                OilTesting model = new OilTesting
                {
                    Materials = request.Payload.notificationoutput.codegrouplist,
                    // BusinessPartners = GetBusinessPartners()
                    BusinessPartner = CurrentPrincipal.BusinessPartner,
                    BusinessPartnerDisplay = CurrentPrincipal.BusinessPartner + "-" + CurrentPrincipal.Name
                };
                CacheProvider.Store(CacheKeys.Miscellaneous_OilTesting, new AccessCountingCacheItem<OilTesting>(model, Times.Once));
                return View("~/Views/Feature/GeneralServices/Miscellaneous/OilTesting.cshtml", model);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The MeterTestingNewconnection
        /// </summary>
        /// <param name="model">The model<see cref="OilTesting"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OilTesting(OilTesting model)
        {
            OilTesting cachemodel;
            if (CacheProvider.TryGet(CacheKeys.Miscellaneous_OilTesting, out cachemodel))
            {
                var list = CustomJsonConvertor.DeserializeObject<List<notificationActivities>>(model.selectedMaterialJSON);
                list = listNotification(list, cachemodel.Materials, "MISC-OIL", 1, 99);
                list.Add(new notificationActivities()
                {
                    activitycode = "0100",
                    activitytext = model.numberOfOilSamples,
                    activitycodegroup = "MISC-OIL",
                    activitysortnumber = "0100",
                    itemsortnumber = "0100"
                });
                string error = string.Empty;
                var Attachment = new byte[0];
                if (model.Attachment1 != null && model.Attachment1.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment1, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.OIL_TESTING);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment1.InputStream.CopyTo(memoryStream);
                            Attachment = memoryStream.ToArray();
                        }
                    }
                }
                if (list.Count > 0)
                {
                    miscellaneousInput input = new miscellaneousInput
                    {
                        activity = ((int)MiscellaneousActivity.Step2).ToString("D4"),
                        attachflag = model.Attachment1 != null ? "X" : string.Empty,
                        businesspartnernumber = model.BusinessPartner,
                        code = ((int)MiscellaneousCode.OilTesting).ToString("D4"),
                        codegroup = "MISC.SER",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,

                        notificationinput = new notification
                        {
                            attachments = model.Attachment1 == null ? null : new miscellaneousAttachment[]
                            {
                            new miscellaneousAttachment
                            {
                            content = Attachment,
                            contenttype = model.Attachment1.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment1.FileName.GetFileNameWithoutPath()
                        }
                            },
                            customernumber = model.BusinessPartner,
                            codegroup = "MISC.SER",
                            credential = CurrentPrincipal.SessionToken,
                            coding = ((int)MiscellaneousCode.OilTesting).ToString("D4"),
                            comments = new comments[]
                            {
                            new comments
                            {
                                textline = model.Remarks
                            }
                            },
                            //deviceid = model.SubstationName,
                            notifactivities = list.ToArray(),
                            //purchaseordernumber = model.PurchaseOrderNumber
                        }
                    };
                    var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
                    if (request.Succeeded && request.Payload != null && request.Payload.notificationoutput != null && !string.IsNullOrWhiteSpace(request.Payload.notificationoutput.notificationnumber))
                    {
                        CacheProvider.Store(CacheKeys.Miscellaneous_Success, new AccessCountingCacheItem<string>(request.Payload.notificationoutput.notificationnumber, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.OIL_TESTING_SUCCESS);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(request.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.OIL_TESTING);
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("MeterTestingproject.Error"), Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.OIL_TESTING);
        }

        #endregion OilTesting

        #region DemineralizedWater

        /// <summary>
        /// The DemineralizedWater
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet, TwoPhaseAuthorize]
        public ActionResult DemineralizedWater()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            miscellaneousInput input = new miscellaneousInput
            {
                activity = ((int)MiscellaneousActivity.Step1).ToString("D4"),
                businesspartnernumber = CurrentPrincipal.BusinessPartner,
                code = ((int)MiscellaneousCode.DemineralizedWater).ToString("D4"),
                codegroup = "MISC.SER",
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
                notificationinput = new notification
                {
                    customernumber = CurrentPrincipal.BusinessPartner,
                    codegroup = "MISC.SER",
                    credential = CurrentPrincipal.SessionToken,
                    coding = ((int)MiscellaneousCode.DemineralizedWater).ToString("D4"),
                }
            };
            var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
            if (request.Succeeded && request.Payload != null)
            {
                DemineralizedWater model = new DemineralizedWater
                {
                    Materials = request.Payload.notificationoutput.codegrouplist,
                    BusinessPartners = GetBusinessPartners()
                };
                CacheProvider.Store(CacheKeys.Miscellaneous_DemineralizedWater, new AccessCountingCacheItem<DemineralizedWater>(model, Times.Once));
                return View("~/Views/Feature/GeneralServices/Miscellaneous/DemineralizedWater.cshtml", model);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The MeterTestingNewconnection
        /// </summary>
        /// <param name="model">The model<see cref="DemineralizedWater"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DemineralizedWater(DemineralizedWater model)
        {
            DemineralizedWater cachemodel;
            if (CacheProvider.TryGet(CacheKeys.Miscellaneous_DemineralizedWater, out cachemodel))
            {
                var list = CustomJsonConvertor.DeserializeObject<List<notificationActivities>>(model.selectedMaterialJSON);
                list = listNotification(list, cachemodel.Materials, "MISC-DMW", 50, 1000);
                string error = string.Empty;
                var Attachment = new byte[0];
                if (model.Attachment1 != null && model.Attachment1.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment1, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEMINERALIZED_WATER);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment1.InputStream.CopyTo(memoryStream);
                            Attachment = memoryStream.ToArray();
                        }
                    }
                }
                if (list.Count > 0)
                {
                    miscellaneousInput input = new miscellaneousInput
                    {
                        activity = ((int)MiscellaneousActivity.Step2).ToString("D4"),
                        attachflag = model.Attachment1 != null ? "X" : string.Empty,
                        businesspartnernumber = model.BusinessPartner,
                        code = ((int)MiscellaneousCode.DemineralizedWater).ToString("D4"),
                        codegroup = "MISC.SER",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,

                        notificationinput = new notification
                        {
                            attachments = model.Attachment1 == null ? null : new miscellaneousAttachment[]
                            {
                            new miscellaneousAttachment
                            {
                            content = Attachment,
                            contenttype = model.Attachment1.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment1.FileName.GetFileNameWithoutPath()
                        }
                            },
                            customernumber = model.BusinessPartner,
                            codegroup = "MISC.SER",
                            credential = CurrentPrincipal.SessionToken,
                            coding = ((int)MiscellaneousCode.DemineralizedWater).ToString("D4"),
                            comments = new comments[]
                            {
                            new comments
                            {
                                textline = model.Remarks
                            }
                            },
                            //deviceid = model.SubstationName,
                            notifactivities = list.ToArray(),
                            //purchaseordernumber = model.PurchaseOrderNumber
                        }
                    };
                    var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
                    if (request.Succeeded && request.Payload != null && request.Payload.notificationoutput != null && !string.IsNullOrWhiteSpace(request.Payload.notificationoutput.notificationnumber))
                    {
                        CacheProvider.Store(CacheKeys.Miscellaneous_Success, new AccessCountingCacheItem<string>(request.Payload.notificationoutput.notificationnumber, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEMINERALIZED_WATER_SUCCESS);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(request.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEMINERALIZED_WATER);
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("MeterTestingproject.Error"), Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.DEMINERALIZED_WATER);
        }

        #endregion DemineralizedWater

        #region JointerTesting

        /// <summary>
        /// The JointerTesting
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false, Order = 0, Roles = Roles.Miscellaneous)]
        [HttpGet]
        public ActionResult JointerTesting()
        {
            string errorMessage;
            if (CacheProvider.TryGet(CacheKeys.ERROR_MESSAGE, out errorMessage))
            {
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            miscellaneousInput input = new miscellaneousInput
            {
                activity = ((int)MiscellaneousActivity.Step1).ToString("D4"),
                businesspartnernumber = CurrentPrincipal.BusinessPartner,
                code = ((int)MiscellaneousCode.JointerTesting).ToString("D4"),
                codegroup = "MISC.SER",
                sessionid = CurrentPrincipal.SessionToken,
                userid = CurrentPrincipal.UserId,
                notificationinput = new notification
                {
                    customernumber = CurrentPrincipal.BusinessPartner,
                    codegroup = "MISC.SER",
                    credential = CurrentPrincipal.SessionToken,
                    coding = ((int)MiscellaneousCode.JointerTesting).ToString("D4"),
                }
            };
            var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
            if (request.Succeeded && request.Payload != null)
            {
                JointerTesting model = new JointerTesting
                {
                    Materials = request.Payload.notificationoutput.codegrouplist,
                    //BusinessPartners = GetBusinessPartners(),
                    BusinessPartnerDisplay = CurrentPrincipal.BusinessPartner + "-" + CurrentPrincipal.Name,
                    BusinessPartner = CurrentPrincipal.BusinessPartner,
                    ContractorList = GetContractorList(request.Payload.notificationoutput.contractorlist)
                };
                CacheProvider.Store(CacheKeys.Miscellaneous_JointerTesting, new AccessCountingCacheItem<JointerTesting>(model, Times.Once));
                return View("~/Views/Feature/GeneralServices/Miscellaneous/JointerTesting.cshtml", model);
            }
            return new EmptyResult();
        }

        /// <summary>
        /// The MeterTestingNewconnection
        /// </summary>
        /// <param name="model">The model<see cref="JointerTesting"/></param>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult JointerTesting(JointerTesting model)
        {
            JointerTesting cachemodel;
            if (CacheProvider.TryGet(CacheKeys.Miscellaneous_JointerTesting, out cachemodel))
            {
                var list = CustomJsonConvertor.DeserializeObject<List<notificationActivities>>(model.selectedMaterialJSON);
                list = listNotification(list, cachemodel.Materials, "MISC-JOT", 1, 1);
                list.Add(new notificationActivities()
                {
                    activitycode = "0100",
                    activitytext = model.Contractor,
                    activitycodegroup = "MISC-JOT",
                    activitysortnumber = "0100",
                    itemsortnumber = "0100"
                });

                if (!string.IsNullOrEmpty(model.JointerName))
                {
                    list.Add(new notificationActivities()
                    {
                        activitycode = "0101",
                        activitytext = model.JointerName,
                        activitycodegroup = "MISC-JOT",
                        activitysortnumber = "0101",
                        itemsortnumber = "0101"
                    });
                }
                if (!string.IsNullOrEmpty(model.AsstJointerName))
                {
                    list.Add(new notificationActivities()
                    {
                        activitycode = "0102",
                        activitytext = model.AsstJointerName,
                        activitycodegroup = "MISC-JOT",
                        activitysortnumber = "0102",
                        itemsortnumber = "0102"
                    });
                }
                if (!string.IsNullOrEmpty(model.HelperName))
                {
                    list.Add(new notificationActivities()
                    {
                        activitycode = "0103",
                        activitytext = model.HelperName,
                        activitycodegroup = "MISC-JOT",
                        activitysortnumber = "0103",
                        itemsortnumber = "0103"
                    });
                }
                string error1 = string.Empty;
                var Attachment1 = new byte[0];
                if (model.Attachment1 != null && model.Attachment1.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment1, General.MaxAttachmentSize, out error1, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error1, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment1.InputStream.CopyTo(memoryStream);
                            Attachment1 = memoryStream.ToArray();
                        }
                    }
                }

                string error2 = string.Empty;
                var Attachment2 = new byte[0];
                if (model.Attachment2 != null && model.Attachment2.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment2, General.MaxAttachmentSize, out error1, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error2, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment2.InputStream.CopyTo(memoryStream);
                            Attachment2 = memoryStream.ToArray();
                        }
                    }
                }
                string error3 = string.Empty;
                var Attachment3 = new byte[0];
                if (model.Attachment3 != null && model.Attachment3.ContentLength > 0)
                {
                    if (!AttachmentIsValid(model.Attachment3, General.MaxAttachmentSize, out error3, General.AcceptedFileTypes))
                    {
                        CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(error1, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
                    }
                    else
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            model.Attachment3.InputStream.CopyTo(memoryStream);
                            Attachment3 = memoryStream.ToArray();
                        }
                    }
                }
                if (list.Count > 0)
                {
                    miscellaneousInput input = new miscellaneousInput
                    {
                        activity = ((int)MiscellaneousActivity.Step2).ToString("D4"),
                        attachflag = model.Attachment1 != null ? "X" : string.Empty,
                        businesspartnernumber = model.BusinessPartner,
                        code = ((int)MiscellaneousCode.JointerTesting).ToString("D4"),
                        codegroup = "MISC.SER",
                        sessionid = CurrentPrincipal.SessionToken,
                        userid = CurrentPrincipal.UserId,

                        notificationinput = new notification
                        {
                            attachments = model.Attachment1 == null ? null : new miscellaneousAttachment[]
                            {
                            new miscellaneousAttachment
                            {
                            content = Attachment1,
                            contenttype = model.Attachment1.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment1.FileName.GetFileNameWithoutPath()
                        },model.Attachment2==null?null:new miscellaneousAttachment
                            {
                            content = Attachment2,
                            contenttype = model.Attachment2.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment2.FileName.GetFileNameWithoutPath()
                        },model.Attachment3==null?null:new miscellaneousAttachment
                            {
                            content = Attachment3,
                            contenttype = model.Attachment3.FileName.GetFileExtensionTrimmed(),
                            filename =model.Attachment3.FileName.GetFileNameWithoutPath()
                        }
                            },
                            customernumber = model.BusinessPartner,
                            codegroup = "MISC.SER",
                            credential = CurrentPrincipal.SessionToken,
                            coding = ((int)MiscellaneousCode.JointerTesting).ToString("D4"),
                            comments = new comments[]
                            {
                            new comments
                            {
                                textline = model.Remarks
                            }
                            },
                            //deviceid = model.SubstationName,
                            notifactivities = list.ToArray(),
                            //purchaseordernumber = model.PurchaseOrderNumber
                        }
                    };
                    var request = DewaApiClient.SetMiscellaneousRequest(input, RequestLanguage, Request.Segment());
                    if (request.Succeeded && request.Payload != null && request.Payload.notificationoutput != null && !string.IsNullOrWhiteSpace(request.Payload.notificationoutput.notificationnumber))
                    {
                        CacheProvider.Store(CacheKeys.Miscellaneous_Success, new AccessCountingCacheItem<string>(request.Payload.notificationoutput.notificationnumber, Times.Once));
                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING_SUCCESS);
                    }
                    CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(request.Message, Times.Once));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
                }
            }
            CacheProvider.Store(CacheKeys.ERROR_MESSAGE, new AccessCountingCacheItem<string>(Translate.Text("MeterTestingproject.Error"), Times.Once));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.JOINTER_TESTING);
        }

        #endregion JointerTesting

        #region Success

        /// <summary>
        /// The MiscellaneousSuccess
        /// </summary>
        /// <returns>The <see cref="ActionResult"/></returns>
        [HttpGet]
        public ActionResult MiscellaneousSuccess()
        {
            string notificationnumber;
            if (CacheProvider.TryGet(CacheKeys.Miscellaneous_Success, out notificationnumber))
            {
                ViewBag.notificationnumber = notificationnumber;
                return View("~/Views/Feature/GeneralServices/Miscellaneous/_Success.cshtml");
            }
            var currentItem = Context.Item;
            return Redirect(Sitecorex.Links.LinkManager.GetItemUrl(currentItem.Parent));
        }

        #endregion Success

        /// <summary>
        /// The GetCodeGroup
        /// </summary>
        /// <param name="codelist">The codelist<see cref="codeGroupsList[]"/></param>
        /// <returns>The <see cref="IEnumerable{SelectListItem}"/></returns>
        public IEnumerable<SelectListItem> GetCodeGroup(codeGroupsList[] codelist)
        {
            try
            {
                var convertedItems = codelist.ToList().Select(c => new SelectListItem { Text = c.codeshorttext, Value = c.code });
                return convertedItems;
            }
            catch (System.Exception)
            {
                throw new System.Exception("Error in getting Customer Types");
            }
        }

        /// <summary>
        /// The GetBusinessPartners
        /// </summary>
        ///
        /// <returns>The <see cref="List{SelectListItem}"/></returns>
        private List<SelectListItem> GetBusinessPartners()
        {
            var UserDetails = DewaApiClient.GetCustomerDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken,
                RequestLanguage, Request.Segment());
            List<BusinessPartner> lstBusinessPartner = UserDetails.Payload.BusinessPartners;
            CacheProvider.Store(CacheKeys.MOVEIN_lST_BUSINESSPARTNER, new CacheItem<List<BusinessPartner>>(lstBusinessPartner));
            var bp = UserDetails.Payload.BusinessPartners.Select(c => new SelectListItem
            {
                Text = c.businesspartnernumber + '-' + c.bpname,
                Value = c.businesspartnernumber
            }).ToList();

            return bp;
        }

        private IEnumerable<SelectListItem> GetContractorList(contractorList[] contractorlist)
        {
            if (contractorlist != null)
            {
                var list = contractorlist.ToList().Select(c => new SelectListItem
                {
                    Text = c.activitytext,
                    Value = c.activitytext
                });
                return list;
            }
            return null;
        }

        private bool parseCount(string count, int min, int max)
        {
            if (string.IsNullOrWhiteSpace(count)) return false;
            int num;
            if (int.TryParse(count, out num))
            {
                if (num >= min && num <= max)
                {
                    return true;
                }
            }
            return false;
        }

        private List<notificationActivities> listNotification(List<notificationActivities> list, IEnumerable<codeGroupsList> Materials, string codegroup, int min, int max)
        {
            list = list.Where(x => !string.IsNullOrWhiteSpace(x.activitycode) && Materials.Select(y => y.code).ToList().Contains(x.activitycode) && parseCount(x.activitytext, min, max)).ToList();
            list.ForEach(x =>
            {
                x.activitycodegroup = codegroup;
                x.activitysortnumber = x.activitycode;
                x.itemsortnumber = x.activitycode;
            });
            return list;
        }
    }
}