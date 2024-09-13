using DEWAXP.Feature.Account.Models.GovernmentObservation;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests;
using Sitecore.Data.Comparers;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Controllers
{
    [TwoPhaseAuthorize(Roles = Roles.Government, EnsurePrimaryAccountIsSet = false)]
    public class GovernmentObservationController : BaseController
    {
        [HttpGet]
        public ActionResult NewRequest()
        {
            CacheProvider.Remove(CacheKeys.GOV_OBS_SENT);
            CacheProvider.Remove(CacheKeys.GOV_OBS_FAILED);

            var details = DewaApiClient.GetGovernmentalUserDetails(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, RequestLanguage, Request.Segment());
            if (!details.Succeeded || details.Payload == null)
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J8_GOVT_LOGIN_PAGE);
            }

            return PartialView("~/Views/Feature/Account/GovernmentObservation/_NewRequest.cshtml", new SubmitObservationModel
            {
                Email = details.Payload.Email,
                MobileNumber = details.Payload.Mobile,
                Date = DateHelper.DubaiNow(),
                ApiKey = ConfigurationManager.AppSettings["GMAPAPIKEY"]
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult NewRequest(SubmitObservationModel model, List<HttpPostedFileBase> files)
        {
            for (var i = 0; i < Request.Files.Count; i++)
            {
                var file = Request.Files[i];
                string error;
                if (file != null && file.ContentLength > 0)
                {
                    if (!AttachmentIsValid(file, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                    {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }
            }
            if (ModelState.IsValid)
            {
                var request = new SubmitGovernmentObservation()
                {
                    Area = model.Area,
                    Email = model.Email,
                    MobileNumber = model.MobileNumber,
                    Date = model.Date,
                    Community = model.Community,
                    Defect = model.Defect,
                    ContactAccountNumber = model.ContactAccountNumber,
                    ElectricityOrWater = model.ElectricityOrWater,
                    Structure = model.Structure,
                    Road = model.Road
                };

                if (files.Any(f => f != null))
                {
                    foreach (var file in files.Where(f => f != null))
                    {
                        request.Attachments.Add(file.ToArray());
                    }
                }

                if (model.SendCoordinates)
                {
                    request.xGPS = model.Latitude;
                    request.yGPS = model.Longitude;
                }

                var response = DewaApiClient.SubmitGovernmentObservation(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, request, RequestLanguage, Request.Segment());
                if (response.Succeeded)
                {
                    CacheProvider.Store(CacheKeys.GOV_OBS_SENT, new CacheItem<string>(response.Payload.ObservationNo));

                    return RedirectToSitecoreItem((LinkHelper.GetPath(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST_SENT)));
                }
                CacheProvider.Store(CacheKeys.GOV_OBS_FAILED, new CacheItem<string>(response.Message));

                return RedirectToSitecoreItem((LinkHelper.GetPath(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST_FAILED)));
            }
            model.Date = DateHelper.DubaiNow();
            model.ApiKey = ConfigurationManager.AppSettings["GMAPAPIKEY"];
            return PartialView("~/Views/Feature/Account/GovernmentObservation/_NewRequest.cshtml", model);
        }

        [HttpGet]
        public ActionResult RequestSent()
        {
            string reference;
            if (!CacheProvider.TryGet(CacheKeys.GOV_OBS_SENT, out reference))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST);
            }
            ViewBag.Reference = reference;
            return PartialView("~/Views/Feature/Account/GovernmentObservation/_RequestSent.cshtml");
        }

        [HttpGet]
        public ActionResult RequestFailed()
        {
            string error;
            if (!CacheProvider.TryGet(CacheKeys.GOV_OBS_FAILED, out error))
            {
                return RedirectToSitecoreItem(SitecoreItemIdentifiers.J91_GOVT_OBS_REQUEST);
            }
            ViewBag.Message = error;
            return PartialView("~/Views/Feature/Account/GovernmentObservation/_RequestFailed.cshtml");
        }
    }
}