using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    using DEWAXP.Foundation.Content;

    //using System.Web.Security;
    using DEWAXP.Foundation.Content.Controllers;
    using DEWAXP.Foundation.Content.Models.Common;
    using DEWAXP.Foundation.Content.Repositories;
    using DEWAXP.Foundation.Helpers.Extensions;
    using DEWAXP.Foundation.Integration.SmartConsultantSvc;
    using Models.OwnerTracking;

    public class OwnerTrackingController : BaseController
    {
        #region Actions

        [HttpGet]
        public ActionResult OwnerTrackingList(string s)
        {
            TrackOwnerModel model = new TrackOwnerModel();
            TrackOwnerModel cacheModel = new TrackOwnerModel();
            ListDataSources searchDataSource = null;
            ViewBag.IsLoggedIn = false;

            if (!string.IsNullOrWhiteSpace(s) && s.Equals("1"))
            {
                if (CacheProvider.TryGet(CacheKeys.OWNER_TRACKING_SEARCHOPTIONS, out searchDataSource))
                {
                    if (searchDataSource != null)
                    {
                        model.SearchOptions = searchDataSource.Items.Select(x => new SelectListItem
                        {
                            Text = x.Text,
                            Value = x.Value
                        }).ToList();
                    }
                }
                if (CacheProvider.TryGet(CacheKeys.OWNER_TRACKING_RESULT, out cacheModel))
                {
                    model.SearchOptions = cacheModel.SearchOptions;
                    model.selectedSearchType = cacheModel.selectedSearchType;
                    model.SearchText = cacheModel.SearchText;
                    model.TrackResultList = cacheModel.TrackResultList;
                    if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
                    {
                        model.ApplicationTypeList = cacheModel.ApplicationTypeList;
                        model.ProjectAreaList = cacheModel.ProjectAreaList;
                        model.selectedProjectArea = cacheModel.selectedProjectArea;
                        model.selectedApplicationType = cacheModel.selectedApplicationType;
                    }
                }
                else if (CacheProvider.TryGet(CacheKeys.OWNER_TRACKING_LOGIN_RESULT, out cacheModel))
                {
                    model.SearchOptions = cacheModel.SearchOptions;
                    model.selectedSearchType = cacheModel.selectedSearchType;
                    model.SearchText = cacheModel.SearchText;
                    model.TrackResultList = cacheModel.TrackResultList;
                    if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
                    {
                        model.ApplicationTypeList = cacheModel.ApplicationTypeList;
                        model.ProjectAreaList = cacheModel.ProjectAreaList;
                        model.selectedProjectArea = cacheModel.selectedProjectArea;
                        model.selectedApplicationType = cacheModel.selectedApplicationType;
                    }
                }
            }
            else
            {
                CacheProvider.Remove(CacheKeys.OWNER_TRACKING_ORDER_RESULT);
                CacheProvider.Remove(CacheKeys.OWNER_TRACKING_SEARCHOPTIONS);
                searchDataSource = ContentRepository.GetItem<ListDataSources>(new Glass.Mapper.Sc.GetItemByPathOptions(DataSources.OWNER_TRACKING_SEARCH_OPTIONS));
                if (searchDataSource != null)
                {
                    model.SearchOptions = searchDataSource.Items.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).Distinct().ToList();
                }
            }
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
            {
                ViewBag.IsLoggedIn = true;
                if (searchDataSource != null)
                {
                    model.SearchOptions = searchDataSource.Items.Where(x => x.Value.Contains("1") || x.Value.Contains("5")).Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();
                }
                if (string.IsNullOrWhiteSpace(s))
                {
                    model.TrackResultList = GetOwnerTracking(CurrentPrincipal.BusinessPartner, "2");
                    if (model.TrackResultList != null)
                    {
                        model.ApplicationTypeList = model.TrackResultList.GroupBy(t => t.ApplicationTypeCode).Select(g => g.First()).Select(x => new SelectListItem
                        {
                            Text = x.ApplicationType,
                            Value = x.ApplicationTypeCode
                        }).ToList();
                    }
                    if (model.TrackResultList != null)
                    {
                        model.ProjectAreaList = model.TrackResultList.GroupBy(t => t.ProjectArea).Select(g => g.First()).Select(x => new SelectListItem
                        {
                            Text = x.ProjectArea,
                            Value = x.ProjectArea
                        }).ToList();
                    }

                    CacheProvider.Store(CacheKeys.OWNER_TRACKING_LOGIN_RESULT, new CacheItem<TrackOwnerModel>(model, TimeSpan.FromMinutes(40)));
                }
            }
            else
            {
                CacheProvider.Remove(CacheKeys.OWNER_TRACKING_LOGIN_RESULT);
            }
            CacheProvider.Store(CacheKeys.OWNER_TRACKING_SEARCHOPTIONS, new CacheItem<ListDataSources>(searchDataSource, TimeSpan.FromMinutes(40)));
            return View("~/Views/Feature/GeneralServices/OwnerTracking/OwnerTrackSearch.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OwnerTrackingList(TrackOwnerModel model)
        {
            string responseMessage = string.Empty;
            string responseCode = string.Empty;
            ViewBag.IsLoggedIn = false;
            ListDataSources searchDataSource = null;
            if (CacheProvider.TryGet(CacheKeys.OWNER_TRACKING_SEARCHOPTIONS, out searchDataSource))
            {
                if (searchDataSource != null)
                {
                    model.SearchOptions = searchDataSource.Items.Select(x => new SelectListItem
                    {
                        Text = x.Text,
                        Value = x.Value
                    }).ToList();
                }
            }

            try
            {
                if (!model.isViewDetail)
                {
                    if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
                    {
                        TrackOwnerModel cacheModel = new TrackOwnerModel();
                        ViewBag.IsLoggedIn = true;
                        if (searchDataSource != null)
                        {
                            // Filter search options for logged in user
                            model.SearchOptions = searchDataSource.Items.Where(x => x.Value.Contains("1") || x.Value.Contains("5")).Select(x => new SelectListItem
                            {
                                Text = x.Text,
                                Value = x.Value
                            }).ToList();
                        }
                        if (CacheProvider.TryGet(CacheKeys.OWNER_TRACKING_LOGIN_RESULT, out cacheModel))
                        {
                            if (cacheModel.TrackResultList != null)
                            {
                                // Get the result from cache
                                model.TrackResultList = cacheModel.TrackResultList;
                                if (model.TrackResultList != null)
                                {
                                    // Get the distinct appliction types from result
                                    model.ApplicationTypeList = model.TrackResultList.GroupBy(t => t.ApplicationTypeCode).Select(g => g.First()).Select(x => new SelectListItem
                                    {
                                        Text = x.ApplicationType,
                                        Value = x.ApplicationTypeCode
                                    }).ToList();
                                }
                                if (model.TrackResultList != null)
                                {
                                    // Get the distinct project area from result
                                    model.ProjectAreaList = model.TrackResultList.GroupBy(t => t.ProjectArea).Select(g => g.First()).Select(x => new SelectListItem
                                    {
                                        Text = x.ProjectArea,
                                        Value = x.ProjectArea
                                    }).ToList();
                                }

                                switch (model.selectedSearchType)
                                {
                                    case "1":
                                        if (!string.IsNullOrWhiteSpace(model.SearchText) && !string.IsNullOrWhiteSpace(model.selectedSearchType))
                                            model.TrackResultList = model.TrackResultList.Where(x => x.ApplicationNumber == model.SearchText).ToList();
                                        break;

                                    default:
                                        if (!string.IsNullOrWhiteSpace(model.SearchText) && !string.IsNullOrWhiteSpace(model.selectedSearchType))
                                            model.TrackResultList = model.TrackResultList.Where(x => x.PlotNo == model.SearchText).ToList();
                                        break;
                                }

                                if (!string.IsNullOrWhiteSpace(model.selectedApplicationType))
                                    model.TrackResultList = model.TrackResultList.Where(x => x.ApplicationTypeCode == model.selectedApplicationType).ToList();

                                if (!string.IsNullOrWhiteSpace(model.selectedProjectArea))
                                    model.TrackResultList = model.TrackResultList.Where(x => x.ProjectArea == model.selectedProjectArea).ToList();

                                CacheProvider.Store(CacheKeys.OWNER_TRACKING_RESULT, new CacheItem<TrackOwnerModel>(model, TimeSpan.FromMinutes(40)));
                            }
                        }
                    }
                    else
                    {
                        model.TrackResultList = GetOwnerTracking(model.SearchText, model.selectedSearchType);
                        CacheProvider.Store(CacheKeys.OWNER_TRACKING_RESULT, new CacheItem<TrackOwnerModel>(model, TimeSpan.FromMinutes(40)));
                    }
                }
                else
                {
                    TrackOwnerOrderModel OwnerOrderModel = new TrackOwnerOrderModel();
                    if (!string.IsNullOrWhiteSpace(model.nocNumer) && !string.IsNullOrWhiteSpace(model.processType))
                    {
                        OwnerOrderModel = GetOwnerTrackingDetails(model.nocNumer, model.processType);
                        OwnerOrderModel.ApplicationNumber = model.applicationNumer;
                        if (model != null)
                        {
                            CacheProvider.Store(CacheKeys.OWNER_TRACKING_ORDER_RESULT, new CacheItem<TrackOwnerOrderModel>(OwnerOrderModel, TimeSpan.FromMinutes(5)));
                        }
                    }
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.OWNERTRACKING_APPLICATION_DETAILS);
                }
            }
            catch (System.Exception ex)
            {
                throw;
            }
            return View("~/Views/Feature/GeneralServices/OwnerTracking/OwnerTrackSearch.cshtml", model);
        }

        [HttpGet]
        public ActionResult ApplicationDetails()
        {
            TrackOwnerOrderModel model = new TrackOwnerOrderModel();
            ViewBag.IsLoggedIn = false;
            if (IsLoggedIn && CurrentPrincipal.Role.Equals(Roles.User))
            {
                ViewBag.IsLoggedIn = true;
            }
            if (CacheProvider.TryGet(CacheKeys.OWNER_TRACKING_ORDER_RESULT, out model))
            {
                if (model != null)
                {
                    return PartialView("~/Views/Feature/GeneralServices/OwnerTracking/ApplicationDetails.cshtml", model);
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.OWNERTRACKING_DASHBOARD);
        }

        public ActionResult OwnerTrackingOrderDetails()
        {
            TrackOwnerOrderModel model = new TrackOwnerOrderModel();

            return PartialView("~/Views/Feature/GeneralServices/OwnerTracking/_StatusDetail.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult getStatusDetails(string applicationNo, string noc, string processType)
        {
            TrackOwnerStatusModel model = new TrackOwnerStatusModel();
            if (!string.IsNullOrWhiteSpace(noc) && !string.IsNullOrWhiteSpace(processType))
            {
                model = GetOwnerTrackingStatus(noc, processType);
                model.ApplicationNumber = applicationNo;
                if (model.OwnerStatusResults != null)
                {
                    return PartialView("~/Views/Feature/GeneralServices/OwnerTracking/_StatusDetail.cshtml", model);
                }
            }
            return Json(new { status = model.errorCode, Message = model.errorDescription }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult getApplicationDetails(string applicationNo, string noc, string processType)
        {
            TrackOwnerOrderModel model = new TrackOwnerOrderModel();
            if (!string.IsNullOrWhiteSpace(noc) && !string.IsNullOrWhiteSpace(processType))
            {
                model = GetOwnerTrackingDetails(noc, processType);
                model.ApplicationNumber = applicationNo;
                if (model != null)
                {
                    CacheProvider.Store(CacheKeys.OWNER_TRACKING_ORDER_RESULT, new CacheItem<TrackOwnerOrderModel>(model, TimeSpan.FromMinutes(5)));
                }
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.OWNERTRACKING_APPLICATION_DETAILS);
        }

        #endregion Actions

        #region Private Methods

        private List<TrackResult> GetOwnerTracking(string searchText, string searchType)
        {
            List<TrackResult> trackResults = new List<TrackResult>();
            var response = ConsultantServiceClient.GetTrackOwner(new TrackOwner
            {
                application = (!string.IsNullOrWhiteSpace(searchType) && searchType == "1") ? searchText : "",
                businessPartner = (!string.IsNullOrWhiteSpace(searchType) && searchType == "2") ? searchText : "",
                emiratesid = (!string.IsNullOrWhiteSpace(searchType) && searchType == "3") ? searchText : "",
                nocnumber = (!string.IsNullOrWhiteSpace(searchType) && searchType == "4") ? searchText : "",
                plotnumber = (!string.IsNullOrWhiteSpace(searchType) && searchType == "5") ? searchText : "",
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Payload != null && response.Payload.@return.responseCode == "000")
            {
                if (response.Payload.@return != null && response.Payload.@return.trackResultList != null)
                {
                    trackResults = response.Payload.@return.trackResultList.Select(x => new TrackResult
                    {
                        NOCNumber = x.objectId,
                        ApplicationType = x.processTypeDesc,
                        ApplicationTypeCode = x.processType,
                        DateApplied = x.dateApplied,
                        ProjectArea = x.areaName,
                        ApplicationNumber = x.displayOrder,
                        PlotNo = x.plot,
                        PowerRequirementDate = x.powReqDate,
                        Status = x.status,
                        ColorCode = x.colorCode,
                        CommunityCode = x.communityCode
                    }).ToList();
                }
            }
            return trackResults;
        }

        private TrackOwnerOrderModel GetOwnerTrackingDetails(string noc, string processType)
        {
            TrackOwnerOrderModel trackOrderResults = new TrackOwnerOrderModel();
            var response = ConsultantServiceClient.GetTrackOwnerOrder(new TrackOwnerOrder
            {
                processType = processType,
                nocnumber = noc,
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Payload != null && response.Payload.@return.responseCode == "000")
            {
                trackOrderResults.BuildingPermitElectricity_YBPE = response.Payload.@return.buildingPermitElectricityDetails;
                trackOrderResults.ElectricityNoc_YBNE = response.Payload.@return.electricityNocDetails;
                trackOrderResults.GettingElectricity_YDA5 = response.Payload.@return.gettingElectricityDetails;
                trackOrderResults.GettingWater_YAPW = response.Payload.@return.gettingWaterDetails;
                trackOrderResults.OneStepElectricity_YLVI = response.Payload.@return.oneStepElectricityDetails;
                trackOrderResults.WaterNoc_YBNW = response.Payload.@return.waterNocDetails;
                trackOrderResults.ApplicationType = processType;
                trackOrderResults.NOCNumber = noc;
            }
            return trackOrderResults;
        }

        private TrackOwnerStatusModel GetOwnerTrackingStatus(string nocnumber, string processType)
        {
            TrackOwnerStatusModel model = new TrackOwnerStatusModel();
            var response = ConsultantServiceClient.GetTrackOwnerStatus(new TrackOwnerStatus
            {
                nocnumber = nocnumber,
                processType = processType
            }, RequestLanguage, Request.Segment());

            if (response != null && response.Payload != null && response.Payload.@return.responseCode == "000")
            {
                if (response.Payload.@return != null && response.Payload.@return.ownerStatusList != null)
                {
                    model.OwnerStatusResults = response.Payload.@return.ownerStatusList.Select(x => new OwnerStatus
                    {
                        ColorCode = x.colorCode,
                        DateDescription = x.dateDescription,
                        Status = x.status,
                        StatusDescription = x.statusDescription,
                        zzDate = x.zzDate
                    }).ToList();
                }
            }
            else
            {
                model.errorDescription = response.Payload.@return.description;
                model.errorCode = response.Payload.@return.responseCode;
            }
            return model;
        }

        #endregion Private Methods
    }
}