using DEWAXP.Feature.SupplyManagement.Models.MaiDubai;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Impl;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.General;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Masar;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.SmartCommunication;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static Sitecore.Configuration.State;

namespace DEWAXP.Feature.SupplyManagement.Controllers
{
    public class GeneralRequestController : BaseController
    {
        // GET: GeneralRequest
        [HttpGet]
        public ActionResult MaiDubaiRedemption(string r)
        {
            try
            {
                if (string.IsNullOrEmpty(r))
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
                else
                {
                    Subscriptioninput decryptCred = new Subscriptioninput()
                    {
                        contractaccount = r,
                        process = "MD",
                        appidentifier = "1",
                        appversion = "v",
                        mode = "R"

                    };

                    var response = GeneralServicesClient.DecryptURL(new MaiDubaiRequest()
                    {
                        subscriptioninput = decryptCred
                    }, RequestLanguage, Request.Segment());

                    if (response != null && response.Succeeded)
                    {
                        if (response.Payload != null)
                        {
                            linkState _successVal = linkState.Invalid;

                            if (response.Payload.resultcode == "0")
                                _successVal = linkState.Success;
                            else if (response.Payload.resultcode == "8")
                                _successVal = linkState.Redeemed;
                            else if (response.Payload.resultcode == "9")
                                _successVal = linkState.Expired;
                            else if (response.Payload.resultcode == "7")
                            {
                                _successVal = linkState.Invalid;
                                ModelState.AddModelError(string.Empty, Translate.Text("MaiDubai.LinkInvalid"));
                            }

                            //if (!string.IsNullOrEmpty(response.Payload.referencenumber))
                            //    CacheProvider.Store("r_Code", new CacheItem<string>(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(response.Payload.referencenumber)), TimeSpan.FromHours(4)));

                            return PartialView("~/Views/Feature/SupplyManagement/MaiDubai/RedeemMaiDubai.cshtml", new MaiDubaiViewModel
                            {
                                urlGUID = r,
                                MaskedEmail = response.Payload.subscribedemail,
                                MaskedMobile = response.Payload.subscribedmobile,
                                ReferenceCode = response.Payload.referencenumber,
                                isSucess = _successVal
                            });
                        }
                        else
                            ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                    }
                    else
                        ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }
            catch (System.Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return PartialView("~/Views/Feature/SupplyManagement/MaiDubai/RedeemMaiDubai.cshtml", new MaiDubaiViewModel
            {

            });
        }



        [HttpPost, ValidateAntiForgeryToken]
        public JsonResult CrudOtp(MasarSendOtpRequest request)
        {
            string errVerf = "";
            string cemail = string.Empty;
            string cmobile = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(request.email))
                    cemail = request.email;

                if (!string.IsNullOrEmpty(request.mobile))
                    cmobile = request.mobile;

                var response = SmartCommunicationClient.CustomerVerifyOtp(new SmartCommunicationVerifyOtpRequest()
                {
                    mode = request.mode,
                    sessionid = string.Empty,
                    reference = string.IsNullOrEmpty(request.reqId) ? string.Empty : request.reqId,
                    prtype = request.prtype,
                    email = (!string.IsNullOrEmpty(cemail)) ? cemail : string.Empty,
                    mobile = (!string.IsNullOrEmpty(cmobile)) ? cmobile : string.Empty,
                    contractaccountnumber = string.Empty,
                    businesspartner = string.Empty,
                    otp = !string.IsNullOrWhiteSpace(request.Otp) ? request.Otp.Trim() : null
                }, Request.Segment(), RequestLanguage);


                if (response != null && response.Succeeded)
                {
                    // responseOTP.Payload.email = response.Payload.maskedemail;
                    // responseOTP.Payload.mobile = response.Payload.maskedmobile;
                    response.Payload.otprequestid = request.reqId;
                    if (!string.IsNullOrEmpty(cemail))
                        response.Payload.description = cemail;
                    else if (!string.IsNullOrEmpty(cmobile))
                        response.Payload.description = cmobile;

                    if (!string.IsNullOrEmpty(request.reqId) && !string.IsNullOrWhiteSpace(request.Otp))
                        response.Payload.otprequestid = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(request.reqId));

                    return Json(new { status = true, desc = response.Message, data = response.Payload }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    errVerf = response.Message;
                }


                return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

            }
            catch (System.Exception ex)
            {
                LogService.Fatal(ex, this);
                errVerf = ex.Message;
            }
            return Json(new { status = false, desc = errVerf }, JsonRequestBehavior.AllowGet);

        }

    }
}