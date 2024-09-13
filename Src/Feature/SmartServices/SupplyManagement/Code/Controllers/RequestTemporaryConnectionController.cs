using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Feature.SupplyManagement.Models.RequestTempConnection;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Models.Common;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Enums;
using DEWAXP.Foundation.Integration.Requests;
using Sitecore.Globalization;
using System;
using System.Web.Mvc;
using DEWAXP.Foundation.Content.Models.RequestTempConnection;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    [TwoPhaseAuthorize]
    public class RequestTemporaryConnectionController : BaseController
    {
        private readonly IDropdownHelper _dropdownHelper;
        protected IDropdownHelper DropdownHelper => _dropdownHelper;
        public RequestTemporaryConnectionController():base()
        {
            _dropdownHelper = DependencyResolver.Current.GetService<IDropdownHelper>();
        }
        [HttpGet]
        public PartialViewResult NewRequest()
        {
            var result = DewaApiClient.GetServiceComplaintCriteria(RequestLanguage, Request.Segment());
            if (result.Succeeded)
            {
                ViewBag.Cities = DropdownHelper.CityDropdown(result.Payload.CityList);
            }

            ViewBag.Modal = ContentRepository.GetItem<ModalOverlay>(new Glass.Mapper.Sc.GetItemByIdOptions(Guid.Parse("{5EA1A59A-4BBB-46EA-9D12-53CCB04A4587}")));
            ViewBag.Page = ContextRepository.GetCurrentItem<RequestTempConnectionPage>();

            return PartialView("~/Views/Feature/SupplyManagement/RequestTemporaryConnection/_NewRequest.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(NewRequest model)
        {
            ValidateNewRequest(model);

            if (ModelState.IsValid)
            {
                var request = new RequestTemporaryConnection
                {
                    ContractAccountNumber = model.AccountNumber ?? string.Empty,
                    Start = model.EventStartDate.GetValueOrDefault(),
                    End = model.EventEndDate.GetValueOrDefault(),
                    Remarks = model.Description,
                    EventType = model.EventType,
                    City = model.Location,
                    MobileNumber = model.MobileNumber.AddMobileNumberZeroPrefix()
                };

                var response = DewaApiClient.RequestTemporaryConnection(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, request, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    if (response.Payload != null && !string.IsNullOrEmpty(response.Payload.RequestNumber))
                    {
                        CacheProvider.Store(CacheKeys.TEMP_CONN_REQ_REF, new CacheItem<string>(response.Payload.RequestNumber));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SUBMISSION_SUCCESSFUL);
                    }

                    if (response.Payload != null && string.IsNullOrEmpty(response.Payload.RequestNumber))
                    {
                        CacheProvider.Store(CacheKeys.TEMP_CON_REQ_FAILED, new CacheItem<string>(Translate.Text("Invalid account number provided")));

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SUBMISSION_FAILED);
                    }

                    CacheProvider.Store(CacheKeys.TEMP_CON_REQ_FAILED, new CacheItem<string>(Translate.Text("An error occurred. Please try again")));
                    return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SUBMISSION_FAILED);
                }

                CacheProvider.Store(CacheKeys.TEMP_CON_REQ_FAILED, new CacheItem<string>(response.Message));

                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SUBMISSION_FAILED);
            }

            CacheProvider.Store(CacheKeys.TEMP_CON_REQ_FAILED, new CacheItem<string>(ViewData.ModelState.AsFormattedString()));
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SUBMISSION_FAILED);
        }

        [HttpGet]
        public ActionResult RequestSent()
        {
            string reference;
            CacheProvider.TryGet(CacheKeys.TEMP_CONN_REQ_REF, out reference);
            ViewBag.Reference = reference;
            return PartialView("~/Views/Feature/SupplyManagement/RequestTemporaryConnection/_RequestSent.cshtml");
        }

        [HttpGet]
        public PartialViewResult RequestFailed()
        {
            string message;
            CacheProvider.TryGet(CacheKeys.TEMP_CON_REQ_FAILED, out message);
            ViewBag.Message = message;
            return PartialView("~/Views/Feature/SupplyManagement/RequestTemporaryConnection/_RequestFailed.cshtml");
        }

        [HttpGet]
        public ActionResult Search()
        {
            CacheProvider.Remove(CacheKeys.TEMP_CONN_REQ_REF);

            ViewBag.Page = ContextRepository.GetCurrentItem<RequestTempConnectionPage>();

            return PartialView("~/Views/Feature/SupplyManagement/RequestTemporaryConnection/_SearchRequest.cshtml");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Search(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SEARCH_REQUEST);
            }

            CacheProvider.Store(CacheKeys.TEMP_CONN_REQ_REF, new CacheItem<string>(reference));

            return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_TRACK_PAY_REQUEST);
        }

        [HttpGet]
        public ActionResult TrackOrPayRequest()
        {
            string reference;
            if (!CacheProvider.TryGet(CacheKeys.TEMP_CONN_REQ_REF, out reference))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J75_SEARCH_REQUEST);
            }
            string message;
            if (CacheProvider.TryGet(CacheKeys.TEMP_CON_REQ_FAILED, out message))
            {
                ModelState.AddModelError(string.Empty, message);
                CacheProvider.Remove(CacheKeys.TEMP_CON_REQ_FAILED);
            }
            ViewBag.Page = ContextRepository.GetCurrentItem<RequestTempConnectionPage>();

            TrackTempConnectionRequestItem model = null;
            var response = DewaApiClient.GetTemporaryConnectionDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, reference, RequestLanguage, Request.Segment());
            if (response.Succeeded && response.Payload != null)
            {
                model = TrackTempConnectionRequestItem.From(response.Payload);
            }
            return PartialView("~/Views/Feature/SupplyManagement/RequestTemporaryConnection/_TrackOrPayRequest.cshtml", model);
        }

        #region Private

        private void ValidateNewRequest(NewRequest model)
        {
            if (!model.TermsConditions)
            {
                ModelState.AddModelError("", Translate.Text("You must agree to the terms and conditions"));
            }

            // If start is after end date
            if (model.EventStartDate.GetValueOrDefault() > model.EventEndDate.GetValueOrDefault())
            {
                ModelState.AddModelError("", Translate.Text("The start date cannot be after the end date"));
            }

            var now = DateHelper.DubaiNow();

            if (model.EventStartDate.GetValueOrDefault().Date < now.Date)
            {
                ModelState.AddModelError("", Translate.Text("The start date cannot be in the past"));
            }

            if (model.EventEndDate.GetValueOrDefault().Date < now.Date)
            {
                ModelState.AddModelError("", Translate.Text("The end date cannot be in the past"));
            }

            // If event type is wedding and less than 4 days from now
            if (model.EventType == EventType.Wedding)
            {
                var delta = model.EventStartDate.GetValueOrDefault().Date - DateTime.UtcNow.Date;
                if (delta.Days > 0 && delta.Days < 4)
                {
                    ModelState.AddModelError("", Translate.Text("Request for temporary connection for a wedding should be at least 4 days prior to start date"));
                }
            }
        }

        #endregion Private
    }
}