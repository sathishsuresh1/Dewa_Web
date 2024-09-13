using DEWAXP.Feature.GeneralServices.Models.Sponsorship;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Repositories;
using DEWAXP.Foundation.Helpers;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration;
using DEWAXP.Foundation.Logger;
using Newtonsoft.Json;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Attribute = DEWAXP.Feature.GeneralServices.Models.Sponsorship.Attribute;

namespace DEWAXP.Feature.GeneralServices.Controllers
{/// <summary>
 /// Defines the <see cref="SponsorshipController" />.
 /// </summary>
    public class SponsorshipController : BaseController
    {
        /// <summary>
        /// The Sponsorship.
        /// </summary>
        /// <returns>The <see cref="ActionResult"/>.</returns>
        [AcceptVerbs("GET", "HEAD")]
        public ActionResult SponsorshipDetails()
        {
            SponsorshipModel sponsorshipModel = new SponsorshipModel();
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

            // Dropdown list
            sponsorshipModel.eventParticipants.EventTargetAudienceList = GetLstDataSource(DataSources.SPONSORSHIP_TARGET_AUDIENCE).ToList();
            sponsorshipModel.sponsorshipDetails.EventSponsorshipDurationList = GetLstDataSource(DataSources.SPONSORSHIP_DURATION).ToList();
            sponsorshipModel.sponsorshipDetails.EventsponsorshipbeforeList = GetLstDataSource(DataSources.SPONSORSHIP_DEWA).ToList();

            return View("~/Views/Feature/GeneralServices/Sponsorship/SponsorshipDetails.cshtml",sponsorshipModel);
        }

        [HttpPost]
        public ActionResult SponsorshipDetails(SponsorshipModel model)
        {
            try
            {
                // Captcha
                bool status = false;
                model.IsSuccess = true;
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
                    string error = string.Empty;

                    // TradeLicense attachment
                    if (model.eventInformation.Event_tradelicenseattach != null && model.eventInformation.Event_tradelicenseattach.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.eventInformation.Event_tradelicenseattach, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            model.IsSuccess = false;
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.eventInformation.Event_tradelicenseattach.InputStream.CopyTo(memoryStream);
                                model.eventInformation.Event_tradelicenseattach_FileBinary = memoryStream.ToArray();
                            }
                        }
                    }

                    // SponsorshipRequestLetter attachment
                    if (model.sponsorshipDetails.Event_letterofsponsorattach != null && model.sponsorshipDetails.Event_letterofsponsorattach.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.sponsorshipDetails.Event_letterofsponsorattach, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            model.IsSuccess = false;
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.sponsorshipDetails.Event_letterofsponsorattach.InputStream.CopyTo(memoryStream);
                                model.sponsorshipDetails.Event_letterofsponsorattach_FileBinary = memoryStream.ToArray();
                            }
                        }
                    }

                    // BenefitsDocument attachment
                    if (model.otherDetails.Event_benefitsattach != null && model.otherDetails.Event_benefitsattach.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.otherDetails.Event_benefitsattach, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            model.IsSuccess = false;
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.otherDetails.Event_benefitsattach.InputStream.CopyTo(memoryStream);
                                model.otherDetails.Event_benefitsattach_FileBinary = memoryStream.ToArray();
                            }
                        }
                    }

                    // SupportingEventDocument attachment
                    if (model.sponsorshipDetails.Event_supporteventdocument != null && model.sponsorshipDetails.Event_supporteventdocument.ContentLength > 0)
                    {
                        if (!AttachmentIsValid(model.sponsorshipDetails.Event_supporteventdocument, General.MaxAttachmentSize, out error, General.AcceptedFileTypes))
                        {
                            model.IsSuccess = false;
                            ModelState.AddModelError(string.Empty, error);
                        }
                        else
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                model.sponsorshipDetails.Event_supporteventdocument.InputStream.CopyTo(memoryStream);
                                model.sponsorshipDetails.Event_supporteventdocument_FileBinary = memoryStream.ToArray();
                            }
                        }
                    }

                    if (ModelState.IsValid)
                    {
                        var isSaved = SaveSponsorshipForm(model);
                        if (isSaved.Item1 == true)
                        {
                            CacheProvider.Store("SponsorshipModel", new CacheItem<SponsorshipModel>(model));
                            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SPONSORSHIP_SUCCESS);
                        }
                        else
                        {
                            ModelState.AddModelError("", isSaved.Item2);
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

            // Dropdown list
            model.eventParticipants.EventTargetAudienceList = GetLstDataSource(DataSources.SPONSORSHIP_TARGET_AUDIENCE).ToList();
            model.sponsorshipDetails.EventSponsorshipDurationList = GetLstDataSource(DataSources.SPONSORSHIP_DURATION).ToList();
            model.sponsorshipDetails.EventsponsorshipbeforeList = GetLstDataSource(DataSources.SPONSORSHIP_DEWA).ToList();

            // Pre-fill goal and VIPs
            JavaScriptSerializer js = new JavaScriptSerializer();
            model.eventInformation.Eventgoalsjs = js.Serialize(model.eventInformation.Eventgoals.Split(','));
            model.eventParticipants.EventinvitedVIPjs = js.Serialize(model.eventParticipants.EventinvitedVIP.Split(','));
            model.eventParticipants.Eventbeneficiariesjs = js.Serialize(model.eventParticipants.Eventbeneficiaries.Split(','));

            return View("~/Views/Feature/GeneralServices/Sponsorship/SponsorshipDetails.cshtml",model);
        }

        private Tuple<bool, string, string> SaveSponsorshipForm(SponsorshipModel model)
        {
            Tuple<bool, string, string> retval = new Tuple<bool, string, string>(false, "", "");
            try
            {
                //create sponsorship in kofax
                KofaxBaseViewModel kofaxBaseViewModel = new KofaxBaseViewModel();
                kofaxBaseViewModel.Parameters.Add(new Parameter(KofaxSponsorConstnats.SPONSORNAME) { Attribute = GetSponsorshipAttributes(model) });
                var res = KofaxRESTService.SubmitKofax(KofaxSponsorConstnats.SPONSORSHIP_FORM, JsonConvert.SerializeObject(kofaxBaseViewModel, Converter.Settings));

                LogService.Info(new Exception("Sponsorship Kofax Service Debug") { Source = JsonConvert.SerializeObject(res) });

                if (res.Succeeded && res.Payload.RobotError == null && res.Payload.Values != null && res.Payload.Values.Length > 0 && res.Payload.Values[0].Attribute != null && res.Payload.Values[0].Attribute.Length > 0)
                {
                    string newid = res.Payload.Values[0].Attribute.Where(x => x.Name.Equals("RequestID") && x.Type.Equals("text")).FirstOrDefault().Value;
                    retval = new Tuple<bool, string, string>(true, string.Empty, newid);
                }
                else
                {
                    retval = new Tuple<bool, string, string>(false, res.Message, "");
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex, this);
            }

            return retval;
        }

        private List<Attribute> GetSponsorshipAttributes(SponsorshipModel model)
        {
            return new List<Attribute> {
                // step 1 - Event general information
                new Attribute { Type = SponsorTypeEnum.Text, Name = "ReqID", Value = model.RequestID ?? string.Empty },
                new Attribute { Type = SponsorTypeEnum.Text, Name = "EventName", Value = model.eventInformation.Eventname ?? string.Empty },
                new Attribute { Type = SponsorTypeEnum.Text, Name = "CompanyName", Value = model.eventInformation.Companyname?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "DateOfEvent", Value = model.eventInformation.Eventofdate ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "EventVenue", Value = model.eventInformation.Eventvenue ?? string.Empty },
                new Attribute { Type = SponsorTypeEnum.Text, Name = "ShortDescription", Value = model.eventInformation.Eventshortdescr ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "EventGoal", Value = model.eventInformation.Eventgoals ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Binary, Name = "TradeLicense", Value = model.eventInformation.Event_tradelicenseattach_FileBinary != null && model.eventInformation.Event_tradelicenseattach_FileBinary.Length > 0 ? Convert.ToBase64String(model.eventInformation.Event_tradelicenseattach_FileBinary) : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "TradelicenseFileType", Value = model.eventInformation.Event_tradelicenseattach != null? model.eventInformation.Event_tradelicenseattach.FileName : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "TradelicenseFileExtention", Value = model.eventInformation.Event_tradelicenseattach!= null? model.eventInformation.Event_tradelicenseattach.FileName.GetFileExtensionTrimmed() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "TradelicenseFileName", Value = model.eventInformation.Event_tradelicenseattach!= null? model.eventInformation.Event_tradelicenseattach.FileName.GetFileNameWithoutPath() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "TradelicenseFileContentType", Value = model.eventInformation.Event_tradelicenseattach!= null? model.eventInformation.Event_tradelicenseattach.ContentType?.ToLower() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "LetterToHisExcellencyMDCEO", Value = model.eventInformation.Eventlettertoceo ?? string.Empty},

                // step 2 - Event Participants
                new Attribute { Type = SponsorTypeEnum.Text, Name = "TargetAudience", Value = model.eventParticipants.Eventtargetaudience ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "Beneficiaries", Value = model.eventParticipants.Eventbeneficiaries ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "NoOfBeneficiary", Value = model.eventParticipants.Eventnoofbeneficiaries ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "InvitedVIPs", Value = model.eventParticipants.EventinvitedVIP ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "NoOfAttendees", Value = model.eventParticipants.Eventnoofattend ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "NoOfVolunteers", Value = model.eventParticipants.Eventnoofvolunteers ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "NoOfTeamCommittee", Value = model.eventParticipants.Eventnoofcommittee ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "NoOfParticipatingStaff", Value = model.eventParticipants.Eventnoofdewaemployee ?? string.Empty},

                // step 3 - Other details
                new Attribute { Type = SponsorTypeEnum.Text, Name = "MediaCoverage", Value = model.otherDetails.Eventmediacoverage ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "NoOfSocialMediaViews", Value = model.otherDetails.Eventnoofsocialmedia ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "CurrentSponsors", Value = model.otherDetails.Eventcurrentsponsor ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "PreviousSponsors", Value = model.otherDetails.Eventprevioussponsor ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SponsorshipManagementBy", Value = model.otherDetails.Eventmanagesponsor ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "Benifits", Value = model.otherDetails.Eventbenefits ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Binary, Name = "BenifitDocument", Value = model.otherDetails.Event_benefitsattach_FileBinary != null && model.otherDetails.Event_benefitsattach_FileBinary.Length > 0 ? Convert.ToBase64String(model.otherDetails.Event_benefitsattach_FileBinary) : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "BenifitDocumentFileType", Value = model.otherDetails.Event_benefitsattach != null? model.otherDetails.Event_benefitsattach.FileName : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "BenifitDocumentFileExt", Value = model.otherDetails.Event_benefitsattach!= null? model.otherDetails.Event_benefitsattach.FileName.GetFileExtensionTrimmed() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "BenifitDocumentFileName", Value = model.otherDetails.Event_benefitsattach!= null? model.otherDetails.Event_benefitsattach.FileName.GetFileNameWithoutPath() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "BenifitDocumentFileContentType", Value = model.otherDetails.Event_benefitsattach!= null? model.otherDetails.Event_benefitsattach.ContentType?.ToLower() : string.Empty},

                // step 4 - Sponsorship details
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SponsorshipDuration", Value = model.sponsorshipDetails.Eventsponsorshipduration ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "DidDewaSponsoredBefore", Value = model.sponsorshipDetails.Eventsponsorshipbefore ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "HowManyYears", Value = model.sponsorshipDetails.Eventtotalyears ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "InvestmentRequiredFromDEWA", Value = model.sponsorshipDetails.Eventinvestmentdewa ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "MediaCoverageType", Value = model.sponsorshipDetails.Eventtypeofmedia ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "CostMediaCoverage", Value = model.sponsorshipDetails.Eventcostofmedia ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "CommunicationOpportunities", Value = model.sponsorshipDetails.Eventcommunication ? "Yes" : "No"},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "LocalInternationalMediaCoverage", Value = model.sponsorshipDetails.Eventlocalmedia ? "Yes" : "No"},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "UseOfDEWALogo", Value = model.sponsorshipDetails.Eventuseofdewalogo ? "Yes" : "No"},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "DEWAStaffOpportunity", Value = model.sponsorshipDetails.Eventopportunityfordewa ? "Yes" : "No"},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "DEWAServiceOpportunity", Value = model.sponsorshipDetails.Eventopportunitytoreview ? "Yes" : "No"},
                new Attribute { Type = SponsorTypeEnum.Binary, Name = "SupportingEventDocument", Value = model.sponsorshipDetails.Event_supporteventdocument_FileBinary != null && model.sponsorshipDetails.Event_supporteventdocument_FileBinary.Length > 0 ? Convert.ToBase64String(model.sponsorshipDetails.Event_supporteventdocument_FileBinary) : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SupportingEventDocumentFileType", Value = model.sponsorshipDetails.Event_supporteventdocument != null? model.sponsorshipDetails.Event_supporteventdocument.FileName : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SupportingEventDocumentFileExt", Value = model.sponsorshipDetails.Event_supporteventdocument!= null? model.sponsorshipDetails.Event_supporteventdocument.FileName.GetFileExtensionTrimmed() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SupportingEventDocumentFileName", Value = model.sponsorshipDetails.Event_supporteventdocument!= null? model.sponsorshipDetails.Event_supporteventdocument.FileName.GetFileNameWithoutPath() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SupportingEventDocumentFileContentType", Value = model.sponsorshipDetails.Event_supporteventdocument!= null? model.sponsorshipDetails.Event_supporteventdocument.ContentType?.ToLower() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Binary, Name = "SponsorshipRequestLetter", Value = model.sponsorshipDetails.Event_letterofsponsorattach_FileBinary != null && model.sponsorshipDetails.Event_letterofsponsorattach_FileBinary.Length > 0 ? Convert.ToBase64String(model.sponsorshipDetails.Event_letterofsponsorattach_FileBinary) : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SponsorshipRequestLetterFileType", Value = model.sponsorshipDetails.Event_letterofsponsorattach != null? model.sponsorshipDetails.Event_letterofsponsorattach.FileName : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SponsorshipRequestLetterFileExtention", Value = model.sponsorshipDetails.Event_letterofsponsorattach!= null? model.sponsorshipDetails.Event_letterofsponsorattach.FileName.GetFileExtensionTrimmed() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SponsorshipRequestLetterFileName", Value = model.sponsorshipDetails.Event_letterofsponsorattach!= null? model.sponsorshipDetails.Event_letterofsponsorattach.FileName.GetFileNameWithoutPath() : string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "SponsorshipRequestLetterFileContentType", Value = model.sponsorshipDetails.Event_letterofsponsorattach!= null? model.sponsorshipDetails.Event_letterofsponsorattach.ContentType?.ToLower() : string.Empty},

                // Email configuration
                new Attribute { Type = SponsorTypeEnum.Text, Name = "RequestSubmittedOn", Value = string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "RequestSubmittedByEmail", Value = model.PersonEmail ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "RequestSubmittedByContact", Value = model.PersonContact ?? string.Empty},
                new Attribute { Type = SponsorTypeEnum.Text, Name = "RequestSubmittedByName", Value = model.PersonName ?? string.Empty},
            };
        }

        #region SponsorshipSuccess

        [HttpGet]
        public ActionResult SponsorshipSuccess()
        {
            SponsorshipModel sponsorshipModel = null;
            if (CacheProvider.TryGet("SponsorshipModel", out sponsorshipModel))
            {
                CacheProvider.Remove("SponsorshipModel");
                return PartialView("~/Views/Feature/GeneralServices/Sponsorship/SponsorshipSuccess.cshtml", new SponsorshipModel()
                {
                    IsSuccess = true
                });
            }
            return RedirectToSitecoreItem(SitecoreItemIdentifiers.SPONSORSHIP_REQUEST);
        }

        #endregion SponsorshipSuccess
    }
}