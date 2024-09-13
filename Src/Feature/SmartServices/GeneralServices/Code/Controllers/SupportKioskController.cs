using DEWAXP.Feature.GeneralServices.Models.SupportKiosk;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.Web.Mvc;
using SitecoreX = Sitecore.Context;

namespace DEWAXP.Feature.GeneralServices.Controllers
{
    public class SupportKioskController : BaseController
    {
        [HttpGet]
        public ActionResult TrackRequest()
        {
            return View("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequest.cshtml", new TrackRequestDetails());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult TrackRequest(TrackRequestDetails model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.RequestNumber))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Kiosk.RequestNumberError"));
                }
                else
                {
                    var response = DewaApiClient.GetCustomerEnquiry(model.RequestNumber, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload != null && !string.IsNullOrEmpty(response.Payload.Reference))
                    {
                        var modelDetails = new TrackRequestDetails()
                        {
                            AccountNumber = response.Payload.AccountNumber,
                            RequestDate = FormatDateStamp(response.Payload.RequestDate) + " " + response.Payload.RequestTime,
                            RequestType = response.Payload.RequestType,
                            RequestNumber = response.Payload.Reference,
                            RequestStatus = response.Payload.Status
                        };
                        return PartialView("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequestDetails.cshtml", modelDetails);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, response.Message);
                    }
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }
            return View("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequest.cshtml", model);
        }

        public ActionResult TrackRequestDetails(string requestNumber)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestNumber))
                {
                    ViewBag.ErrorMessage = Translate.Text("Kiosk.RequestNumberError");
                    return PartialView("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequestDetails.cshtml", new TrackRequestDetails());
                }
                else
                {
                    var response = DewaApiClient.GetCustomerEnquiry(requestNumber, RequestLanguage, Request.Segment());

                    if (response.Succeeded && response.Payload != null && !string.IsNullOrEmpty(response.Payload.Reference))
                    {
                        var modelDetails = new TrackRequestDetails()
                        {
                            AccountNumber = response.Payload.AccountNumber,
                            RequestDate = FormatDateStamp(response.Payload.RequestDate) + " " + response.Payload.RequestTime,
                            RequestType = response.Payload.RequestType,
                            RequestNumber = response.Payload.Reference,
                            RequestStatus = response.Payload.Status
                        };
                        return PartialView("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequestDetails.cshtml", modelDetails);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = response.Message;
                        return PartialView("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequestDetails.cshtml", new TrackRequestDetails());
                    }
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = Translate.Text("Unexpected error");
                return PartialView("~/Views/Feature/GeneralServices/SupportKiosk/TrackRequestDetails.cshtml", new TrackRequestDetails());
            }
        }

        private static string FormatDateStamp(string datestamp)
        {
            if (!string.IsNullOrWhiteSpace(datestamp))
            {
                var trimmed = datestamp.Replace("PM", string.Empty).Replace("AM", string.Empty).Replace("M", string.Empty).Trim();

                DateTime d;
                if (DateTime.TryParse(trimmed, out d))
                {
                    return d.ToString("dd-MM-yyyy", SitecoreX.Culture);
                }
            }
            return string.Empty;
        }
    }
}