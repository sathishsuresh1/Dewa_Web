using DEWAXP.Feature.Bills.VerifyDocument;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using Sitecore.Globalization;
using System;
using System.Text;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    //[TwoPhaseAuthorize(EnsurePrimaryAccountIsSet = false)]
    public class VerifyDocumentController : BaseController
    {
        #region VerifyDocGet

        [AcceptVerbs("GET", "HEAD")]
        public ActionResult VerifyDoc(string cert, string refno, string pin)
        {
            string _refNo = string.Empty;
            string _pin = string.Empty;
            //string _docType = string.Empty;
            VerifyDocumentModel _verifyModel = new VerifyDocumentModel { DocumentTypeRequired = false };
            if (!string.IsNullOrEmpty(refno))
            {
                _refNo = Base64Decode(refno);
                _verifyModel.DocumentTypeRequired = true;
            }
            if (!string.IsNullOrEmpty(pin))
                _pin = Base64Decode(pin);
            //if (!string.IsNullOrEmpty(doctype))
            //    _docType = Base64Decode(doctype);

            //ViewBag.DocumentTypeList = GetDocumentType();
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            _verifyModel.DocumentType = cert;
            _verifyModel.ReferenceNumber = _refNo;
            _verifyModel.PinNumber = _pin;
            return View("~/Views/Feature/Bills/VerifyDocument/VerifyDocumentation.cshtml", _verifyModel);
        }

        #endregion VerifyDocGet

        #region VerifyDocPost

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult VerifyDoc(VerifyDocumentModel model)
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
                if (status)
                {
                    var verifiedresponse = VerificationCodeClient.getQRCodeVerified(model.DocumentType, model.ReferenceNumber, model.PinNumber, RequestLanguage, Request.Segment());
                    if (verifiedresponse != null)
                    {
                        if (verifiedresponse.Succeeded && verifiedresponse.Payload != null)
                        {
                            ViewBag.Message = "valid";
                            model.QRCodeResponse = verifiedresponse.Payload;
                        }
                        else if (verifiedresponse != null && verifiedresponse.Payload != null && !string.IsNullOrWhiteSpace(verifiedresponse.Payload.number) && verifiedresponse.Payload.number.Equals("001"))
                        {
                            ViewBag.Message = "expired";
                        }
                        else if (verifiedresponse != null && verifiedresponse.Payload != null && !string.IsNullOrWhiteSpace(verifiedresponse.Payload.number) && verifiedresponse.Payload.number.Equals("002"))
                        {
                            ViewBag.Message = "locked";
                            model.QRCodeResponse = verifiedresponse.Payload;
                        }
                        //else if (verifiedresponse != null && verifiedresponse.Payload != null && !string.IsNullOrWhiteSpace(verifiedresponse.Payload.number) && !verifiedresponse.Payload.number.Equals("001"))
                        else
                        {
                            ViewBag.Message = "invalid";
                        }
                        return View("~/Views/Feature/Bills/VerifyDocument/DocumentDetails.cshtml", model);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, Translate.Text("Error Response"));
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
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
            //ViewBag.DocumentTypeList = GetDocumentType();
            return View("~/Views/Feature/Bills/VerifyDocument/VerifyDocumentation.cshtml", model);
        }

        #endregion VerifyDocPost

    }
}