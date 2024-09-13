using DEWAXP.Feature.Bills.Models.LandlordInformation;
using DEWAXP.Foundation.Content;
using DEWAXP.Foundation.Content.Controllers;
using DEWAXP.Foundation.Content.Filters.Mvc;
using DEWAXP.Foundation.Helpers.Extensions;
using DEWAXP.Foundation.Integration.Requests;
using Sitecore.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DEWAXP.Feature.Bills.Controllers
{
    [TwoPhaseAuthorize]
    public class LandlordController : BaseWorkflowController<ChangeLandlordDetailsWorkflowState>
    {
        [HttpGet]
        public ActionResult ChangeLandlordInformation()
        {
            Clear();

            return PartialView("~/Views/Feature/Bills/LandlordInformation/_ChangeLandlordInformation.cshtml", new LandlordDetails
            {
                BusinessPartnerNumberList = GetBusinessPartners()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult ChangeLandlordInformation(LandlordDetails model)
        {
            //additional validation
            if (model.OfficialLetterUploader == null || model.TitleDeedUploader == null)
            {
                ModelState.AddModelError(string.Empty, Translate.Text("upload file validation message"));
            }

            string attachmentValidationMessage;
            if (!AttachmentIsValid(model.OfficialLetterUploader, General.MaxAttachmentSize, out attachmentValidationMessage, General.AcceptedFileTypes))
            {
                ModelState.AddModelError(string.Empty, attachmentValidationMessage);
            }

            if (!AttachmentIsValid(model.TitleDeedUploader, General.MaxAttachmentSize, out attachmentValidationMessage, General.AcceptedFileTypes))
            {
                ModelState.AddModelError(string.Empty, attachmentValidationMessage);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var @params = new LandlordDetailsParameters
                    {
                        BusinessPartnerNumber = model.SelectedBusinessPartnerNumber,
                        PremiseNumber = model.PremiseNumber,
                        Mobile = model.Mobile.AddMobileNumberZeroPrefix(),
                        Remarks = model.Remarks,
                        Attachment1 = model.OfficialLetterUploader.ToArray(),
                        FileType1 = model.OfficialLetterUploader.GetTrimmedFileExtension(),
                        Filename1 = model.OfficialLetterUploader.FileName.GetFileNameWithoutPath(),
                        Attachment2 = model.TitleDeedUploader.ToArray(),
                        FileType2 = model.TitleDeedUploader.GetTrimmedFileExtension(),
                        Filename2 = model.TitleDeedUploader.FileName.GetFileNameWithoutPath(),
                        UserId = CurrentPrincipal.UserId,
                        SessionId = CurrentPrincipal.SessionToken
                    };

                    var response = DewaApiClient.ChangeLandlordInformation(@params, RequestLanguage, Request.Segment());
                    if (response.Succeeded)
                    {
                        State.BusinessPartnerNumber = @params.BusinessPartnerNumber;
                        State.PremiseNumber = model.PremiseNumber;
                        State.Remarks = @params.Remarks;
                        State.Mobile = @params.Mobile;
                        State.OfficialLetter = @params.Attachment1;
                        State.OfficialLetterType = @params.FileType1;
                        State.OfficialLetterFileName = @params.Filename1.GetFileNameWithoutPath();
                        State.TitleDeed = @params.Attachment2;
                        State.TitleDeedFileName = @params.Filename2.GetFileNameWithoutPath();
                        State.TitleDeedType = @params.FileType2;
                        State.NotificationNumber = response.Payload.NotificationNumber;

                        Save();

                        return RedirectToSitecoreItem(SitecoreItemIdentifiers.J90_CHANGE_LANDLORD_INFORMATION_SUCCESS);
                    }
                    ModelState.AddModelError(string.Empty, response.Message);
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, Translate.Text("Unexpected error"));
                }
            }

            model.BusinessPartnerNumberList = GetBusinessPartners();

            return PartialView("~/Views/Feature/Bills/LandlordInformation/_ChangeLandlordInformation.cshtml", model);
        }

        [HttpGet]
        public ActionResult ChangeLandlordInformationSuccess()
        {
            return PartialView("~/Views/Feature/Bills/LandlordInformation/_LandlordDetailsSuccess.cshtml", State);
        }

        private List<SelectListItem> GetBusinessPartners()
        {
            //var accountList = DewaApiClient.GetAccountList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, false, RequestLanguage, Request.Segment());
            var accountList = SmartCustomerClient.GetCAList(CurrentPrincipal.UserId, CurrentPrincipal.SessionToken, string.Empty, string.Empty, false, RequestLanguage, Request.Segment());

            return accountList.Payload.Select(account => new SelectListItem()
            {
                Text = string.Format("{0} ({1})", Translate.Text("business partner label"), account.BusinessPartnerNumber.TrimStart('0')),
                Value = account.BusinessPartnerNumber
            }).DistinctBy(x => x.Value, null).ToList();
        }

        protected override string Name
        {
            get { return CacheKeys.CHANGE_LANDLORD_DETAILS_WORKFLOW_STATE; }
        }
    }
}