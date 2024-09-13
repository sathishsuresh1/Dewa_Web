// <copyright file="Expo2020Controller.cs">
// Copyright (c) 2020
// </copyright>
// <author>DEWA\mayur.prajapati</author>
using DEWAXP.Feature.Account.Models.Expo2020;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.APIHandler.Models.Request.Expo2020;
using DEWAXP.Foundation.Logger;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Account.Controllers
{
    /// <summary>
    /// Defines the <see cref="Expo2020Controller" />.
    /// </summary>
    public class Expo2020Controller : BaseController
    {
        #region FeedbackExpo

        [HttpGet]
        public ActionResult FeedbackExpo()
        {
            Expo2020Model expo2020Model = new Expo2020Model();
            try
            {
                // Captcha
                if (ReCaptchaHelper.Recaptchasetting())
                {
                    ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                    ViewBag.Recaptcha = true;
                }
                else
                {
                    ViewBag.Recaptcha = false;
                }
                expo2020Model.EXPODiscussionAreaList = GetMasterDataExpo();
                expo2020Model.IsLoggedIn = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? true : false;
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            return View("~/Views/Feature/Account/Expo2020/FeedbackExpo.cshtml", expo2020Model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult FeedbackExpo(Expo2020Model model)
        {
            Expo2020Request expo2020Request = new Expo2020Request();
            try
            {
                model.IsLoggedIn = !string.IsNullOrWhiteSpace(CurrentPrincipal.Username) ? true : false;
                // Captcha
                bool status = false;

                if (!model.IsLoggedIn)
                {
                    string recaptchaResponse = System.Convert.ToString(Request.Form["g-recaptcha-response"] ?? "");

                    if (ReCaptchaHelper.Recaptchasetting() && !String.IsNullOrEmpty(recaptchaResponse))
                    {
                        status = ReCaptchaHelper.RecaptchaResponse(recaptchaResponse);
                    }
                    else if (!ReCaptchaHelper.Recaptchasetting())
                    {
                        status = true;
                    }
                }
                if (status || model.IsLoggedIn)
                {
                    if (ModelState.IsValid)
                    {
                        #region Ruquest for Expo Feeback

                        expo2020Request.smartformsubmission = new SmartFormSubmission()
                        {
                            description = model.EXPOParticipant ?? string.Empty,
                            mobile = model.EXPOMobile.AddMobileNumberZeroPrefix() ?? string.Empty,
                            contactperson = model.EXPOParticipantName ?? string.Empty,
                            category = "Z24",
                            text = model.EXPODiscussionSubject ?? string.Empty,
                            emailid = model.EXPOEmailID ?? string.Empty,
                            majordeveloper = model.EXPODiscussionArea ?? string.Empty,
                            userid = CurrentPrincipal.UserId ?? string.Empty,
                            sessionid = CurrentPrincipal.SessionToken ?? string.Empty,
                        };

                        #endregion Ruquest for Expo Feeback

                        var feedbackResponse = Expo2020Client.FeedbackExpo(expo2020Request, RequestLanguage, Request.Segment());
                        if (feedbackResponse != null && feedbackResponse.Succeeded && feedbackResponse.Payload != null)
                        {
                            CacheProvider.Store("Expo2020Model", new CacheItem<Expo2020Model>(model));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.Expo2020_Success);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, feedbackResponse.Message);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("unsubscribe-Captcha-Not-Valid"));
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
                ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
            }

            // Captcha
            if (ReCaptchaHelper.Recaptchasetting())
            {
                ViewBag.SiteKey = ReCaptchaHelper.RecaptchaSiteKey();
                ViewBag.Recaptcha = true;
            }
            else
            {
                ViewBag.Recaptcha = false;
            }
            model.EXPODiscussionAreaList = GetMasterDataExpo();
            return View("~/Views/Feature/Account/Expo2020/FeedbackExpo.cshtml", model);
        }

        [HttpGet]
        public ActionResult FeedbackSuccess()
        {
            Expo2020Model expo2020Model = null;
            if (CacheProvider.TryGet("Expo2020Model", out expo2020Model))
            {
                CacheProvider.Remove("Expo2020Model");
                return PartialView("~/Views/Feature/Account/Expo2020/FeedbackExpoSuccess.cshtml", new Expo2020Model()
                {
                    IsSuccess = true
                });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.Expo2020_Inquiries);
        }

        #endregion FeedbackExpo

        private List<SelectListItem> GetMasterDataExpo()
        {
            //List<SelectListItem> data = new List<SelectListItem>();
            List<SelectListItem> reponseData = null;

            if (!CacheProvider.TryGet("Expo2020MasterData" + RequestLanguage.ToString(), out reponseData))
            {
                var masterData = Expo2020Client.MasterDataExpo(RequestLanguage, Request.Segment());

                if (masterData.Succeeded && masterData.Payload.details != null && masterData != null)
                {
                    reponseData = new List<SelectListItem>();
                    reponseData = masterData.Payload.details.Select(x => new SelectListItem()
                    {
                        Text = x.description,
                        Value = x.majordeveloper
                    }).ToList();
                }
                //if (reponseData != null)
                //{
                //    data.AddRange(reponseData);
                //}
                CacheProvider.Store("Expo2020MasterData" + RequestLanguage.ToString(), new CacheItem<List<SelectListItem>>(reponseData, TimeSpan.FromMinutes(40)));
            }
            return reponseData;
        }
    }
}